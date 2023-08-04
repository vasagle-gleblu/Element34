using System;
using System.Collections.Generic;

namespace Element34.StringMetrics.Phonetic
{
    /// <summary>
    /// The Refined Soundex: 
    /// Also known as the Modified Soundex or American Soundex, is an improvement over the original 
    /// Soundex algorithm. It was introduced to address some of the limitations and drawbacks of 
    /// the original Soundex method and to provide better phonetic encoding for names and 
    /// words. Developed by Morten M.Lynge and implemented by Gary Smith in 1985, the Refined 
    /// Soundex algorithm is specifically designed to be more accurate for indexing and searching 
    /// English names.It aims to produce more distinct codes for names that have different pronunciations 
    /// but may be mapped to the same Soundex code in the original algorithm.
    /// </summary>
    public class SoundExRefined : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;

        private IReadOnlyDictionary<char, char> Map = new Dictionary<char, char>()
            {{'B', '1'},{ 'P', '1'},{ 'F', '2'},{ 'V', '2'},{ 'C', '3'},{ 'K', '3'},
            { 'S', '3'},{ 'G', '4'},{ 'J', '4'},{ 'Q', '5'},{ 'X', '5'},{ 'Z', '5'},
            { 'D', '6'},{ 'T', '6'},{ 'L', '7'},{ 'M', '8'},{ 'N', '8'},{ 'R', '9'}};

        /// <summary>
        /// Compares the specified values using Refined SoundEx algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            SoundExRefined sdx = new SoundExRefined();
            value1 = sdx.Encode(value1);
            value2 = sdx.Encode(value2);

            return value1.Equals(value2);
        }

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        /// <summary>
        /// Encodes a string with the Refined SoundEx specification.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The encoded string.</returns>
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
