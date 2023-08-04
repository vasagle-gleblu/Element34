using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Text;
using static OfficeOpenXml.ExcelErrorValue;

namespace Element34.StringMetrics.Phonetic
{
    /// <summary>
    /// Soundex:
    /// The Soundex algorithm is a phonetic algorithm used for indexing and searching words 
    /// by their pronunciation, specifically their phonetic similarity. It was developed to help 
    /// improve the accuracy of matching names and words that may have different spellings but sound 
    /// similar when pronounced. It was originally created by Robert C.Russell and Margaret King Odell 
    /// in the late 1910s and later refined by Robert C.Russell in 1922. It was initially intended for 
    /// improving the accuracy of surname indexing in the U.S. Census, but it has since found applications 
    /// in various fields, particularly in genealogy and database searching.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/Soundex" />
    /// </summary>
    public class SoundEx : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;
        private readonly IDictionary<char, char> Map = new Dictionary<char, char>()
            { { 'B', '1' }, { 'F', '1' }, { 'P', '1' }, { 'V', '1' }, { 'C', '2' },
              { 'G', '2' }, { 'J', '2' }, { 'K', '2' }, { 'Q', '2' }, { 'S', '2' },
              { 'X', '2' }, { 'Z', '2' }, { 'D', '3' }, { 'T', '3' }, { 'L', '4' },
              { 'M', '5' }, { 'N', '5' }, { 'R', '6' } };

        /// <summary>
        /// Compares the specified values using SoundEx algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            SoundEx sdx = new SoundEx();
            value1 = sdx.Encode(value1);
            value2 = sdx.Encode(value2);

            return value1.Equals(value2);
        }

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        /// <summary>
        /// Encodes a string with the SoundEx specification.
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

            token[0] = source[0];
            return new string(token);
        }
    }
}
