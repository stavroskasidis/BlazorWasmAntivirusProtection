using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace BlazorWasmAntivirusProtection.Tasks
{
    public static class Tools
    {
        public static bool GZipCompress(string source, string target, TaskLoggingHelper log)
        {
            try
            {
                File.Delete(target);
                using var fileStream = File.OpenRead(source);
                using var stream = File.Create(target);
                using var destination = new GZipStream(stream, CompressionLevel.Optimal);
                fileStream.CopyTo(destination);
            }
            catch (Exception ex)
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                log.LogErrorFromException(ex, true, true, null);
                return false;
            }
            return true;
        }

        public static bool BrotliCompress(string source, string target, string brotliCompressToolPath, string compressionLevel, TaskLoggingHelper log)
        {
            try
            {
                // NOTE: This MSBuild Custom Task will run not only on .NET 6 or later but also on .NET Framework 4.x.
                //       Therefore the `BrotliStream` class is not usable in this MSBuild Custom Task due to
                //       the `BrotliStream` class is not provided on.NET Framework.Instead, we can do that
                //       with execution as an out process.
                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"exec \"{brotliCompressToolPath}\" \"{source}\" \"{target}\" \"{compressionLevel}\""
                };
                log.LogMessage(MessageImportance.Low, $"{startInfo.FileName} {startInfo.Arguments}");
                var process = Process.Start(startInfo);
                process.WaitForExit();
                if (process.ExitCode != 0)
                    throw new Exception($"The exit code of recompressing with Brotli command was not 0. (it was {process.ExitCode})");
            }
            catch (Exception ex)
            {
                if (File.Exists(target))
                {
                    File.Delete(target);
                }
                log.LogErrorFromException(ex, true, true, null);
                return false;
            }
            return true;
        }

        public static string ComputeSha256Hash(string rawData)
        {
            using var sha256Hash = SHA256.Create();
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(hash);
        }
    }
}
