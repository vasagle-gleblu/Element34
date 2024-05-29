using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;  // Needed for accessing process information

namespace Element34.Utilities
{
    /// <summary>
    /// Specifies the type of base directory to retrieve.
    /// </summary>
    public enum BaseDirectoryType
    {
        /// <summary>
        /// The base directory derived from the code base location of the executing assembly.
        /// </summary>
        CodeBase,

        /// <summary>
        /// The directory where the assembly manifest, defining the executing assembly, is located.
        /// </summary>
        AssemblyLocation,

        /// <summary>
        /// The base directory that the assembly resolver uses to probe for assemblies.
        /// </summary>
        ApplicationDomain,

        /// <summary>
        /// The current working directory of the application.
        /// </summary>
        EnvironmentCurrentDirectory,

        /// <summary>
        /// The directory where the entry assembly is located.
        /// </summary>
        EntryAssemblyLocation,

        /// <summary>
        /// The directory of the main module of the current process.
        /// </summary>
        ProcessMainModule
    }

    /// <summary>
    /// Provides methods to search for specific folders based on various base directories.
    /// </summary>
    public static class GetFolder
    {
        /// <summary>
        /// Retrieves the directory path based on the specified base directory type.
        /// </summary>
        /// <param name="baseDirectoryType">The type of base directory to retrieve the path from.</param>
        /// <returns>The directory path associated with the specified <see cref="BaseDirectoryType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid or unsupported base directory type is provided.</exception>

        private static string GetBaseDirectory(BaseDirectoryType baseDirectoryType)
        {
            switch (baseDirectoryType)
            {
                case BaseDirectoryType.CodeBase:
                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    UriBuilder uri = new UriBuilder(codeBase);
                    return Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

                case BaseDirectoryType.AssemblyLocation:
                    return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                case BaseDirectoryType.ApplicationDomain:
                    return AppDomain.CurrentDomain.BaseDirectory;

                case BaseDirectoryType.EnvironmentCurrentDirectory:
                    return Environment.CurrentDirectory;

                case BaseDirectoryType.EntryAssemblyLocation:
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                        return Path.GetDirectoryName(entryAssembly.Location);
                    return null;

                case BaseDirectoryType.ProcessMainModule:
                    return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                default:
                    throw new ArgumentException("Invalid base directory type");
            }
        }

        /// <summary>
        /// Searches for a folder with the specified name in various predefined base directories.
        /// </summary>
        /// <param name="sInput">The name of the folder to search for.</param>
        /// <returns>The full path to the first found directory matching the specified folder name.</returns>
        /// <exception cref="ArgumentException">Thrown when the folder name is null or empty.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the folder is not found in any base paths.</exception>

        public static string searchForFolder(string sInput)
        {
            if (string.IsNullOrEmpty(sInput))
            {
                throw new ArgumentException("Folder name must be specified.");
            }

            BaseDirectoryType[] basePaths = {
                BaseDirectoryType.AssemblyLocation,
                BaseDirectoryType.ApplicationDomain,
                BaseDirectoryType.CodeBase,
                BaseDirectoryType.EnvironmentCurrentDirectory,
                BaseDirectoryType.EntryAssemblyLocation,
                BaseDirectoryType.ProcessMainModule
            };

            foreach (var basePath in basePaths)
            {
                string currentFolder = GetBaseDirectory(basePath);
                if (currentFolder != null)
                {
                    string foundFolder = TraverseDirectoriesForFolder(currentFolder, sInput);
                    if (foundFolder != null)
                    {
                        return foundFolder;
                    }
                }
            }

            throw new DirectoryNotFoundException($"Folder '{sInput}' not found in any base paths.");
        }

        /// <summary>
        /// Recursively traverses directories upward from the specified start folder to find a directory with the specified name.
        /// </summary>
        /// <param name="startFolder">The starting directory for the search.</param>
        /// <param name="folderName">The name of the folder to search for.</param>
        /// <returns>The full path to the found directory, or null if the folder is not found.</returns>

        private static string TraverseDirectoriesForFolder(string startFolder, string folderName)
        {
            string currentFolder = startFolder;
            while (true)
            {
                string[] foundFolders = Directory.GetDirectories(currentFolder, folderName, SearchOption.TopDirectoryOnly);
                if (foundFolders.Length > 0)
                {
                    return foundFolders[0];
                }

                string parentFolder = Directory.GetParent(currentFolder)?.FullName;
                if (string.IsNullOrEmpty(parentFolder) || parentFolder == currentFolder)
                {
                    break; // Reached the root folder
                }
                currentFolder = parentFolder;
            }

            return null;
        }
    }
}
