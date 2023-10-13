using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Element34.ExtensionClasses
{
    public static class StringExtensions
    {
        public static string stripQuotes(this string str)
        {
            if (str.StartsWith("\""))
            {
                str = str.Substring(1);
            }

            if (str.EndsWith("\""))
            {
                str = str.Substring(0, str.Length - 1);
            }

            return str;
        }

        public static bool IsWhiteSpace(this string str)
        {
            bool result = false;

            foreach (char c in str)
                result &= char.IsWhiteSpace(c);

            return result;
        }

        public static string Repeat(this char @this, int repeatCount)
        {
            return new string(@this, repeatCount);
        }

        /// <summary>
        ///     A string extension method that replace first occurrence.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The string with the first occurrence of old value replace by new value.</returns>
        public static string ReplaceFirst(this string @this, string oldValue, string newValue)
        {
            int startindex = @this.IndexOf(oldValue);

            if (startindex == -1)
            {
                return @this;
            }

            return @this.Remove(startindex, oldValue.Length).Insert(startindex, newValue);
        }

        /// <summary>
        ///     A string extension method that replace first number of occurrences.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="number">Number of.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The string with the numbers of occurrences of old value replace by new value.</returns>
        public static string ReplaceFirst(this string @this, int number, string oldValue, string newValue)
        {
            List<string> list = @this.Split(oldValue).ToList();
            int old = number + 1;
            IEnumerable<string> listStart = list.Take(old);
            IEnumerable<string> listEnd = list.Skip(old);

            return string.Join(newValue, listStart) +
                   (listEnd.Any() ? oldValue : "") +
                   string.Join(oldValue, listEnd);
        }

        public static string[] Split(this string @this, string delimiter) 
        {
            return Regex.Split(@this, delimiter);
        }

        public static string[] Split(this string @this, string[] delimiters)
        {
            return Regex.Split(@this, string.Join("|", delimiters));
        }
    }
}
