using System.Configuration;
using System.IO;
using System.Reflection;

namespace Element34.Utilities
{
    //static Assembly thisAssembly = Assembly.GetAssembly(typeof(nameOfClass));
    public static class Assets
    {
        /// <summary>
        /// Retrieves the specified [embedded] resource file and saves it to disk.  
        /// </summary>
        /// <param name="assembly">Previously loaded CLR assembly</param>
        /// <param name="nameSpace">Namespace of your project, located right above your class' name</param>
        /// <param name="outDirectory">Location where the file will be extracted</param>
        /// <param name="internalFilePath">Name of the folder inside visual studio where the file is located</param>
        /// <param name="resourceName">the name of the embedded file</param>
        public static void ExtractEmbeddedResource(Assembly assembly, string nameSpace, string outDirectory, string internalFilePath, string resourceName)
        {
            using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
            using (BinaryReader r = new BinaryReader(s))
            using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    fs.WriteByte((byte)s.ReadByte());
                }
            }
        }

        public static string GetConnectionString(Assembly assembly, string name)
        {
            // Get connection string from App.config
            var conStringCollection = ConfigurationManager.OpenExeConfiguration(assembly.Location).ConnectionStrings;
            return conStringCollection.ConnectionStrings[name].ConnectionString;
        }

        public static string GetAppSetting(Assembly assembly, string name)
        {
            // Get app setting value from App.config
            var SettingsCollection = ConfigurationManager.OpenExeConfiguration(assembly.Location).AppSettings;
            return SettingsCollection.Settings[name].Value;
        }
    }
}
