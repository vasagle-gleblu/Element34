using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetric
{
    public class Alphabet
    {
        private string _name { get; set; }
        private ICollection<char> _chars { get; set; }

        public Alphabet(string name, params char[] chars)
        {
            _name = name;
            _chars = chars;
        }

        public Alphabet(params char[] chars)
        {
            if ((_chars == null))
            {
                throw new ArgumentNullException("Chars cannot be null.");
            }

            _chars = chars;
        }

        public string getName()
        {
            return _name;
        }

        public ICollection<char> getChars()
        {
            return _chars;
        }

        public bool isSuperset(char a)
        {
            return _chars.Contains(a);
        }

        public bool isSuperset(char[] a)
        {
            if ((a == null))
            {
                throw new ArgumentNullException("Array cannot be null.");
            }

            if ((a.Length == 0))
            {
                return false;
            }

            foreach (char c in a)
            {
                if (!_chars.Contains(c))
                {
                    return false;
                }

            }

            return true;
        }

        public bool isSuperset(string a)
        {
            if ((a == null))
            {
                throw new ArgumentNullException("String cannot be null.");
            }

            return isSuperset(a.ToCharArray());
        }
    }

    public class AlphabetSoup
    {
        public static Alphabet LowercaseConsonant = new Alphabet("LowercaseConsonant", new char[] {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z'});
        public static Alphabet UppercaseConsonant = new Alphabet("UppercaseConsonant", new char[] {'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z'});
        public static Alphabet Consonant = new Alphabet(LowercaseConsonant.getChars().AddRange(UppercaseConsonant.getChars()).ToArray());
        public static Alphabet LowercaseVowel = new Alphabet(new char[] { 'a', 'e', 'i', 'o', 'u' });
        public static Alphabet UppercaseVowel = new Alphabet(new char[] { 'A', 'E', 'I', 'O', 'U' });
        public static Alphabet Vowel = new Alphabet(LowercaseVowel.getChars().AddRange((UppercaseVowel.getChars())).ToArray());
        public static Alphabet LowercaseY = new Alphabet(new char[] { 'y' });
        public static Alphabet UppercaseY = new Alphabet(new char[] { 'Y' });
        public static Alphabet Y = new Alphabet(LowercaseY.getChars().AddRange(UppercaseY.getChars()).ToArray());
        public static Alphabet LowercaseAlpha = new Alphabet(LowercaseConsonant.getChars().AddRange(LowercaseVowel.getChars()).AddRange(LowercaseY.getChars()).ToArray());
        public static Alphabet UppercaseAlpha = new Alphabet(UppercaseConsonant.getChars().AddRange(UppercaseVowel.getChars()).AddRange(UppercaseY.getChars()).ToArray());
        public static Alphabet Alpha = new Alphabet(LowercaseAlpha.getChars().AddRange(UppercaseAlpha.getChars()).ToArray());
    }
}
