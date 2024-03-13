using System;
using System.Linq;

namespace System
{
    public static class MathExtensions
    {
        // Max Method
        public static T Max<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            return values.Max();
        }

        // Min Method
        public static T Min<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            return values.Min();
        }

        // Average Method
        public static double Average<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            return values.Average(x => Convert.ToDouble(x));
        }

        // Sum Method
        public static T Sum<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            dynamic sum = 0;
            foreach (T value in values)
            {
                sum += value;
            }
            return sum;
        }

        // Product Method
        public static T Product<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            dynamic product = 1;
            foreach (T value in values)
            {
                product *= value;
            }
            return product;
        }

        // Absolute Value Method
        public static T Abs<T>(params T[] values) where T : struct, IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.");
            }

            dynamic absValue = Convert.ToDouble(values[0]);
            foreach (T value in values)
            {
                absValue = Max(absValue, Convert.ToDouble(value));
            }
            return absValue < 0 ? (T)(-absValue) : (T)absValue;
        }

    }
}
