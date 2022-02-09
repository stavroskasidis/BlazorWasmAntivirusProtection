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
    using System.Text;

    public class RenameDlls : Task
    {
        [Required]
        public string PublishDir { get; set; }

        public string RenameDllsTo { get; set; } = "bin";
        public bool DisableRenamingDlls { get; set; }

        public override bool Execute()
        {
            if (DisableRenamingDlls)
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Skipping renaming .dll files");
                return true;
            }
            Log.LogMessage(MessageImportance.High,$"BlazorWasmAntivirusProtection: Renaming .dll files to .{RenameDllsTo}");
            var frameworkDir = Directory.GetDirectories(PublishDir, "_framework", SearchOption.AllDirectories).First();
            var wwwrootDir = Directory.GetDirectories(PublishDir, "wwwroot", SearchOption.AllDirectories).First();

            foreach (var file in Directory.GetFiles(frameworkDir,"*.*", SearchOption.AllDirectories))
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
            var serviceWorkerPath = Path.Combine(wwwrootDir, "service-worker-assets.js");


            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Updating \"{bootJsonPath}\"");
            var bootJson = File.ReadAllText(bootJsonPath);
            bootJson = bootJson.Replace(".dll", $".{RenameDllsTo}");
            File.WriteAllText(bootJsonPath, bootJson);
            
            if (File.Exists(serviceWorkerPath))
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Updating \"{serviceWorkerPath}\"");
                var serviceWorker = File.ReadAllText(serviceWorkerPath);
                serviceWorker = serviceWorker.Replace(".dll", $".{RenameDllsTo}");
                File.WriteAllText(serviceWorkerPath, serviceWorker);
            }

            if (File.Exists(bootJsonGzPath))
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Deleting \"{bootJsonGzPath}\"");
                File.Delete(bootJsonGzPath);
            }
            if (File.Exists(bootJsonBrPath))
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Deleting \"{bootJsonBrPath}\"");
                File.Delete(bootJsonBrPath);
            }
            Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Renaming .dll files to .{RenameDllsTo} finished");

            return true;
        }
    }
}
