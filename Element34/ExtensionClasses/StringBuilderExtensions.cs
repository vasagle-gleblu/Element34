using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace System.Text
{
    /// <summary>
    /// Provides a set of static methods for working with <see cref="StringBuilder"/> objects.
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Removes all occurrences of specified characters from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to remove characters from.</param>
        /// <param name="removeChars">The characters to remove.</param>
        /// <returns>The StringBuilder instance after the removal of specified characters.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="removeChars"/> is null.</exception>
        public static StringBuilder Remove(this StringBuilder sb, params char[] removeChars)
        {
            if (removeChars == null)
                throw new ArgumentNullException(nameof(removeChars), "Remove characters array cannot be null.");

            for (int i = 0; i < sb.Length;)
            {
                if (removeChars.Contains(sb[i]))
                    sb.Remove(i, 1);
                else
                    i++;
            }

            return sb;
        }

        /// <summary>
        /// Removes characters from the specified start index to the end of the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to remove characters from.</param>
        /// <param name="startIndex">The zero-based starting index to begin removal.</param>
        /// <returns>The StringBuilder instance after the removal of characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startIndex"/> is less than zero or greater than the length of the StringBuilder.</exception>
        public static StringBuilder Remove(this StringBuilder sb, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index cannot be negative.");
            if (startIndex > sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index cannot be greater than the length of the StringBuilder.");

            return sb.Remove(startIndex, sb.Length - startIndex);
        }

        /// <summary>
        /// Removes all leading occurrences of a set of characters from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to trim.</param>
        /// <param name="trimChars">An array of characters to remove, or null to remove whitespace.</param>
        /// <returns>The trimmed StringBuilder instance.</returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            return sb.TrimHelper(trimChars, 0);
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of characters from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to trim.</param>
        /// <param name="trimChars">An array of characters to remove, or null to remove whitespace.</param>
        /// <returns>The trimmed StringBuilder instance.</returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            return sb.TrimHelper(trimChars, 1);
        }

        /// <summary>
        /// Removes all leading and trailing whitespace characters from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to trim.</param>
        /// <returns>The trimmed StringBuilder instance.</returns>
        public static StringBuilder Trim(this StringBuilder sb)
        {
            return sb.TrimHelper(null, 2);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to trim.</param>
        /// <param name="trimChars">An array of characters to remove, or null to remove whitespace.</param>
        /// <returns>The trimmed StringBuilder instance.</returns>
        public static StringBuilder Trim(this StringBuilder sb, params char[] trimChars)
        {
            return sb.TrimHelper(trimChars, 2);
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="value">The character to locate.</param>
        /// <param name="startIndex">The starting position for the search.</param>
        /// <returns>The zero-based index position of the character if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startIndex"/> is out of range.</exception>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex = 0)
        {
            if (startIndex < 0 || startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");

            for (int i = startIndex; i < sb.Length; i++)
            {
                if (sb[i] == value)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="value">The string to locate.</param>
        /// <param name="startIndex">The starting position for the search.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>The zero-based index position of the string if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startIndex"/> is out of range.</exception>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex = 0, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Search string cannot be null.");
            if (startIndex < 0 || startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var sbString = sb.ToString();
            return sbString.IndexOf(value, startIndex, comparison);
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified character in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="value">The character to locate.</param>
        /// <param name="startIndex">The starting position for the search. If not specified, search starts from the end of the StringBuilder.</param>
        /// <returns>The zero-based index position of the character if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startIndex"/> is out of range.</exception>
        public static int LastIndexOf(this StringBuilder sb, char value, int startIndex = -1)
        {
            startIndex = startIndex == -1 ? sb.Length - 1 : startIndex;

            if (startIndex < 0 || startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");

            for (int i = startIndex; i >= 0; i--)
            {
                if (sb[i] == value)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="value">The string to locate.</param>
        /// <param name="startIndex">The starting position for the search. If not specified, search starts from the end of the StringBuilder.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>The zero-based index position of the string if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="startIndex"/> is out of range.</exception>
        public static int LastIndexOf(this StringBuilder sb, string value, int startIndex = -1, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Search string cannot be null.");
            startIndex = startIndex == -1 ? sb.Length - 1 : startIndex;

            if (startIndex < 0 || startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            var sbString = sb.ToString();
            return sbString.LastIndexOf(value, startIndex, comparison);
        }

        /// <summary>
        /// Determines whether the StringBuilder starts with the specified string.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to compare.</param>
        /// <param name="value">The string to compare.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>true if the string matches the beginning of the StringBuilder; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static bool StartsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Search string cannot be null.");
            if (value.Length > sb.Length)
                return false;

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return sb.ToString().StartsWith(value, comparison);
        }

        /// <summary>
        /// Determines whether the StringBuilder ends with the specified string.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to compare.</param>
        /// <param name="value">The string to compare to the substring at the end of the StringBuilder.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>true if the string matches the end of the StringBuilder; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static bool EndsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Search string cannot be null.");
            if (value.Length > sb.Length)
                return false;

            var comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            return sb.ToString().EndsWith(value, comparison);
        }

        /// <summary>
        /// Converts the StringBuilder to lowercase using the specified culture.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to convert.</param>
        /// <param name="culture">The culture-specific casing rules to use. If null, the current culture is used.</param>
        /// <returns>The StringBuilder converted to lowercase.</returns>
        public static StringBuilder ToLower(this StringBuilder sb, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToLower(sb[i], culture);
            }
            return sb;
        }

        /// <summary>
        /// Converts the StringBuilder to lowercase using the invariant culture.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to convert.</param>
        /// <returns>The StringBuilder converted to lowercase using invariant culture.</returns>
        public static StringBuilder ToLowerInvariant(this StringBuilder sb)
        {
            return sb.ToLower(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the StringBuilder to uppercase using the specified culture.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to convert.</param>
        /// <param name="culture">The culture-specific casing rules to use. If null, the current culture is used.</param>
        /// <returns>The StringBuilder converted to uppercase.</returns>
        public static StringBuilder ToUpper(this StringBuilder sb, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            for (int i = 0; i < sb.Length; i++)
            {
                sb[i] = char.ToUpper(sb[i], culture);
            }
            return sb;
        }

        /// <summary>
        /// Converts the StringBuilder to uppercase using the invariant culture.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to convert.</param>
        /// <returns>The StringBuilder converted to uppercase using invariant culture.</returns>
        public static StringBuilder ToUpperInvariant(this StringBuilder sb)
        {
            return sb.ToUpper(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Retrieves a substring from the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance.</param>
        /// <param name="startIndex">The starting index of the substring.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>A new StringBuilder instance containing the substring.</returns>
        public static StringBuilder Substring(this StringBuilder sb, int startIndex, int length)
        {
            return new StringBuilder(sb.ToString(startIndex, length));
        }

        /// <summary>
        /// Converts the StringBuilder to a character array.
        /// </summary>
        /// <param name="sb">The StringBuilder instance.</param>
        /// <returns>A character array containing the characters of the StringBuilder.</returns>
        public static char[] ToCharArray(this StringBuilder sb)
        {
            return sb.ToString().ToCharArray();
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of a whitespace character in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="startIndex">The starting position for the search.</param>
        /// <returns>The zero-based index position of the first whitespace character if found; otherwise, -1.</returns>
        public static int IndexOfWhitespace(this StringBuilder sb, int startIndex = 0)
        {
            for (int i = startIndex; i < sb.Length; i++)
            {
                if (char.IsWhiteSpace(sb[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Counts the number of occurrences of the specified string in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to search.</param>
        /// <param name="value">The string to locate.</param>
        /// <returns>The number of occurrences of the specified string.</returns>
        public static int CountInstancesOf(this StringBuilder sb, string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            int count = 0;
            int index = 0;

            while ((index = sb.IndexOf(value, index)) != -1)
            {
                count++;
                index += value.Length;
            }

            return count;
        }

        /// <summary>
        /// Replaces the first occurrence of a specified string in the StringBuilder with another string.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="search">The string to search for.</param>
        /// <param name="replace">The string to replace the first occurrence with.</param>
        /// <param name="startIndex">The starting position for the search.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        public static StringBuilder ReplaceFirst(this StringBuilder sb, string search, string replace, int startIndex = 0)
        {
            int index = sb.IndexOf(search, startIndex);

            if (index != -1)
            {
                sb.Remove(index, search.Length);
                sb.Insert(index, replace);
            }

            return sb;
        }

        /// <summary>
        /// Replaces each occurrence of any of the specified characters in the StringBuilder with a specified replacement character.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="search">An array of characters to replace.</param>
        /// <param name="replace">The replacement character.</param>
        /// <param name="startIndex">The starting position for the search.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        public static StringBuilder ReplaceAny(this StringBuilder sb, char[] search, char replace, int startIndex = 0)
        {
            for (int i = startIndex; i < sb.Length; i++)
            {
                if (search.Contains(sb[i]))
                {
                    sb[i] = replace;
                }
            }
            return sb;
        }

        /// <summary>
        /// Prepends the specified elements to the beginning of the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="elements">The elements to prepend.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        public static StringBuilder Prepend(this StringBuilder sb, params string[] elements)
        {
            return sb.InsertChain(0, elements);
        }

        /// <summary>
        /// Removes a specified number of characters from the start of the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="characterCount">The number of characters to remove.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="characterCount"/> is less than zero or greater than the length of the StringBuilder.</exception>
        public static StringBuilder RemoveCharactersAtStart(this StringBuilder sb, int characterCount)
        {
            if (characterCount > sb.Length)
                throw new ArgumentOutOfRangeException(nameof(characterCount), "Character count is greater than the length of the StringBuilder.");
            if (characterCount < 0)
                throw new ArgumentOutOfRangeException(nameof(characterCount), "Character count cannot be less than 0.");

            sb.Remove(0, characterCount);
            return sb;
        }

        /// <summary>
        /// Removes a specified number of characters from the end of the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="characterCount">The number of characters to remove.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="characterCount"/> is less than zero or greater than the length of the StringBuilder.</exception>
        public static StringBuilder RemoveCharactersAtEnd(this StringBuilder sb, int characterCount)
        {
            if (characterCount > sb.Length)
                throw new ArgumentOutOfRangeException(nameof(characterCount), "Character count is greater than the length of the StringBuilder.");
            if (characterCount < 0)
                throw new ArgumentOutOfRangeException(nameof(characterCount), "Character count cannot be less than 0.");

            sb.Remove(sb.Length - characterCount, characterCount);
            return sb;
        }

        /// <summary>
        /// Inserts the specified elements at the specified index in the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="index">The zero-based index at which to insert the elements.</param>
        /// <param name="elements">The elements to insert.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        public static StringBuilder InsertChain(this StringBuilder sb, int index, params string[] elements)
        {
            foreach (var element in elements)
            {
                sb.Insert(index, element);
                index += element.Length;
            }
            return sb;
        }

        /// <summary>
        /// Appends the specified elements to the end of the StringBuilder.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to modify.</param>
        /// <param name="elements">The elements to append.</param>
        /// <returns>The modified StringBuilder instance.</returns>
        public static StringBuilder AppendChain(this StringBuilder sb, params string[] elements)
        {
            return sb.InsertChain(sb.Length, elements);
        }

        /// <summary>
        /// Determines whether the StringBuilder is empty.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to check.</param>
        /// <returns>true if the StringBuilder is empty; otherwise, false.</returns>
        public static bool IsEmpty(this StringBuilder sb)
        {
            return sb.Length == 0;
        }

        /// <summary>
        /// Determines whether the StringBuilder is empty or consists only of whitespace characters.
        /// </summary>
        /// <param name="sb">The StringBuilder instance to check.</param>
        /// <returns>true if the StringBuilder is empty or consists only of whitespace characters; otherwise, false.</returns>
        public static bool IsEmptyOrWhitespace(this StringBuilder sb)
        {
            for (int i = 0; i < sb.Length; i++)
            {
                if (!char.IsWhiteSpace(sb[i]))
                    return false;
            }
            return true;
        }

        private static StringBuilder TrimHelper(this StringBuilder sb, char[] trimChars, int trimType)
        {
            int start = 0;
            int end = sb.Length - 1;

            if (trimType != 1)
            {
                while (start <= end && (trimChars == null ? char.IsWhiteSpace(sb[start]) : trimChars.Contains(sb[start])))
                    start++;
            }

            if (trimType != 0)
            {
                while (end >= start && (trimChars == null ? char.IsWhiteSpace(sb[end]) : trimChars.Contains(sb[end])))
                    end--;
            }

            return sb.CreateTrimmedString(start, end);
        }

        private static StringBuilder CreateTrimmedString(this StringBuilder sb, int start, int end)
        {
            if (start == 0 && end == sb.Length - 1)
                return sb;

            sb.Remove(end + 1, sb.Length - end - 1);
            sb.Remove(0, start);
            return sb;
        }
    }

}
