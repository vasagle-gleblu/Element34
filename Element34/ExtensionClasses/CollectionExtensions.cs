using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34
{
    /// <summary>
    /// Provides extension methods for various collection types.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of items to a collection.
        /// </summary>
        /// <typeparam name="TCol">The type of the collection.</typeparam>
        /// <typeparam name="TItem">The type of the items.</typeparam>
        /// <param name="destination">The collection to add items to.</param>
        /// <param name="source">The items to add to the collection.</param>
        /// <returns>The collection with the added items.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the destination or source is null.</exception>
        public static TCol AddRange<TCol, TItem>(this TCol destination, IEnumerable<TItem> source) where TCol : ICollection<TItem>
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (destination is List<TItem> list)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (var item in source)
                {
                    destination.Add(item);
                }
            }

            return destination;
        }

        /// <summary>
        /// Retrieves the value associated with the specified key or computes it using the given function if not present.
        /// </summary>
        /// <typeparam name="K">The type of the keys.</typeparam>
        /// <typeparam name="V">The type of the values.</typeparam>
        /// <param name="dict">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key whose associated value is to be returned or computed.</param>
        /// <param name="generator">The function to compute the value if not present.</param>
        /// <returns>The value associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dictionary or generator is null.</exception>
        public static V ComputeIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<K, V> generator)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));
            if (generator == null) throw new ArgumentNullException(nameof(generator));

            if (!dict.TryGetValue(key, out var value))
            {
                value = generator(key);
                dict.Add(key, value);
            }

            return value;
        }

        /// <summary>
        /// Retrieves the value associated with the specified key or returns the default value if not present.
        /// </summary>
        /// <typeparam name="K">The type of the keys.</typeparam>
        /// <typeparam name="V">The type of the values.</typeparam>
        /// <param name="dict">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key whose associated value is to be returned.</param>
        /// <returns>The value associated with the specified key or the default value if not present.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dictionary is null.</exception>
        public static V GetValue<K, V>(this IDictionary<K, V> dict, K key)
        {
            return dict.GetValue(key, default);
        }

        /// <summary>
        /// Retrieves the value associated with the specified key or returns the specified default value if not present.
        /// </summary>
        /// <typeparam name="K">The type of the keys.</typeparam>
        /// <typeparam name="V">The type of the values.</typeparam>
        /// <param name="dict">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key whose associated value is to be returned.</param>
        /// <param name="defaultValue">The default value to return if the key is not present.</param>
        /// <returns>The value associated with the specified key or the specified default value if not present.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the dictionary is null.</exception>
        public static V GetValue<K, V>(this IDictionary<K, V> dict, K key, V defaultValue)
        {
            if (dict == null) throw new ArgumentNullException(nameof(dict));

            return dict.TryGetValue(key, out V value) ? value : defaultValue;
        }

        /// <summary>
        /// Enumerates the items in the collection along with their indices.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The collection to enumerate.</param>
        /// <returns>An enumerable of items and their indices.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source collection is null.</exception>
        public static IEnumerable<(T item, int index)> LoopIndex<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.Select((item, index) => (item, index));
        }

        /// <summary>
        /// Appends an item to the end of an array.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="array">The array to append to.</param>
        /// <param name="item">The item to append.</param>
        /// <returns>A new array with the appended item.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the array is null.</exception>
        public static T[] Append<T>(this T[] array, T item)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));

            var list = new List<T>(array) { item };
            return list.ToArray();
        }

        /// <summary>
        /// Populates an array with a default value.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The array to populate.</param>
        /// <param name="defaultValue">The default value to populate.</param>
        /// <returns>The populated array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source array is null.</exception>
        public static T[] Populate<T>(this T[] source, T defaultValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < source.Length; i++)
            {
                source[i] = defaultValue;
            }

            return source;
        }

        /// <summary>
        /// Populates a 2D jagged array with a default value.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The jagged array to populate.</param>
        /// <param name="defaultValue">The default value to populate.</param>
        /// <returns>The populated 2D jagged array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source jagged array is null.</exception>
        public static T[][] Populate<T>(this T[][] source, T defaultValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            for (int i = 0; i < source.Length; i++)
            {
                for (int j = 0; j < source[i].Length; j++)
                {
                    source[i][j] = defaultValue;
                }
            }

            return source;
        }

        /// <summary>
        /// Populates a 2D array with a default value.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="source">The 2D array to populate.</param>
        /// <param name="defaultValue">The default value to populate.</param>
        /// <returns>The populated 2D array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source 2D array is null.</exception>
        public static T[,] Populate<T>(this T[,] source, T defaultValue)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            int rows = source.GetLength(0);
            int cols = source.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    source[i, j] = defaultValue;
                }
            }

            return source;
        }
    }
}
