using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace System.Text
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Removes all occurrences of specified characters from <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="removeChars">A Unicode characters to remove.</param>
        /// <returns>
        /// Returns <see cref="System.Text.StringBuilder"/> without specified Unicode characters.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="removeChars"/> is null.</exception>
        public static StringBuilder Remove(this StringBuilder sb, params char[] removeChars)
        {
            if (removeChars == null)
                throw new ArgumentNullException($"{nameof(removeChars)} cannot be null.");

            for (int i = 0; i < sb.Length;)
            {
                if (removeChars.Any(ch => ch == sb[i]))
                    sb.Remove(i, 1);
                else
                    i++;
            }
            return sb;
        }

        /// <summary>
        /// Removes the range of characters from the specified index to the end of <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="startIndex">The zero-based position to begin deleting characters.</param>
        /// <returns>A reference to this instance after the excise operation has completed.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// If <paramref name="startIndex"/> is less than zero, or <paramref name="startIndex"/> is greater
        /// than the length - 1 of this instance.
        /// </exception>
        public static StringBuilder Remove(this StringBuilder sb, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex > sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than {nameof(sb)} length.");

            StringBuilder result = sb.Remove(startIndex, sb.Length - startIndex);

            return result;
        }

        private static bool IsBOMWhitespace(char c)
        {
#if FEATURE_LEGACYNETCF
            if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8 && c == '\xFEFF')
            {
                // Dev11 450846 quirk:
                // NetCF treats the BOM as a whitespace character when performing trim operations.
                return true;
            }
            else
#endif
            {
                return false;
            }
        }

        private static StringBuilder TrimHelper(this StringBuilder sb, int trimType)
        {
            int end = sb.Length - 1;
            int start = 0;
            if (trimType != 1)
            {
                start = 0;
                while (start < sb.Length)
                {
                    if (!char.IsWhiteSpace(sb[start]) && !IsBOMWhitespace(sb[start]))
                    {
                        break;
                    }
                    start++;
                }
            }
            if (trimType != 0)
            {
                end = sb.Length - 1;
                while (end >= start)
                {
                    if (!char.IsWhiteSpace(sb[end]) && !IsBOMWhitespace(sb[start]))
                    {
                        break;
                    }
                    end--;
                }
            }
            return sb.CreateTrimmedString(start, end);
        }

        private static StringBuilder CreateTrimmedString(this StringBuilder sb, int start, int end)
        {
            int length = (end - start) + 1;
            if (length == sb.Length)
            {
                return sb;
            }
            if (length == 0)
            {
                sb.Length = 0;
                return sb;
            }
            return sb.Substring(start, end);
        }

        private static StringBuilder TrimHelper(this StringBuilder sb, char[] trimChars, int trimType)
        {
            int end = sb.Length - 1;
            int start = 0;
            if (trimType != 1)
            {
                start = 0;
                while (start < sb.Length)
                {
                    int index = 0;
                    char ch = sb[start];
                    while (index < trimChars.Length)
                    {
                        if (trimChars[index] == ch)
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == trimChars.Length)
                    {
                        break;
                    }
                    start++;
                }
            }
            if (trimType != 0)
            {
                end = sb.Length - 1;
                while (end >= start)
                {
                    int num4 = 0;
                    char ch2 = sb[end];
                    while (num4 < trimChars.Length)
                    {
                        if (trimChars[num4] == ch2)
                        {
                            break;
                        }
                        num4++;
                    }
                    if (num4 == trimChars.Length)
                    {
                        break;
                    }
                    end--;
                }
            }
            return sb.CreateTrimmedString(start, end);
        }

        /// <summary>
        /// Removes all leading occurrences of a set of characters specified in an array 
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters 
        /// that remains after all occurrences of the characters in the <paramref name="trimChars"/> parameter 
        /// are removed from the end of the current string. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder TrimStart(this StringBuilder sb, params char[] trimChars)
        {
            StringBuilder result;

            if ((trimChars != null) && (trimChars.Length != 0))
                result = sb.TrimHelper(trimChars, 0);
            else
                result = sb.TrimHelper(0);

            return result;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of characters specified in an array 
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters that remains 
        /// after all occurrences of the characters in the <paramref name="trimChars"/> parameter are removed 
        /// from the end of the current string. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, params char[] trimChars)
        {
            StringBuilder result;

            if ((trimChars != null) && (trimChars.Length != 0))
                result = sb.TrimHelper(trimChars, 1);
            else
                result = sb.TrimHelper(1);

            return result;
        }

        /// <summary>
        /// Removes all leading and trailing white-space characters from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to remove from.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters 
        /// that remains after all white-space characters are removed 
        /// from the start and end of the current StringBuilder.
        /// </returns>
        public static StringBuilder Trim(this StringBuilder sb)
        {
            StringBuilder result = sb.TrimHelper(2);

            return result;
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters specified in an array
        /// from the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="trimChars">An array of Unicode characters to remove, or null.</param>
        /// <returns>
        /// The <see cref="System.Text.StringBuilder"/> object that contains a list of characters that remains 
        /// after all occurrences of the characters in the <paramref name="trimChars"/> parameter are removed 
        /// from the end of the current StringBuilder. If <paramref name="trimChars"/> is null or an empty array, 
        /// Unicode white-space characters are removed instead.
        /// </returns>
        public static StringBuilder Trim(this StringBuilder sb, params char[] trimChars)
        {
            StringBuilder result;

            if ((trimChars != null) && (trimChars.Length != 0))
                result = sb.TrimHelper(trimChars, 2);
            else
                result = sb.TrimHelper(2);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index position of the first occurrence of the specified Unicode
        /// character within this instance.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        public static int IndexOf(this StringBuilder sb, char value)
        {
            int result = IndexOf(sb, value, 0, sb.Length);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index position of the first occurrence of the specified Unicode
        /// character within this instance. The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, and <paramref name="startIndex"/> 
        /// is less than 0 (zero) or greater than the length of the <see cref="System.Text.StringBuilder"/>.
        /// </exception>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex > sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than {nameof(sb)} length.");

            int result = sb.IndexOf(value, startIndex, sb.Length - startIndex);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified Unicode
        /// character in this <see cref="System.Text.StringBuilder"/>. The search starts 
        /// at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, and <paramref name="count"/> 
        /// or <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is greater than 
        /// the length of this <see cref="System.Text.StringBuilder"/>.
        /// -or-The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, and <paramref name="count"/> 
        /// is greater than the length of this string minus <paramref name="startIndex"/>. 
        /// </exception>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex, int count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");

            if (startIndex + count > sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} + {nameof(count)} cannot be larger than {nameof(sb)} length.");

            int result = -1;

            if (sb.Length == 0 || count == 0)
                return result;

            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (sb[i] == value)
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the <paramref name="value"/> parameter if that string is found, 
        /// or -1 if it is not. If <paramref name="value"/> is <see cref="System.String.Empty"/>, the return value 
        /// is <paramref name="startIndex"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static int IndexOf(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            int result = 0;

            if (value == string.Empty)
                return result;

            result = IndexOfInternal(sb, value, 0, sb.Length, ignoreCase);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// Parameter specifies the starting search position in the current <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the <paramref name="value"/> parameter if that string is found, 
        /// or -1 if it is not. If <paramref name="value"/> is <see cref="System.String.Empty"/>, the return value 
        /// is <paramref name="startIndex"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than 0 (zero) or greater than the length of this <see cref="System.Text.StringBuilder"/>.
        /// </exception>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            int result = IndexOfInternal(sb, value, startIndex, sb.Length - startIndex, ignoreCase);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the <paramref name="value"/> parameter if that string is found, 
        /// or -1 if it is not. If <paramref name="value"/> is <see cref="System.String.Empty"/>, 
        /// the return value is <paramref name="startIndex"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is 
        /// greater than the length of this instance.-or-<paramref name="count"/> is greater than the length of 
        /// this <see cref="System.Text.StringBuilder"/> minus <paramref name="startIndex"/>.
        /// </exception>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex, int count, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");

            if (startIndex + count > sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} + {nameof(count)} cannot be larger than {nameof(sb)} length.");

            int result = IndexOfInternal(sb, value, startIndex, count, ignoreCase);

            return result;
        }

        private static int IndexOfInternal(StringBuilder sb, string value, int startIndex, int count, bool ignoreCase)
        {
            if (value == string.Empty)
                return startIndex;

            if (sb.Length == 0 || count == 0 || startIndex + 1 + value.Length > sb.Length)
                return -1;

            int num3;
            int length = value.Length;
            int num2 = startIndex + count - value.Length;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i <= num2; i++)
                {
                    if (sb[i] == value[0])
                    {
                        num3 = 1;
                        while ((num3 < length) && (sb[i + num3] == value[num3]))
                        {
                            num3++;
                        }
                        if (num3 == length)
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                for (int j = startIndex; j <= num2; j++)
                {
                    if (char.ToLower(sb[j]) == char.ToLower(value[0]))
                    {
                        num3 = 1;
                        while ((num3 < length) && (char.ToLower(sb[j + num3]) == char.ToLower(value[num3])))
                        {
                            num3++;
                        }
                        if (num3 == length)
                        {
                            return j;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance 
        /// of any character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">anyOf is null.</exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            int result = sb.IndexOfAny(anyOf, 0, sb.Length);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is negative.-or-<paramref name="startIndex"/> is greater 
        /// than the number of characters in this instance.
        /// </exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            int result = sb.IndexOfAny(anyOf, startIndex, sb.Length - startIndex);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of the first occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
        /// -or-<paramref name="count"/> + <paramref name="startIndex"/> is greater than the number of characters in this instance.
        /// </exception>
        public static int IndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");

            if (startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} - {nameof(count)} + 1 cannot be negative.");

            int result = -1;

            if (sb.Length == 0 || count == 0)
                return result;

            for (int i = startIndex; i < startIndex + count; i++)
            {
                if (anyOf.Any(ch => ch == sb[i]))
                {
                    result = i;
                }
            }

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified Unicode
        /// character within this instance.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        public static int LastIndexOf(this StringBuilder sb, char value)
        {
            int result = LastIndexOf(sb, value, sb.Length - 1, sb.Length);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index position of the last occurrence of the specified Unicode character 
        /// in a substring within this instance. The search starts at a specified character position and 
        /// proceeds backward toward the beginning of the <see cref="System.Text.StringBuilder"/> 
        /// for a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">
        /// The starting position of the search. The search proceeds from <paramref name="startIndex"/> toward the beginning 
        /// of this instance.
        /// </param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, 
        /// and <paramref name="startIndex"/> is less than zero or greater than or equal to the length of this instance.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, char value, int startIndex)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            int result = sb.LastIndexOf(value, startIndex, startIndex + 1);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified Unicode
        /// character in this <see cref="System.Text.StringBuilder"/>. The search starts 
        /// at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">A Unicode character to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that character is found, or -1 
        /// if it is not.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The current instance <see cref="System.Text.StringBuilder.Length"/> does not equal 0, 
        /// and <paramref name="startIndex"/> is less than zero or greater than or equal to the length of this instance.
        /// -or-The current instance <see cref="System.Text.StringBuilder.Length"/> 
        /// does not equal 0, and <paramref name="startIndex"/> - <paramref name="count"/> + 1 is less than zero.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, char value, int startIndex, int count)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");

            if (startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} - {nameof(count)} + 1 cannot be negative.");

            int result = -1;

            if (sb.Length == 0 || count == 0)
                return result;

            for (int i = startIndex; i > startIndex - count; i--)
            {
                if (sb[i] == value)
                {
                    result = i;
                }
            }
            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string 
        /// in the current <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to </param>
        /// <param name="value">The string to seek.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        public static int LastIndexOf(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            int result = -1;

            if (value == string.Empty)
            {
                if (sb.Length == 0)
                    return 0;
                else
                    return sb.Length - 1;
            }

            if (sb.Length == 0)
                return result;

            result = LastIndexOfInternal(sb, value, sb.Length - 1, sb.Length, ignoreCase);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// Parameter specifies the starting search position in the current <see cref="System.Text.StringBuilder"/>.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek. </param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        public static int LastIndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            int result = LastIndexOfInternal(sb, value, startIndex, startIndex + 1, ignoreCase);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence of the specified string in the current <see cref="System.Text.StringBuilder"/> object. 
        /// The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="value">The string to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// The zero-based index position of the value parameter if that string is found, 
        /// or -1 if it is not. If value is <see cref="System.String.Empty"/>, the return value is startIndex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">value is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startIndex is less than 0 (zero) or greater than the length of this string.
        /// </exception>
        public static int LastIndexOf(this StringBuilder sb, string value, int startIndex, int count, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative."); ;

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be larger than or equal to {nameof(sb)} length.");

            if (startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} - {nameof(count)} + 1 cannot be negative.");

            int result = LastIndexOfInternal(sb, value, startIndex, count, ignoreCase);

            return result;
        }

        private static int LastIndexOfInternal(StringBuilder sb, string value, int startIndex, int count, bool ignoreCase)
        {
            if (value == string.Empty)
                return startIndex;
            if (sb.Length == 0 || count == 0 || startIndex + 1 - count + value.Length > sb.Length)
                return -1;

            int num3;
            int length = value.Length;
            int maxValueIndex = length - 1;
            int num2 = startIndex - count + value.Length;
            if (ignoreCase == false)
            {
                for (int i = startIndex; i >= num2; i--)
                {
                    if (sb[i] == value[maxValueIndex])
                    {
                        num3 = 1;
                        while ((num3 < length) && (sb[i - num3] == value[maxValueIndex - num3]))
                        {
                            num3++;
                        }
                        if (num3 == length)
                        {
                            return i - num3 + 1;
                        }
                    }
                }
            }
            else
            {
                for (int j = startIndex; j >= num2; j--)
                {
                    if (char.ToLower(sb[j]) == char.ToLower(value[maxValueIndex]))
                    {
                        num3 = 1;
                        while ((num3 < length) && (char.ToLower(sb[j - num3]) == char.ToLower(value[maxValueIndex - num3])))
                        {
                            num3++;
                        }
                        if (num3 == length)
                        {
                            return j - num3 + 1;
                        }
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance 
        /// of any character in a specified array of Unicode characters.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            int result = sb.LastIndexOfAny(anyOf, sb.Length - 1, sb.Length);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is negative.-or- <paramref name="startIndex"/> is greater 
        /// than the number of characters in this instance.
        /// </exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be greater than or equal to {nameof(sb)} length.");

            int result = sb.LastIndexOfAny(anyOf, startIndex, startIndex + 1);

            return result;
        }

        /// <summary>
        /// Reports the zero-based index of the last occurrence in this instance of any character 
        /// in a specified array of Unicode characters. The search starts at a specified character position and examines a specified number of character positions.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to search.</param>
        /// <param name="anyOf">A Unicode character array containing one or more characters to seek.</param>
        /// <param name="startIndex">The search starting position.</param>
        /// <param name="count">The number of character positions to examine.</param>
        /// <returns>
        /// The zero-based index position of the last occurrence in this instance
        /// where any character in <paramref name="anyOf"/> was found; -1 if no character in <paramref name="anyOf"/> was found.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="anyOf"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> or <paramref name="startIndex"/> is negative.
        /// -or-<paramref name="count"/> + <paramref name="startIndex"/> is greater than the number of characters in this instance.
        /// </exception>
        public static int LastIndexOfAny(this StringBuilder sb, char[] anyOf, int startIndex, int count)
        {
            if (anyOf == null)
                throw new ArgumentNullException($"{nameof(anyOf)} cannot be null.");

            if (startIndex < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be negative.");

            if (startIndex >= sb.Length)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} cannot be greater than or equal to {nameof(sb)} length.");

            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative.");

            if (startIndex - count + 1 < 0)
                throw new ArgumentOutOfRangeException($"{nameof(startIndex)} - {nameof(count)} + 1 cannot be negative.");

            int result = -1;

            if (sb.Length == 0 || count == 0)
                return result;

            for (int i = startIndex; i > startIndex - count; i--)
            {
                if (anyOf.Any(ch => ch == sb[i]))
                {
                    result = i;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether this instance of <see cref="System.Text.StringBuilder"/> starts with the specified string.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to compare.</param>
        /// <param name="value">The string to compare.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// true if the <paramref name="value"/> parameter matches the beginning of this string; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static bool StartsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            int length = value.Length;
            bool result = true;
            if (length > sb.Length)
                return false;

            if (ignoreCase == false)
            {
                for (int i = 0; i < length; i++)
                {
                    if (sb[i] != value[i])
                    {
                        result = false;
                    }
                }
            }
            else
            {
                for (int j = 0; j < length; j++)
                {
                    if (char.ToLower(sb[j]) != char.ToLower(value[j]))
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        public static bool StartsWith(this StringBuilder sb, char value, bool ignoreCase = false)
        {
            Contract.Ensures(!Contract.Result<bool>() || sb.Length > 0);

            if (ignoreCase == false)
            {
                return (sb[0] == value);
            }
            else
            {
                return (char.ToLower(sb[0]) == char.ToLower(value));
            }

        }

        /// <summary>
        /// Determines whether this instance of <see cref="System.Text.StringBuilder"/> ends with the specified string.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to compare.</param>
        /// <param name="value">The string to compare to the substring at the end of this instance.</param>
        /// <param name="ignoreCase">true to ignore case during the comparison; otherwise, false.</param>
        /// <returns>
        /// true if the <paramref name="value"/> parameter matches the beginning of this string; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="value"/> is null.</exception>
        public static bool EndsWith(this StringBuilder sb, string value, bool ignoreCase = false)
        {
            if (value == null)
                throw new ArgumentNullException($"{nameof(value)} cannot be null.");

            int length = value.Length;
            int maxSBIndex = sb.Length - 1;
            int maxValueIndex = length - 1;
            bool result = true;

            if (length > sb.Length)
                return false;

            if (ignoreCase == false)
            {
                for (int i = 0; i < length; i++)
                {
                    if (sb[maxSBIndex - i] != value[maxValueIndex - i])
                    {
                        result = false;
                    }
                }
            }
            else
            {
                for (int j = length - 1; j >= 0; j--)
                {
                    if (char.ToLower(sb[maxSBIndex - j]) != char.ToLower(value[maxValueIndex - j]))
                    {
                        result = false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase.</returns>
        public static StringBuilder ToLower(this StringBuilder sb)
        {
            StringBuilder result = new StringBuilder(sb.Length);

            for (int i = 0; i < sb.Length; i++)
            {
                result[i] = char.ToLower(sb[i]);
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase using specified culture.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="culture"/>is null.</exception>
        public static StringBuilder ToLower(this StringBuilder sb, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException($"{nameof(culture)} cannot be null.");

            StringBuilder result = new StringBuilder(sb.Length);

            for (int i = 0; i < sb.Length; i++)
            {
                result[i] = char.ToLower(sb[i], culture);
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to lowercase, using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to lowercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to lowercase using invariant culture.</returns>
        public static StringBuilder ToLowerInvariant(this StringBuilder sb)
        {
            StringBuilder result = sb.ToLower(CultureInfo.InvariantCulture);

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase.</returns>
        public static StringBuilder ToUpper(this StringBuilder sb)
        {
            StringBuilder result = new StringBuilder(sb.Length);

            for (int i = 0; i < sb.Length; i++)
            {
                result[i] = char.ToUpper(sb[i]);
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <param name="culture">An object that supplies culture-specific casing rules.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase using specified culture.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="culture"/>is null.</exception>
        public static StringBuilder ToUpper(this StringBuilder sb, CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException($"{nameof(culture)} cannot be null.");

            StringBuilder result = new StringBuilder(sb.Length);

            for (int i = 0; i < sb.Length; i++)
            {
                result[i] = char.ToUpper(sb[i], culture);
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.Text.StringBuilder"/> converted to uppercase, using the casing rules of the invariant culture.
        /// </summary>
        /// <param name="sb">A <see cref="System.Text.StringBuilder"/> to convert to uppercase.</param>
        /// <returns>The <see cref="System.Text.StringBuilder"/> converted to uppercase using invariant culture.</returns>
        public static StringBuilder ToUpperInvariant(this StringBuilder sb)
        {
            StringBuilder result = sb.ToUpper(CultureInfo.InvariantCulture);

            return result;
        }

        public static StringBuilder Substring(this StringBuilder sbInput, int iIndex, int iLength)
        {
            return new StringBuilder(sbInput.ToString(iIndex, iLength));
        }

        public static char[] ToCharArray(this StringBuilder stringBuilder)
        {
            return stringBuilder.ToString().ToCharArray();
        }

        public static int IndexOfWhitespace(this StringBuilder builder, int startIndex = 0)
        {
            for (int i = startIndex; i < builder.Length; ++i)
            {
                if (Char.IsWhiteSpace(builder[i]))
                    return i;
            }

            return -1;
        }

        public static int CountInstancesOf(this StringBuilder builder, string find)
        {
            int count = 0;

            int nextIndex = 0;
            while (nextIndex > -1)
            {
                nextIndex = builder.IndexOf(find, nextIndex + find.Length);

                if (nextIndex > -1)
                    count++;
            }

            return count;
        }

        public static StringBuilder ReplaceFirst(this StringBuilder builder, string search, string replace, int startIndex = 0)
        {
            return builder.ReplaceFirst(search, replace, out _, startIndex);
        }

        public static StringBuilder ReplaceFirst(this StringBuilder builder, string search, string replace, out int indexOfReplacementStart, int startIndex = 0)
        {
            indexOfReplacementStart = builder.IndexOf(search, startIndex);

            if (indexOfReplacementStart < 0)
                return builder;

            builder.Remove(indexOfReplacementStart, search.Length);
            builder.Insert(indexOfReplacementStart, replace);

            return builder;
        }

        /// <summary>
        /// Replace any characters from a list with a given character.
        /// </summary>
        public static StringBuilder ReplaceAny(this StringBuilder builder, char[] search, char replace, int startIndex = 0)
        {
            for (int i = startIndex; i < builder.Length; i++)
            {
                foreach (var candidate in search)
                {
                    if (builder[i] == candidate)
                    {
                        builder[i] = replace;
                        break;
                    }
                }
            }

            return builder;
        }

        public static StringBuilder Prepend(this StringBuilder builder, IEnumerable<string> elements)
        {
            return builder.InsertChain(0, elements);
        }

        public static StringBuilder Prepend(this StringBuilder builder, out int insertionEndIndex, IEnumerable<string> elements)
        {
            return builder.InsertChain(0, out insertionEndIndex, elements);
        }

        public static StringBuilder Prepend(this StringBuilder builder, params string[] elements)
        {
            return builder.InsertChain(0, elements);
        }

        public static StringBuilder Prepend(this StringBuilder builder, out int insertionEndIndex, params string[] elements)
        {
            return builder.InsertChain(0, out insertionEndIndex, elements);
        }

        public static StringBuilder RemoveCharactersAtStart(this StringBuilder builder, int characterCount)
        {
            if (characterCount > builder.Length)
                throw new ArgumentException($"{nameof(characterCount)} is greater than the length of the builder");

            if (characterCount < 0)
                throw new ArgumentException($"{nameof(characterCount)} cannot be less than 0");


            builder.Remove(startIndex: 0, length: characterCount);
            return builder;
        }

        public static StringBuilder RemoveCharactersAtEnd(this StringBuilder builder, int characterCount)
        {
            if (characterCount > builder.Length)
                throw new ArgumentException($"{nameof(characterCount)} is greater than the length of the builder");

            if (characterCount < 0)
                throw new ArgumentException($"{nameof(characterCount)} cannot be less than 0");

            builder.Remove(startIndex: builder.Length - characterCount, length: characterCount);
            return builder;
        }

        public static StringBuilder InsertChain(this StringBuilder builder, int index, IEnumerable<string> elements)
        {
            return InsertChain(builder, index, out var _, elements);
        }

        public static StringBuilder InsertChain(this StringBuilder builder, int index, params string[] elements)
        {
            return builder.InsertChain(index, (IEnumerable<string>)elements);
        }

        public static StringBuilder InsertChain(this StringBuilder builder, int index, out int insertionEndIndex, params string[] elements)
        {
            return builder.InsertChain(index, out insertionEndIndex, (IEnumerable<string>)elements);
        }

        public static StringBuilder InsertChain(this StringBuilder builder, int index, out int insertionEndIndex, IEnumerable<string> elements)
        {
            foreach (var element in elements)
            {
                builder.Insert(index, element);
                index += element.Length;
            }

            insertionEndIndex = index;
            return builder;
        }

        public static StringBuilder AppendChain(this StringBuilder builder, IEnumerable<string> elements)
        {
            return builder.InsertChain(builder.Length, elements);
        }

        public static StringBuilder AppendChain(this StringBuilder builder, params string[] elements)
        {
            return builder.InsertChain(builder.Length, elements);
        }

        public static bool IsEmpty(this StringBuilder builder)
        {
            return builder.Length == 0;
        }

        public static bool IsEmptyOrWhitespace(this StringBuilder builder)
        {
            if (builder.IsEmpty())
                return true;

            for (int i = 0; i < builder.Length; i++)
            {
                if (!Char.IsWhiteSpace(builder[i]))
                    return false;
            }

            return true;
        }

    }

}
