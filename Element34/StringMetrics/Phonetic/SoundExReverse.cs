using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Collections.Generic;

namespace Element34.StringMetrics.Phonetic
{
    /// <summary>
    /// The Reverse Soundex:
    /// This algorithm is a variation of the Soundex algorithm that works in the opposite 
    /// direction. While the original Soundex algorithm converts words into phonetic codes, 
    /// the Reverse Soundex algorithm attempts to reverse-engineer a word from a given Soundex 
    /// code. In other words, given a Soundex code, the Reverse Soundex algorithm tries to 
    /// generate a list of possible words that could produce that code when processed with 
    /// the regular Soundex algorithm.This can be helpful when trying to find possible original 
    /// words or names that match a given Soundex code, especially in scenarios like genealogical 
    /// research or database searches where similar-sounding names need to be identified.
    /// </summary>
    public class SoundExReverse : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;
        private IReadOnlyDictionary<char, char> Map = new Dictionary<char, char>()
            { { 'B', '1' }, { 'F', '1' }, { 'P', '1' }, { 'V', '1' }, { 'C', '2' },
              { 'G', '2' }, { 'J', '2' }, { 'K', '2' }, { 'Q', '2' }, { 'S', '2' },
              { 'X', '2' }, { 'Z', '2' }, { 'D', '3' }, { 'T', '3' }, { 'L', '4' },
              { 'M', '5' }, { 'N', '5' }, { 'R', '6' } };

        /// <summary>
        /// Compares the specified values using Reverse SoundEx algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            SoundExReverse sdx = new SoundExReverse();
            value1 = sdx.Encode(value1);
            value2 = sdx.Encode(value2);

            return value1.Equals(value2);
        }

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        /// <summary>
        /// Encodes a string with the Reverse SoundEx specification.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The encoded string.</returns>
        public string Encode(string source)
        {
            int i = 0, s = 0, p = 0;
            char c, j;
            char[] token = new char[tokenLength];

            for (int k = 0; k < tokenLength; k++)
                token[k] = '0';

            source = source.ToUpper();

            if (source == null)
            {
                return "";
            }

            while (s < tokenLength)
            {
                if (i < source.Length)
                    c = source[i++];
                else
                    break;

                if (Map.TryGetValue(c, out j))
                {
                    if (j != p)
                    {
                        token[s] = j;
                        p = j;
                        s++;
                    }
                }
                else
                {
                    if (i == 1)
                        s = 1;

                    p = 0;
                }
            }

            token[0] = source[source.Length - 1];
            return new string(token);
        }

    }
}
