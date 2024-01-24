using System;
using System.Diagnostics;
using System.Reflection;

namespace Element34.Utilities
{
    public class PropertiesRetriever
    {
        public PropertyInfo[] RetrieveProperties(object obj)
        {
            Type type = obj.GetType();
            return type.GetProperties();
        }

        public PropertyInfo[] RetrievePropertiesWithFilter(object obj, BindingFlags binding)
        {
            Type type = obj.GetType();
            return type.GetProperties(binding);
        }

        public PropertyInfo[] RetrieveParentClassPropertiesWithFilter(object obj, BindingFlags binding)
        {
            Type type = obj.GetType();
            return type.BaseType?.GetProperties(binding);
        }

        public void PrintProperties(object obj)
        {
            Type type = obj.GetType();

            foreach (var item in type.GetProperties())
            {
                Debug.WriteLine($" * {item.Name}: {item.GetValue(obj)}");
            }

            Debug.WriteLine("--");
            Debug.WriteLine("");
        }

    }
}
