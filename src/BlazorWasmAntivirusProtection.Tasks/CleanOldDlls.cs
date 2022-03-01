namespace BlazorWasmAntivirusProtection.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    public class CleanOldDlls : Task
    {
        [Required]
        public string IntermediateLinkDir { get; set; }

        public override bool Execute()
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
            if (!Directory.Exists(IntermediateLinkDir)) return true;

            var linkSemaphore = Path.Combine(IntermediateLinkDir, "Link.semaphore");
            var existingDll = Directory.GetFiles(IntermediateLinkDir, "*.dll").FirstOrDefault();
            if (File.Exists(linkSemaphore) && existingDll != null && IsDllHeaderBz(existingDll))
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Cleaning old .dll files in \"{IntermediateLinkDir}\"");
                //We delete the Link.semaphore file to force a regeneration of objs in the IntermediateLinkDir
                //This is to remove remnants of a previous publish
                File.Delete(linkSemaphore);
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
