using System;
using System.Text;

namespace Element34.StringMetrics
{
    public class SoundExRefined : IStringEncoder, IStringComparison
    {
        private const int tokenLength = 4;

        public bool Compare(string value1, string value2)
        {
            throw new NotImplementedException();
        }

        public string Encode(string source)
        {
            StringBuilder result = new StringBuilder();

            if (source != null && source.Length > 0)
            {
                char previousCode = ' ', currentCode, currentLetter;
                result.Append(source[0]); // keep initial char

                for (int i = 0; i < source.Length; i++) //start at 0 in order to correctly encode "Pf..."
                {
                    currentLetter = char.ToUpper(source[i]);
                    currentCode = ' ';

                    switch (currentLetter)
                    {
                        case 'A':
                        case 'E':
                        case 'H':
                        case 'I':
                        case 'O':
                        case 'U':
                        case 'W':
                        case 'Y':
                            currentCode = '0';
                            break;

                        case 'B':
                        case 'P':
                            currentCode = '1';
                            break;

                        case 'F':
                        case 'V':
                            currentCode = '2';
                            break;

                        case 'C':
                        case 'K':
                        case 'S':
                            currentCode = '3';
                            break;

                        case 'G':
                        case 'J':
                            currentCode = '4';
                            break;

                        case 'Q':
                        case 'X':
                        case 'Z':
                            currentCode = '5';
                            break;

                        case 'D':
                        case 'T':
                            currentCode = '6';
                            break;

                        case 'L':
                            currentCode = '7';
                            break;

                        case 'M':
                        case 'N':
                            currentCode = '8';
                            break;

                        case 'R':
                            currentCode = '9';
                            break;

                    }

                    if (currentCode != previousCode && i > 0) // do not add first code to result string
                        result.Append(currentCode);

                    if (result.Length == tokenLength) break;

                    previousCode = currentCode; // always retain previous code, even empty
                }
            }
            if (result.Length < tokenLength)
                result.Append(new string('0', tokenLength - result.Length));

            return result.ToString();
        }

    }
}
