using System;
using System.Diagnostics;
using System.IO;

namespace Element34.Utilities
{
    /// <summary>
    /// Manages the creation and automatic deletion of a temporary file.
    /// </summary>
    public sealed class TemporaryFile : IDisposable
    {
        /// <summary>
        /// Gets the full path of the temporary file.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFile"/> class
        /// using the system's temporary folder as the default directory.
        /// </summary>
        public TemporaryFile() : this(Path.GetTempPath())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFile"/> class,
        /// creating a temporary file in the specified directory.
        /// </summary>
        /// <param name="directory">The directory in which to create the temporary file.</param>
        /// <exception cref="ArgumentException">Thrown if the specified directory does not exist.</exception>
        public TemporaryFile(string directory)
        {
            if (!Directory.Exists(directory))
                throw new ArgumentException("Directory does not exist.", nameof(directory));

            Create(Path.Combine(directory, Path.GetRandomFileName()));
        }

        /// <summary>
        /// Releases all resources used by the <see cref="TemporaryFile"/>.
        /// </summary>
        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer that ensures the temporary file is deleted when the object is collected by garbage collection.
        /// </summary>
        ~TemporaryFile()
        {
            Delete();
        }

        /// <summary>
        /// Creates a temporary file at the specified path.
        /// </summary>
        /// <param name="path">The full file path where the temporary file will be created.</param>
        private void Create(string path)
        {
            FilePath = path;
            using (var stream = File.Create(FilePath)) { }
        }

        /// <summary>
        /// Deletes the temporary file.
        /// </summary>
        private void Delete()
        {
            if (FilePath != null && File.Exists(FilePath))
            {
                try
                {
                    File.Delete(FilePath);
                }
                catch (IOException e)
                {
                    Debug.WriteLine("Failed to delete temporary file: " + e.Message);
                }
                FilePath = null;
            }
        }
    }

}
