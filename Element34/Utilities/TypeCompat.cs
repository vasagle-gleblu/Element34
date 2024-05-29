using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element34.Utilities
{
    /// <summary>
    /// Provides utility methods for type introspection and manipulation using reflection.
    /// This class includes methods to check type characteristics, such as whether a type is primitive,
    /// a subclass of another type, or a generic type. It also provides a method to safely retrieve
    /// property values from objects, enhancing error handling and robustness in dealing with
    /// type-related operations.
    /// </summary>

    public static class TypeCompat
    {
        /// <summary>
        /// Determines whether the specified object is of a primitive type.
        /// </summary>
        /// <param name="v">The object to check.</param>
        /// <returns>true if the object is of a primitive type; otherwise, false.</returns>
        public static bool IsPrimitive(object v)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            return v.GetType().IsPrimitive;
        }

        /// <summary>
        /// Determines whether the specified type is a subclass of the specified class.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <param name="c">The class type to compare against.</param>
        /// <returns>true if the specified type is a subclass of the specified class type; otherwise, false.</returns>
        public static bool IsSubclassOf(Type t, Type c)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            if (c == null) throw new ArgumentNullException(nameof(c));
            return t.IsSubclassOf(c);
        }

        /// <summary>
        /// Determines whether the specified type is a generic type.
        /// </summary>
        /// <param name="t">The type to check.</param>
        /// <returns>true if the specified type is a generic type; otherwise, false.</returns>
        public static bool IsGenericType(Type t)
        {
            if (t == null) throw new ArgumentNullException(nameof(t));
            return t.IsGenericType;
        }

        /// <summary>
        /// Gets the value of a property specified by name on the given object.
        /// </summary>
        /// <param name="v">The object from which to get the property value.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        /// <exception cref="ArgumentException">Thrown if the property does not exist or cannot be read.</exception>
        public static object GetPropertyValue(object v, string name)
        {
            if (v == null) throw new ArgumentNullException(nameof(v));
            if (string.IsNullOrEmpty(name)) throw new ArgumentException("Property name cannot be null or empty.", nameof(name));

            var property = v.GetType().GetProperty(name) ?? throw new ArgumentException($"Property '{name}' not found on type {v.GetType().FullName}.");
            if (!property.CanRead)
                throw new ArgumentException($"Property '{name}' is not readable.");

            return property.GetValue(v, null);
        }
    }

}
