using System;
using System.IO;
using System.Reflection;

namespace Element34.Utilities
{
    public static class GetFolder
    {
        public static string CodeBase
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            }
        }

        public static string AssemblyLocation
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string ApplicationDomain
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        public static string searchUpForFolder(string sInput)
        {
            string currentFolder = AssemblyLocation;
            string[] list = Directory.GetDirectories(currentFolder, sInput);

            while (list.Length == 0)
            {
                currentFolder = Path.GetFullPath(Path.Combine(currentFolder, ".."));
                list = Directory.GetDirectories(currentFolder, sInput);
            }

            return list[0];
        }
    }
}
