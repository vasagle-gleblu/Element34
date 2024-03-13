using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34
{
    public static class CollectionExtensions
    {
        public static TCol AddRange<TCol, TItem>(this TCol destination, IEnumerable<TItem> source) where TCol : ICollection<TItem>
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (source == null) throw new ArgumentNullException(nameof(source));

            // don't cast to IList to prevent recursion
            if (destination is List<TItem> list)
            {
                list.AddRange(source);
                return destination;
            }

            foreach (var item in source)
            {
                destination.Add(item);
            }

            return destination;
        }

        public static V ComputeIfAbsent<K, V>(this Dictionary<K, V> dict, K key, Func<K, V> generator)
        {
            bool exists = dict.TryGetValue(key, out var value);
            if (exists)
            {
                return value;
            }
            var generated = generator(key);
            dict.Add(key, generated);
            return generated;
        }

        // Extension methods to add Python-style get() to C#
        public static V GetValue<K, V>(this IDictionary<K, V> dict, K key)
        {
            return dict.GetValue(key, default(V));
        }

        public static V GetValue<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        // To get the index of the current item just write an extension method using LINQ and Value Tuples.
        public static IEnumerable<(T item, int index)> LoopIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }

        // "Append" an element to an array.
        public static T[] Append<T>(this T[] array, T item)
        {
            List<T> list = new List<T>(array);
            list.Add(item);

            return list.ToArray();
        }

        // Populate array with default value.
        public static T[] Populate<T>(this T[] source, int size, T defaultValue)
        {
            source = new T[size];
            source = Enumerable.Repeat(defaultValue, source.Length).ToArray();
            return source;
        }


        // Populate 2D jagged array with default value.
        public static T[][] Populate<T>(this T[][] source, int rows, int cols, T defaultValue)
        {
            source = Enumerable.Repeat(Enumerable.Repeat(defaultValue, cols).ToArray(), rows).ToArray();
            return source;
        }

        // Populate 2D array with default value
        public static T[,] Populate<T>(this T[,] source, int rows, int cols, T defaultValue)
        {
            source = new T[rows, cols];

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
