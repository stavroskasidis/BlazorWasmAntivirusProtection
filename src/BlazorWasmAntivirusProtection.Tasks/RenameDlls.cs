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

        public string BinaryExtensionName { get; set; } = "bin";

        public override bool Execute()
        {
            
            Log.LogMessage(MessageImportance.High,$"Renaming .dll files to .{BinaryExtensionName}");
            var frameworkDir = Directory.GetDirectories(PublishDir, "_framework", SearchOption.AllDirectories).First();
            foreach(var file in Directory.GetFiles(frameworkDir,"*.*", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".dll") || file.EndsWith(".dll.gz") || file.EndsWith(".dll.br"))
                {
                    var newName = file.Replace(".dll", $".{BinaryExtensionName}");
                    Log.LogMessage(MessageImportance.High, $"Renaming \"{file}\" to \"{newName}\"");
                    File.Move(file, newName);
                }
            }
            var bootJsonPath = Path.Combine(frameworkDir, "blazor.boot.json");
            var bootJsonGzPath = Path.Combine(frameworkDir, "blazor.boot.json.gz");
            var bootJsonBrPath = Path.Combine(frameworkDir, "blazor.boot.json.br");
            var serviceWorkerPath = Path.Combine(frameworkDir, "service-worker-assets.js");


            Log.LogMessage(MessageImportance.High, $"Updating \"{bootJsonPath}\"");
            var bootJson = File.ReadAllText(bootJsonPath);
            bootJson = bootJson.Replace(".dll", $".{BinaryExtensionName}");
            File.WriteAllText(bootJsonPath, bootJson);
            
            if (File.Exists(serviceWorkerPath))
            {
                Log.LogMessage(MessageImportance.High, $"Updating \"{serviceWorkerPath}\"");
                var serviceWorker = File.ReadAllText(serviceWorkerPath);
                serviceWorker = serviceWorker.Replace(".dll", $".{BinaryExtensionName}");
                File.WriteAllText(serviceWorkerPath, serviceWorker);
            }

            if (File.Exists(bootJsonGzPath))
            {
                Log.LogMessage(MessageImportance.High, $"Deleting \"{bootJsonGzPath}\"");
                File.Delete(bootJsonGzPath);
            }
            if (File.Exists(bootJsonBrPath))
            {
                Log.LogMessage(MessageImportance.High, $"Deleting \"{bootJsonBrPath}\"");
                File.Delete(bootJsonBrPath);
            }

            return true;
        }
    }
}
