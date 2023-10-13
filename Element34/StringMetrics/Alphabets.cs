using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics
{
    public class Alphabet
    {
        public HashSet<char> Chars { get; }

        public Alphabet(IEnumerable<char> chars)
        {
            Chars = new HashSet<char>(chars);
        }
    }

    public static class Alphabets
    {
        public static Alphabet LowercaseConsonant { get; } = new Alphabet("bcdfghjklmnpqrstvwxyz");
        public static Alphabet UppercaseConsonant { get; } = new Alphabet("BCDFGHJKLMNPQRSTVWXYZ");
        public static Alphabet Consonant { get; } = new Alphabet(LowercaseConsonant.Chars.Concat(UppercaseConsonant.Chars));
        public static Alphabet LowercaseVowel { get; } = new Alphabet("aeiou");
        public static Alphabet UppercaseVowel { get; } = new Alphabet("AEIOU");
        public static Alphabet Vowel { get; } = new Alphabet(LowercaseVowel.Chars.Concat(UppercaseVowel.Chars));
        public static Alphabet LowercaseY { get; } = new Alphabet("y");
        public static Alphabet UppercaseY { get; } = new Alphabet("Y");
        public static Alphabet Y { get; } = new Alphabet(LowercaseY.Chars.Concat(UppercaseY.Chars));
        public static Alphabet LowercaseAlpha { get; } = new Alphabet(Consonant.Chars.Concat(Vowel.Chars).Concat(LowercaseY.Chars));
        public static Alphabet UppercaseAlpha { get; } = new Alphabet(Consonant.Chars.Concat(Vowel.Chars).Concat(UppercaseY.Chars));
        public static Alphabet Alpha { get; } = new Alphabet(LowercaseAlpha.Chars.Concat(UppercaseAlpha.Chars));
        public static Alphabet UppercaseDipthongH { get; } = new Alphabet("CGPST");
        public static Alphabet LowercaseDipthongH { get; } = new Alphabet("cgpst");

        internal static bool IsContained(this Alphabet set, char c)
        {
            return Array.IndexOf(set.Chars.ToArray(), c) > -1;
        }

        internal static bool IsSuperset(this Alphabet set, char c)
        {
            return Array.IndexOf(set.Chars.ToArray(), c) != -1;
        }
    }
}
