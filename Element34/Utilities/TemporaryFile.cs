using System;
using System.IO;

namespace Element34.Utilities
{
    public sealed class TemporaryFile : IDisposable
    {
        // Plagiarized from StackOverflow.com
        public TemporaryFile() : this(Path.GetTempPath())
        { }

        public TemporaryFile(string directory)
        {
            Create(Path.Combine(directory, Path.GetRandomFileName()));
        }

        ~TemporaryFile()
        {
            Delete();
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        public string FilePath { get; private set; }

        private void Create(string path)
        {
            FilePath = path;
            using (File.Create(FilePath)) { };
        }

        private void Delete()
        {
            if (FilePath == null) return;
            File.Delete(FilePath);
            FilePath = null;
        }

        // Usage:
        //using (var tempFile = new TemporaryFile())
        //{
        //    use the file through tempFile.FilePath...
        //}

    }
}
