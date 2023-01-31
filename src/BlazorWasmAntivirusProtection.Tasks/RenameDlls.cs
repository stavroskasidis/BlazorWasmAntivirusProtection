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

    public class RenameDlls : Task
    {
        [Required]
        public string PublishDir { get; set; }
        [Required]
        public string BrotliCompressToolPath { get; set; }

        public string ScriptQueryString { get; set; }
        public string RenameDllsTo { get; set; } = "bin";
        public bool DisableRenamingDlls { get; set; }
        public bool BlazorEnableCompression { get; set; } = true;
        public string CompressionLevel { get; set; }

        public override bool Execute()
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
            if (DisableRenamingDlls)
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Skipping renaming .dll files");
                return true;
            }
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Renaming .dll files to .{RenameDllsTo}");
            var frameworkDirs = Directory.GetDirectories(PublishDir, "_framework", SearchOption.AllDirectories);
            foreach(var frameworkDir in frameworkDirs)
            {
                foreach (var file in Directory.GetFiles(frameworkDir, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".dll") || file.EndsWith(".dll.gz") || file.EndsWith(".dll.br"))
                    {
                        var newName = file.Replace(".dll", $".{RenameDllsTo}");
                        Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Renaming \"{file}\" to \"{newName}\"");
                        if (File.Exists(newName)) File.Delete(newName);
                        File.Move(file, newName);
                    }
                }
                var bootJsonPath = Path.Combine(frameworkDir, "blazor.boot.json");
                var bootJsonGzPath = Path.Combine(frameworkDir, "blazor.boot.json.gz");
                var bootJsonBrPath = Path.Combine(frameworkDir, "blazor.boot.json.br");
                var serviceWorkerPathAssets = Path.Combine(frameworkDir, $"..{Path.DirectorySeparatorChar}service-worker-assets.js");


                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Updating \"{bootJsonPath}\"");
                var bootJson = File.ReadAllText(bootJsonPath);
                if (!string.IsNullOrWhiteSpace(ScriptQueryString))
                {
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Cache busting \"BlazorWasmAntivirusProtection.lib.module.js\" with query string: {ScriptQueryString}");
                    bootJson = bootJson.Replace("BlazorWasmAntivirusProtection.lib.module.js", $"BlazorWasmAntivirusProtection.lib.module.js?cache={ScriptQueryString}");
                }
                bootJson = bootJson.Replace(".dll", $".{RenameDllsTo}");
                File.WriteAllText(bootJsonPath, bootJson);

                if (File.Exists(serviceWorkerPathAssets))
                {
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Updating \"{serviceWorkerPathAssets}\"");
                    var serviceWorkerAssets = File.ReadAllText(serviceWorkerPathAssets);
                    serviceWorkerAssets = serviceWorkerAssets.Replace(".dll", $".{RenameDllsTo}");
                    var assetsManifest = JsonSerializer.Deserialize<AssetsManifest>(serviceWorkerAssets.Replace("self.assetsManifest = ", "").Trim().TrimEnd(';'));
                    var bootJsonAssetEntry = assetsManifest.assets.First(x => x.url.EndsWith("blazor.boot.json"));
                    bootJsonAssetEntry.hash = $"sha256-{Tools.ComputeSha256Hash(bootJson)}";
                    //var bootJsonModel = JsonSerializer.Deserialize<BlazorBoot>(bootJson);
                    //foreach(var assembly in bootJsonModel.resources.assembly)
                    //{
                    //    var assemblyName = assembly.Key;
                    //    var assemblyHash = assembly.Value;
                    //    var asset = assetsManifest.assets.First(x => x.url.EndsWith(assemblyName));
                    //    asset.hash = assemblyHash;
                    //}
                    serviceWorkerAssets = $"self.assetsManifest = {JsonSerializer.Serialize(assetsManifest, new JsonSerializerOptions { WriteIndented = true })};";
                    File.WriteAllText(serviceWorkerPathAssets, serviceWorkerAssets);
                }

                if (File.Exists(bootJsonGzPath) && BlazorEnableCompression)
                {
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing \"{bootJsonGzPath}\"");
                    if(!Tools.GZipCompress(bootJsonPath, bootJsonGzPath,Log))
                    {
                        return false;
                    }
                }
                if (File.Exists(bootJsonBrPath) && BlazorEnableCompression)
                {
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Recompressing \"{bootJsonBrPath}\"");
                    if(!Tools.BrotliCompress(bootJsonPath, bootJsonBrPath, BrotliCompressToolPath, CompressionLevel, Log))
                    {
                        return false;
                    }
                }
            }
            
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Renaming .dll files to .{RenameDllsTo} finished");

            return true;
        }
    }
}
