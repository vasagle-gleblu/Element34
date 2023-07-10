using System;
using System.Collections.Generic;

namespace Element34.StringMetrics
{
    public class SoundExRefined : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;

        private IReadOnlyDictionary<char, char> Map = new Dictionary<char, char>()
            {{'B', '1'},{ 'P', '1'},{ 'F', '2'},{ 'V', '2'},{ 'C', '3'},{ 'K', '3'},
            { 'S', '3'},{ 'G', '4'},{ 'J', '4'},{ 'Q', '5'},{ 'X', '5'},{ 'Z', '5'},
            { 'D', '6'},{ 'T', '6'},{ 'L', '7'},{ 'M', '8'},{ 'N', '8'},{ 'R', '9'}};

        public bool Compare(string value1, string value2)
        {
            SoundExRefined sdx = new SoundExRefined();
            value1 = sdx.Encode(value1);
            value2 = sdx.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            source = source.ToUpper().Trim();

            if (source.Length == 0)
                return string.Empty;

            List<char> token = new List<char>();

            token.Add(source[0]);
            char last = '*', current = '\0';

            for (int i = 0; i < source.Length; i++)
            {
                if (Map.ContainsKey(source[i]))
                    current = Map[source[i]];

                if (current == last)
                {
                    continue;
                }

                if (current != '\0')
                {
                    token.Add(current);
                }

                last = current;
            }

            return new string(token.ToArray());
        }

    }
}
