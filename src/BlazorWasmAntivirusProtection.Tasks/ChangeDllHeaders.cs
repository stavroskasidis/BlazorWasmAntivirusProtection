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
        public bool DisableChangingDllHeaders { get; set; }

        public override bool Execute()
        {
            if (PublishBlazorBootStaticWebAsset.Length == 0) return true;

            if (DisableChangingDllHeaders)
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Skipping changing .dll headers");
                return true;
            }


            Log.LogMessage(MessageImportance.High, "BlazorWasmAntivirusProtection: Changing .dll headers from MZ to BZ");
            foreach (var asset in PublishBlazorBootStaticWebAsset)
            {
                var name = Path.GetFileName(asset.GetMetadata("RelativePath"));
                if (Path.GetExtension(name) != ".dll") continue;

                ChangeDllHeaderToBz(asset.ItemSpec);
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Changed header of {asset.ItemSpec}");
            }
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Changing .dll headers from MZ to BZ finished");

            return true;
        }

        void ChangeDllHeaderToBz(string fn)
        {
            using var fs = File.Open(fn, FileMode.Open, FileAccess.ReadWrite);
            using var bw = new BinaryWriter(fs);
            var bz = Encoding.ASCII.GetBytes("BZ"); //This changes header from MZ to BZ to prevent firewalls and
                                                    //antiviruses from identifing the dll as executable
            bw.Write(bz);
        }
    }
}
