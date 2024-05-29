using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Element34.Utilities
{
    /// <summary>
    /// Provides a set of static methods for managing embedded resources and configuration settings within a .NET assembly.
    /// This class facilitates the extraction of embedded files to disk, retrieval of strings and connection strings from
    /// resources, and access to application settings. It is designed to enhance ease of access to various types of
    /// resources bundled within an assembly, making it ideal for applications that require dynamic resource management
    /// based on runtime conditions or configurations.
    /// </summary>

    public static class Assets
    {
        /// <summary>
        /// Extracts an embedded resource from a given assembly and saves it to a specified directory.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="nameSpace">The root namespace of the project.</param>
        /// <param name="outDirectory">The directory to save the extracted file.</param>
        /// <param name="internalFilePath">The internal path within the project for the resource.</param>
        /// <param name="resourceName">The name of the resource to extract.</param>
        public static void ExtractEmbeddedResource(Assembly assembly, string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrEmpty(outDirectory)) throw new ArgumentNullException(nameof(outDirectory));
            if (!Directory.Exists(outDirectory)) Directory.CreateDirectory(outDirectory);

            string resourcePath = $"{nameSpace}.{internalFilePath}.{resourceName}".Replace("..", ".");
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null) throw new ArgumentException("Resource not found.", nameof(resourceName));

                using (FileStream fs = new FileStream(Path.Combine(outDirectory, resourceName), FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fs);
                }
            }
        }

        /// <summary>
        /// Retrieves a string from an embedded resource within a .NET assembly.
        /// </summary>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <param name="baseName">The root namespace where the resource manager looks for the resource.</param>
        /// <param name="resourceName">The name of the embedded resource.</param>
        /// <returns>The string content of the embedded resource.</returns>
        public static string GetEmbeddedResourceString(Assembly assembly, string baseName, string resourceName)
        {
            ResourceManager rm = new ResourceManager(baseName, assembly);
            return rm.GetString(resourceName) ?? throw new ArgumentException("Resource not found.", nameof(resourceName));
        }

        /// <summary>
        /// Retrieves a database connection string from the configuration of a specific assembly.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the configuration from.</param>
        /// <param name="name">The name of the connection string.</param>
        /// <returns>The connection string.</returns>
        public static string GetConnectionString(Assembly assembly, string name)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(assembly.Location);
            ConnectionStringSettings connectionString = config.ConnectionStrings.ConnectionStrings[name] ?? throw new ArgumentException("Connection string not found.", nameof(name));
            return connectionString.ConnectionString;
        }

        /// <summary>
        /// Retrieves an application setting from the configuration file associated with a specific .NET assembly.
        /// </summary>
        /// <param name="assembly">The assembly to retrieve the configuration from.</param>
        /// <param name="name">The name of the setting to retrieve.</param>
        /// <returns>The value of the setting.</returns>
        public static string GetAppSetting(Assembly assembly, string name)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(assembly.Location);
            KeyValueConfigurationElement setting = config.AppSettings.Settings[name] ?? throw new ArgumentException("App setting not found.", nameof(name));
            return setting.Value;
        }
    }

}
