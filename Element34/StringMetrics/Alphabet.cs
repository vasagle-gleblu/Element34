using System;
using System.Collections.Generic;

namespace Element34.StringMetrics
{
    public class Alphabet
    {
        private string _name { get; set; }
        private object _chars { get; set; }

        public Alphabet(string name, params char[] arr)
        {
            _name = name;
            _chars = arr;
        }

        public Alphabet(string name, Dictionary<char, char> dictionary)
        {
            this._name = name;
            this._chars = dictionary;
        }

        public Alphabet(params char[] arr)
        {
            if ((arr == null))
            {
                throw new ArgumentNullException("Chars cannot be null.");
            }

            _chars = arr;
        }

        public Alphabet(Dictionary<char, char> dictionary)
        {
            if ((dictionary == null))
            {
                throw new ArgumentNullException("Chars cannot be null.");
            }

            _chars = dictionary;
        }

        public string getName()
        {
            return _name;
        }

        public object getChars()
        {
            return _chars;
        }
    }

    public static class AlphabetSoup
    {
        public static Alphabet Consonants = new Alphabet("Consonants", new char[] { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' });
        public static Alphabet Vowels = new Alphabet("Vowels", new char[] { 'a', 'e', 'i', 'o', 'u' });
        public static Alphabet Foldings = new Alphabet("Foldings", new Dictionary<char, char>() {
          { 'ß', 's' }, { 'à', 'a' }, { 'á', 'a' }, { 'â', 'a' }, { 'ã', 'a' },
          { 'ä', 'a' }, { 'å', 'a' }, { 'æ', 'a' }, { 'ç', 'c' }, { 'è', 'e' },
          { 'é', 'e' }, { 'ê', 'e' }, { 'ë', 'e' }, { 'ì', 'i' }, { 'í', 'i' },
          { 'î', 'i' }, { 'ï', 'i' }, { 'ð', 'd' }, { 'ñ', 'n' }, { 'ò', 'o' },
          { 'ó', 'o' }, { 'ô', 'o' }, { 'õ', 'o' }, { 'ö', 'o' }, { 'ø', 'o' },
          { 'ù', 'u' }, { 'ú', 'u' }, { 'û', 'u' }, { 'ý', 'y' }, { 'ỳ', 'y' },
          { 'þ', 'b' }, { 'ÿ', 'y' }, { 'ć', 'c' }, { 'ł', 'l' }, { 'ś', 's' },
          { 'ż', 'z' }, { 'ź', 'z' }});
    }
}
