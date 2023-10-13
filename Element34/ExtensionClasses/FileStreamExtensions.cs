using System;
using System.IO;
using System.Text;

namespace Element34
{
    public static class FileStreamExtensions
    {
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

        public static bool EndOfStream(this FileStream fileStream)
        {
            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            return fileStream.Position == fileStream.Length;
        }
    }
}
