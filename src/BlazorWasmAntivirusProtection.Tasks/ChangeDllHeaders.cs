namespace BlazorWasmAntivirusProtection.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Text;

    public class ChangeDllHeaders : Task
    {
        [Required]
        public ITaskItem[] PublishBlazorBootStaticWebAsset { get; set; }

        public override bool Execute()
        {
            if (PublishBlazorBootStaticWebAsset.Length == 0) return true;

            Log.LogMessage(MessageImportance.High, "Changing dll headers from MZ to BZ");
            foreach (var asset in PublishBlazorBootStaticWebAsset)
            {
                var name = Path.GetFileName(asset.GetMetadata("RelativePath"));
                if (Path.GetExtension(name) != ".dll") continue;

                FlipBz(asset.ItemSpec);
                Log.LogMessage(MessageImportance.High, $"Changed headers of {asset.ItemSpec}");
            }

            return true;
        }

        void FlipBz(string fn)
        {
            using var fs = File.Open(fn, FileMode.Open, FileAccess.ReadWrite);
            using var bw = new BinaryWriter(fs);
            var bz = Encoding.ASCII.GetBytes("BZ"); //This changes header from MZ to BZ to prevent firewalls and
                                                    //antiviruses from identifing the dll as executable
            bw.Write(bz);
        }
    }
}
