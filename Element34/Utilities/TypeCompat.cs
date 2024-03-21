using System;

namespace Element34.Utilities
{
    public class TypeCompat
    {
        public static bool IsPrimitive(object v)
        {
            return v.GetType().IsPrimitive;
        }

        public static bool IsSubclassOf(Type t, Type c)
        {
            return t.IsSubclassOf(c);
        }

        public static bool IsGenericType(Type t)
        {
            return t.IsGenericType;
        }

        public static object GetPropertyValue(object v, string name)
        {
            return v.GetType().GetProperty(name).GetValue(v, null);
        }
    }
}
