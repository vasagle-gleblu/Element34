using System;
using System.IO;
using System.Text;

namespace Element34
{
    /// <summary>
    /// Provides extension methods for the <see cref="FileStream"/> class.
    /// </summary>
    public static class FileStreamExtensions
    {
        /// <summary>
        /// Reads a line from the file stream.
        /// </summary>
        /// <param name="fileStream">The file stream to read from.</param>
        /// <returns>The line read from the file stream.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="fileStream"/> is null.</exception>
        public static string ReadLine(this FileStream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            StringBuilder line = new StringBuilder();
            int byteRead;

            while ((byteRead = fileStream.ReadByte()) != -1)
            {
                char character = (char)byteRead;

                // Check for newline characters ('\n' or '\r\n')
                if (character == '\n')
                {
                    break; // End of line
                }
                else if (character != '\r') // Ignore '\r' characters
                {
                    line.Append(character);
                }
            }

            return line.ToString();
        }

        /// <summary>
        /// Writes a line to the file stream using the specified encoding.
        /// </summary>
        /// <param name="fileStream">The file stream to write to.</param>
        /// <param name="line">The line to write.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileStream"/>, <paramref name="line"/>, or <paramref name="encoding"/> is null.</exception>
        public static void WriteLine(this FileStream fileStream, string line, Encoding encoding)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));
            if (line == null)
                throw new ArgumentNullException(nameof(line));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            byte[] lineBytes = encoding.GetBytes(line + Environment.NewLine);
            fileStream.Write(lineBytes, 0, lineBytes.Length);
        }

        /// <summary>
        /// Determines whether the end of the file stream has been reached.
        /// </summary>
        /// <param name="fileStream">The file stream to check.</param>
        /// <returns><c>true</c> if the end of the file stream has been reached; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="fileStream"/> is null.</exception>
        public static bool EndOfStream(this FileStream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            return fileStream.Position == fileStream.Length;
        }
    }
}
