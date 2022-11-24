using System.IO.Compression;

try
{
    var bootJsonPath = args[0];
    var bootJsonBrPath = args[1];
    var compressionLeveText = args[2];

    File.Delete(bootJsonBrPath);
    var compressionLevel = Enum.TryParse<CompressionLevel>(compressionLeveText, out var level) ? level : CompressionLevel.Optimal;
    using var fileStream = File.OpenRead(bootJsonPath);
    using var stream = File.Create(bootJsonBrPath);
    using var destination = new BrotliStream(stream, compressionLevel);
    fileStream.CopyTo(destination);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.ToString());
    return 1;
}