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
            { 'ź', 'z' }, { 'Ź', 'Z' }});
    }
}
