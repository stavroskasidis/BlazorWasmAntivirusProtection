namespace BlazorWasmAntivirusProtection.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;

    public class RecompressSatelliteAssemblies : Task
    {
        [Required]
        public string PublishDir { get; set; }
        [Required]
        public string BrotliCompressToolPath { get; set; }

        public string CompressionLevel { get; set; }

        public override bool Execute()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing satellite assemblies");
            var frameworkDirs = Directory.GetDirectories(PublishDir, "_framework", SearchOption.AllDirectories);
            foreach(var frameworkDir in frameworkDirs)
            {
                foreach (var file in Directory.GetFiles(frameworkDir, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".resources.bin"))
                    {
                        var gzFile = $"{file}.gz";
                        var brFile = $"{file}.br";
                        if (File.Exists(gzFile))
                        {
                            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing \"{gzFile}\"");
                            if (!Tools.GZipCompress(file, gzFile, Log)) return false;
                        }
                        if (File.Exists(brFile))
                        {
                            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing \"{brFile}\"");
                            if (!Tools.BrotliCompress(file, gzFile, BrotliCompressToolPath, CompressionLevel, Log)) return false;
                        }
                    }
                }
            }
            
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing satellite assemblies finished");

            return true;
        }


    }
}
