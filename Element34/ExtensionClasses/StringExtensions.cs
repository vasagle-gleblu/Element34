using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Element34.ExtensionClasses
{
    public static class StringExtensions
    {
        /// <summary>
        /// This method is a static extension method taking a single string parameter and returns a modified 
        /// version of that string with any leading and trailing double quotes removed.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>A string with any leading and trailing double quotes removed</returns>
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

        /// <summary>
        /// The method IsWhiteSpace is a static extension method for the string class in C#. It evaluates whether all characters in 
        /// the provided string are whitespace characters.
        /// </summary>
        /// <param name="str">The input string</param>
        /// <returns>The method returns a boolean value indicating the result.</returns>
        public static bool IsWhiteSpace(this string str)
        {
            bool result = false;

            foreach (char c in str)
                result &= char.IsWhiteSpace(c);

            return result;
        }

        /// <summary>
        /// This method takes a character and an integer repeatCount as parameters.
        /// </summary>
        /// <param name="this">The input character</param>
        /// <param name="repeatCount">Number of repeats</param>
        /// <returns>Returns a new string composed of the input character repeated repeatCount times.</returns>
        public static string Repeat(this char @this, int repeatCount)
        {
            return new string(@this, repeatCount);
        }

        /// <summary>
        /// A string extension method that replace first occurrence.
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
        /// It replaces the first occurrence of a specified substring (oldValue) within the string with another substring (newValue). 
        /// </summary>
        /// <param name="this">The input string.</param>
        /// <param name="number">Number of.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The method returns the modified string, or the original string if the specified substring is not found.</returns>
        public static string ReplaceFirst(this string @this, int number, string oldValue, string newValue)
        {
            List<string> list = Regex.Split(@this, oldValue).ToList();
            int old = number + 1;
            IEnumerable<string> listStart = list.Take(old);
            IEnumerable<string> listEnd = list.Skip(old);

            return string.Join(newValue, listStart) +
                   (listEnd.Any() ? oldValue : "") +
                   string.Join(oldValue, listEnd);
        }

        /// <summary>
        /// Normalizes the provided text by replacing non-breaking space characters with regular spaces and trimming leading and trailing spaces.
        /// </summary>
        /// <param name="text">The text to be normalized. This can include text with non-breaking spaces (encoded as "\u00A0") that need to be replaced with standard space characters.</param>
        /// <returns>The normalized text with non-breaking spaces replaced by regular spaces and any excess whitespace trimmed from both ends.</returns>
        public static string NormalizeText(this string text)
        {
            return text.Replace("\u00A0", " ").Trim();
        }

    }
}
