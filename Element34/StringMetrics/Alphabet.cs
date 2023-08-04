using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics
{
    public static class Alphabet
    {
        public static readonly char[] LowercaseConsonant = new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        public static readonly char[] UppercaseConsonant = new char[] { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z' };
        public static readonly char[] Consonant = LowercaseConsonant.Concat(UppercaseConsonant).ToArray();
        public static readonly char[] LowercaseVowel = new char[] { 'a', 'e', 'i', 'o', 'u' };
        public static readonly char[] UppercaseVowel = new char[] { 'A', 'E', 'I', 'O', 'U' };
        public static readonly char[] Vowel = LowercaseVowel.Concat(UppercaseVowel).ToArray();
        public static readonly char[] LowercaseY = new char[] { 'y' };
        public static readonly char[] UppercaseY = new char[] { 'Y' };
        public static readonly char[] Y = LowercaseY.Concat(UppercaseY).ToArray();
        public static readonly char[] LowercaseAlpha = LowercaseConsonant.Concat(LowercaseVowel).Concat(LowercaseY).ToArray();
        public static readonly char[] UppercaseAlpha = UppercaseConsonant.Concat(UppercaseVowel).Concat(UppercaseY).ToArray();
        public static readonly char[] Alpha = LowercaseAlpha.Concat(UppercaseAlpha).ToArray();

    public static Dictionary<char, char> Foldings = (new Dictionary<char, char>() {
            { 'à', 'a' }, { 'À', 'A' },
            { 'á', 'a' }, { 'Á', 'A' },
            { 'â', 'a' }, { 'Â', 'A' },
            { 'ã', 'a' }, { 'Ã', 'A' },
            { 'ä', 'a' }, { 'Ä', 'A' },
            { 'å', 'a' }, { 'Å', 'A' },
            { 'æ', 'a' }, { 'Æ', 'A' },
            { 'ç', 'c' }, { 'Ç', 'C' },
            { 'ć', 'c' }, { 'Ć', 'C' },
            { 'è', 'e' }, { 'È', 'E' },
            { 'é', 'e' }, { 'É', 'E' },
            { 'ê', 'e' }, { 'Ê', 'E' },
            { 'ë', 'e' }, { 'Ë', 'E' },
            { 'ì', 'i' }, { 'Ì', 'I' },
            { 'í', 'i' }, { 'Í', 'I' },
            { 'î', 'i' }, { 'Î', 'I' },
            { 'ï', 'i' }, { 'Ï', 'i' },
            { 'ð', 'd' }, { 'Ð', 'D' },
            { 'ñ', 'n' }, { 'Ñ', 'N' },
            { 'ò', 'o' }, { 'Ò', 'O' },
            { 'ó', 'o' }, { 'Ó', 'O' },
            { 'ô', 'o' }, { 'Ô', 'O' },
            { 'õ', 'o' }, { 'Õ', 'O' },
            { 'ö', 'o' }, { 'Ö', 'O' },
            { 'ø', 'o' }, { 'Ø', 'O' },
            { 'ù', 'u' }, { 'Ù', 'U' },
            { 'ú', 'u' }, { 'Ú', 'U' },
            { 'û', 'u' }, { 'Û', 'U' },
            { 'ü', 'u' }, { 'Ü', 'U' },
            { 'ý', 'y' }, { 'Ý', 'Y' },
            { 'ỳ', 'y' }, { 'Ỳ', 'Y' },
            { 'ÿ', 'y' }, { 'Ÿ', 'Y' },
            { 'þ', 'b' }, { 'Þ', 'B' },
            { 'ł', 'l' }, { 'Ł', 'L' },
            { 'ß', 's' }, { 'ẞ', 'S' },
            { 'ś', 's' }, { 'Ś', 'S' },
            { 'ż', 'z' }, { 'Ż', 'Z' },
            { 'ź', 'z' }, { 'Ź', 'Z' }
        });

        public static bool IsSuperset(this char[] set, char c)
        {
            return Array.IndexOf(set, c) != -1;
        }
    }
}
