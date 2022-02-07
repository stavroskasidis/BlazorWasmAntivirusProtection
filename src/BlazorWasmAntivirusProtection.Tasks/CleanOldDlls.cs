namespace BlazorWasmAntivirusProtection.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Text;

    public class CleanOldDlls : Task
    {
        [Required]
        public string IntermediateLinkDir { get; set; }

        public override bool Execute()
        {
            Debugger.Launch();
            if (!Directory.Exists(IntermediateLinkDir)) return true;

            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Cleaning old .dll files");

            foreach(var file in Directory.GetFiles(IntermediateLinkDir, "*.dll"))
            {
                if (IsDllHeaderBz(file))
                {
                    File.Delete(file);
                }
            }


            return true;
        }

        bool IsDllHeaderBz(string fn)
        {
            using var fs = File.Open(fn, FileMode.Open, FileAccess.ReadWrite);
            using var reader = new BinaryReader(fs);
            var buffer = new byte[2];
            reader.Read(buffer, 0 , buffer.Length);

            var bz = Encoding.ASCII.GetBytes("BZ");
            return bz[0] == buffer[0] && bz[1] == buffer[1];
        }
    }
}
