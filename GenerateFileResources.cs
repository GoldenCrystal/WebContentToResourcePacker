using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Resources;

public class GenerateFileResources : Task
{
    private sealed class LazyFileStream : Stream
    {
        private readonly string _path;
        private FileStream _baseStream;

        public LazyFileStream(string path) => _path = path;

        private FileStream BaseStream => _baseStream ?? (_baseStream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.Read));

        protected override void Dispose(bool disposing)
        {
            _baseStream.Dispose();
            _baseStream = null;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => BaseStream.Length;
        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public override void Flush() { }
        public override int Read(byte[] buffer, int offset, int count) => BaseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => BaseStream.Seek(offset, origin);
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }

    [Required]
    public ITaskItem[] InputFiles { get; set; }

    [Output, Required]
    public ITaskItem[] OutputFile { get; set; }

    private static string GetResourceName(ITaskItem item)
    {
        return item.GetMetadata("LogicalName");
    }

    public override bool Execute()
    {
        string outputPath = OutputFile[0].ItemSpec;

        Log.LogMessageFromText("Starting generation of resource file " + outputPath + ".", MessageImportance.Low);
        using (var writer = new ResourceWriter(outputPath))
        {
            foreach (var file in InputFiles)
            {
                string sourcePath = file.ItemSpec;
                string resourceName = GetResourceName(file);
                writer.AddResource(resourceName, new LazyFileStream(sourcePath), true);
            }

            writer.Generate();
        }
        Log.LogMessageFromText("Finished generation of resource file " + outputPath + ".", MessageImportance.Low);

        return true;
    }
}
