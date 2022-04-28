namespace BlazorWasmAntivirusProtection.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;

    public class ObfuscateDlls : Task
    {
        [Required]
        public ITaskItem[] PublishBlazorBootStaticWebAsset { get; set; }

        [Required]
        public string SettingsPath { get; set; }

        public bool OriginalBlazorCacheBootResources { get; set; }

        public string ObfuscationMode { get; set; } = Tasks.ObfuscationMode.Xor.ToString();

        public string XorKey { get; set; } = "blazor is not a virus!!";

        [Output]
        public ITaskItem[] Extension { get; set; }

        public override bool Execute()
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
            if (PublishBlazorBootStaticWebAsset.Length == 0) return true;
            if(!Enum.TryParse<ObfuscationMode>(ObfuscationMode, out var obfuscationMode))
            {
                return false;
            }

            if (obfuscationMode == Tasks.ObfuscationMode.None)
            {
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Skipping obfuscating .dlls");
            }
            else if (obfuscationMode == Tasks.ObfuscationMode.ChangeHeaders)
            {
                Log.LogMessage(MessageImportance.High, "BlazorWasmAntivirusProtection: Changing .dll headers from MZ to BZ");
                foreach (var asset in PublishBlazorBootStaticWebAsset)
                {
                    var name = Path.GetFileName(asset.GetMetadata("RelativePath"));
                    if (Path.GetExtension(name) != ".dll") continue;

                    ChangeDllHeaderToBz(asset.ItemSpec);
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Changed header of {asset.ItemSpec}");
                }
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Changing .dll headers from MZ to BZ finished");

            }
            else if(obfuscationMode == Tasks.ObfuscationMode.Xor)
            {
                var key = Encoding.ASCII.GetBytes(XorKey);
                Log.LogMessage(MessageImportance.High, "BlazorWasmAntivirusProtection: Xor'ing .dlls");
                foreach (var asset in PublishBlazorBootStaticWebAsset)
                {
                    var name = Path.GetFileName(asset.GetMetadata("RelativePath"));
                    if (Path.GetExtension(name) != ".dll") continue;

                    XorDllWithKey(asset.ItemSpec, key);
                    Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Xor'ed dll {asset.ItemSpec}");
                }
                Log.LogMessage(MessageImportance.High, $"BlazorWasmAntivirusProtection: Xor'ing .dlls finished");
            }

            var settings = JsonSerializer.Serialize(new
            {
                obfuscationMode = obfuscationMode,
                xorKey = XorKey,
                cacheBootResourcesObfuscated =  OriginalBlazorCacheBootResources
            });
            File.WriteAllText(SettingsPath, settings);

            var bundleItem = new TaskItem(SettingsPath);
            bundleItem.SetMetadata("RelativePath", "avp-settings.json");
            bundleItem.SetMetadata("ExtensionName", "avpsettings");

            Extension = new ITaskItem[] { bundleItem };

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

        void XorDllWithKey(string fn,byte[] key)
        {
            var data = File.ReadAllBytes(fn);
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)(data[i] ^ key[i % key.Length]);

            File.WriteAllBytes(fn, data);
        }
    }
}
