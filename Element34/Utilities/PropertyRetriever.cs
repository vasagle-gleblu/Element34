using System;
using System.Diagnostics;
using System.Reflection;

namespace Element34.Utilities
{
    /// <summary>
    /// Provides utility methods for retrieving and printing property information of objects.
    /// This class uses reflection to inspect the properties of objects and can handle properties
    /// from the object itself, its base classes, or according to specific binding flags. It also 
    /// includes functionality to print property values, which can be useful for debugging and logging.
    /// </summary>

    public class PropertiesRetriever
    {
        /// <summary>
        /// Retrieves all public properties of the specified object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object from which to retrieve properties.</param>
        /// <returns>An array of PropertyInfo objects representing all public properties of the object.</returns>
        public PropertyInfo[] RetrieveProperties<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();
            return type.GetProperties();
        }

        /// <summary>
        /// Retrieves properties of the specified object using the provided binding flags.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object from which to retrieve properties.</param>
        /// <param name="binding">The binding flags to use for retrieving properties.</param>
        /// <returns>An array of PropertyInfo objects representing the properties of the object.</returns>
        public PropertyInfo[] RetrieveProperties<T>(T obj, BindingFlags binding)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();
            return type.GetProperties(binding);
        }

        /// <summary>
        /// Retrieves properties of the parent class of the specified object using provided binding flags.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object whose parent class properties to retrieve.</param>
        /// <param name="binding">The binding flags to use for retrieving properties.</param>
        /// <returns>An array of PropertyInfo objects representing the properties of the parent class of the object.</returns>
        public PropertyInfo[] RetrieveParentClassProperties<T>(T obj, BindingFlags binding)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType().BaseType;
            return type?.GetProperties(binding) ?? Array.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Prints the properties and their values of the specified object to the debug output.
        /// </summary>
        /// <param name="obj">The object whose properties to print.</param>
        public void PrintProperties(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            Type type = obj.GetType();
            foreach (PropertyInfo item in type.GetProperties())
            {
                object value = item.GetValue(obj, null);
                Debug.WriteLine($" * {item.Name}: {value}");
            }

            Debug.WriteLine("--");
            Debug.WriteLine("");
        }
    }
}
