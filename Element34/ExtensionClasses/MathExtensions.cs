using System;
using System.Linq;

namespace System
{
    /// <summary>
    /// Provides extension methods for mathematical operations.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Returns the maximum value in a sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to determine the maximum value.</param>
        /// <returns>The maximum value in the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static T Max<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            return values.Max();
        }

        /// <summary>
        /// Returns the minimum value in a sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to determine the minimum value.</param>
        /// <returns>The minimum value in the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static T Min<T>(params T[] values) where T : IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            return values.Min();
        }

        /// <summary>
        /// Computes the average of a sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to compute the average.</param>
        /// <returns>The average value of the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static double Average<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            return values.Average(x => Convert.ToDouble(x));
        }

        /// <summary>
        /// Computes the sum of a sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to compute the sum.</param>
        /// <returns>The sum of the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static T Sum<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            dynamic sum = default(T);
            foreach (T value in values)
            {
                sum += value;
            }
            return sum;
        }

        /// <summary>
        /// Computes the product of a sequence of values.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to compute the product.</param>
        /// <returns>The product of the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static T Product<T>(params T[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            dynamic product = (T)Convert.ChangeType(1, typeof(T));
            foreach (T value in values)
            {
                product *= value;
            }
            return product;
        }

        /// <summary>
        /// Computes the absolute value of each value in a sequence.
        /// </summary>
        /// <typeparam name="T">The type of the values.</typeparam>
        /// <param name="values">An array of values to compute the absolute values.</param>
        /// <returns>The absolute value of each value in the sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when no values are provided.</exception>
        public static T Abs<T>(params T[] values) where T : struct, IComparable<T>
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException("At least one value must be provided.", nameof(values));
            }

            dynamic absValue = Convert.ToDouble(values[0]);
            foreach (T value in values)
            {
                absValue = Math.Max(absValue, Convert.ToDouble(value));
            }
            return absValue < 0 ? (T)(-absValue) : (T)absValue;
        }
    }
}
