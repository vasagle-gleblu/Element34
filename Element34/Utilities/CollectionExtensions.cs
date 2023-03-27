using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static V GetValue<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }

        // To get the index of the current item just write an extension method using LINQ and Tuples.
        public static IEnumerable<(T item, int index)> LoopIndex<T>(this IEnumerable<T> source)
        {
            return source.Select((item, index) => (item, index));
        }
    }
}
