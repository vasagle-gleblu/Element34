﻿using static Element34.StringMetrics.Alphabets;

namespace Element34.StringMetrics.Phonetic
{
    /// <summary>
    /// Metaphone is a phonetic algorithm, published by Lawrence Phillips in 1990, for indexing words by their
    /// English pronunciation. It fundamentally improves on the Soundex algorithm by using information about 
    /// variations and inconsistencies in English spelling and pronunciation to produce a more accurate encoding, 
    /// which does a better job of matching words and names which sound similar.
    /// <a href="https://en.wikipedia.org/wiki/Metaphone" />
    /// </summary>

    public class Metaphone : IStringEncoder, IStringComparison
    {
        private const char SH = 'X';
        private const char TH = '0';
        private string m_normalized;
        private int m_index;
        private int m_length;
        private int m_tokenLength = 4;

        public char[] Encode(char[] buffer)
        {
            return Encode(buffer.ToString()).ToCharArray();
        }

        /// <summary>
        /// Encodes a string with the original Metaphone specification.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>The encoded string.</returns>
        public string Encode(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            m_normalized = value.ToUpper();
            m_length = m_normalized.Length;

            string token = string.Empty;
            int skip;

            char? next = atFactory(1);
            char? current = atFactory(0);
            char? previous = atFactory(-1);

            // Find our first letter
            while (!alpha(current))
            {
                m_index++;
            }

            switch (current)
            {
                case 'A':
                    // AE becomes E
                    if (next == 'E')
                    {
                        token += 'E';
                        m_index += 2;
                    }
                    else
                    {
                        // Remember, preserve vowels at the beginning
                        token += 'A';
                        m_index++;
                    }
                    break;

                // [GKP]N becomes N
                case 'G':
                case 'K':
                case 'P':
                    if (next == 'N')
                    {
                        token += 'N';
                        m_index += 2;
                    }
                    break;

                // WH becomes H, WR becomes R, W if followed by a vowel
                case 'W':
                    if (next == 'R')
                    {
                        token += next;
                        m_index += 2;
                    }
                    else if (next == 'H')
                    {
                        token += current;
                        m_index += 2;
                    }
                    else if (isVowel(next))
                    {
                        token += 'W';
                        m_index += 2;
                    }
                    break;

                // X becomes S
                case 'X':
                    token += 'S';
                    m_index++;
                    break;

                // Vowels are kept (we did A already)
                case 'E':
                case 'I':
                case 'O':
                case 'U':
                    token += current;
                    m_index++;
                    break;

                default:
                    // Ignore
                    break;
            }

            // On to the metaphoning
            while (current != null && token.Length < m_tokenLength)
            {
                // How many letters to skip because an earlier encoding handled multiple
                // letters
                next = atFactory(1);
                current = atFactory(0);
                previous = atFactory(-1);

                skip = 1;

                // Ignore non-alphas
                if (!alpha(current) || (current == previous && current != 'C'))
                {
                    m_index += skip;
                    continue;
                }

                switch (current)
                {
                    // B -> B unless in MB
                    case 'B':
                        if (previous != 'M')
                        {
                            token += 'B';
                        }
                        break;

                    // 'sh' if -CIA- or -CH, but not SCH, except SCHW (SCHW is handled in S)
                    // S if -CI-, -CE- or -CY- dropped if -SCI-, SCE-, -SCY- (handed in S)
                    // else K
                    case 'C':
                        if (soft(next))
                        {
                            // C[IEY]
                            if (next == 'I' && at(2) == 'A')
                            {
                                // CIA
                                token += SH;
                            }
                            else if (previous != 'S')
                            {
                                token += 'S';
                            }
                        }
                        else if (next == 'H')
                        {
                            token += SH;
                            skip++;
                        }
                        else
                        {
                            // C
                            token += 'K';
                        }
                        break;

                    // J if in -DGE-, -DGI- or -DGY-, else T
                    case 'D':
                        if (next == 'G' && soft(at(2)))
                        {
                            token += 'J';
                            skip++;
                        }
                        else
                        {
                            token += 'T';
                        }
                        break;

                    // F if in -GH and not B--GH, D--GH, -H--GH, -H---GH
                    // else dropped if -GNED, -GN,
                    // else dropped if -DGE-, -DGI- or -DGY- (handled in D)
                    // else J if in -GE-, -GI, -GY and not GG
                    // else K
                    case 'G':
                        if (next == 'H')
                        {
                            if (!(noGhToF(at(-3)) || at(-4) == 'H'))
                            {
                                token += 'F';
                                skip++;
                            }
                        }
                        else if (next == 'N')
                        {
                            if (!(!alpha(at(2)) || (at(2) == 'E' && at(3) == 'D')))
                            {
                                token += 'K';
                            }
                        }
                        else if (soft(next) && previous != 'G')
                        {
                            token += 'J';
                        }
                        else
                        {
                            token += 'K';
                        }
                        break;

                    // H if before a vowel and not after C,G,P,S,T
                    case 'H':
                        if (isVowel(next) && !dipthongH(previous))
                        {
                            token += 'H';
                        }
                        break;

                    // Dropped if after C, else K
                    case 'K':
                        if (previous != 'C')
                        {
                            token += 'K';
                        }
                        break;

                    // F if before H, else P
                    case 'P':
                        token += (next == 'H') ? 'F' : 'P';
                        break;

                    // K
                    case 'Q':
                        token += 'K';
                        break;

                    // 'sh' in -SH-, -SIO- or -SIA- or -SCHW-, else S
                    case 'S':
                        if (next == 'I' && (at(2) == 'O' || at(2) == 'A'))
                        {
                            token += SH;
                        }
                        else if (next == 'H')
                        {
                            token += SH;
                            skip++;
                        }
                        else
                        {
                            token += 'S';
                        }
                        break;

                    // 'sh' in -TIA- or -TIO-, else 'th' before H, else T
                    case 'T':
                        if (next == 'I' && (at(2) == 'O' || at(2) == 'A'))
                        {
                            token += SH;
                        }
                        else if (next == 'H')
                        {
                            token += TH;
                            skip++;
                        }
                        else if (!(next == 'C' && at(2) == 'H'))
                        {
                            token += 'T';
                        }
                        break;

                    // F
                    case 'V':
                        token += 'F';
                        break;
                    case 'W':
                        if (isVowel(next))
                        {
                            token += 'W';
                        }
                        break;

                    // KS
                    case 'X':
                        token += "KS";
                        break;

                    // Y if followed by a vowel
                    case 'Y':
                        if (isVowel(next))
                        {
                            token += 'Y';
                        }
                        break;

                    // S
                    case 'Z':
                        token += 'S';
                        break;

                    // No transformation
                    case 'F':
                    case 'J':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'R':
                        token += current;
                        break;
                }

                m_index += skip;
            }

            return token;
        }

        /// <summary>
        /// Compares the specified values using original Metaphone algorithm.
        /// </summary>
        /// <param name="value1">string A</param>
        /// <param name="value2">string B</param>
        /// <returns>Results in true if the encoded input strings match.</returns>
        public bool Compare(string value1, string value2)
        {
            Metaphone mph = new Metaphone();
            value1 = mph.Encode(value1);
            value2 = mph.Encode(value2);

            return value1.Equals(value2);
        }

        /// <summary>
        /// Create an 'at' function with a bound 'offset'.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>nullable char</returns>
        private char? atFactory(int offset)
        {
            return at(offset);
        }

        /// <summary>
        /// Get the character offset by 'offset' from the current character.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>nullable char</returns>
        private char? at(int offset)
        {
            if ((m_index + offset) < 0)
                return null;
            else if ((m_index + offset) > (m_length - 1))
                return null;
            else
                return m_normalized[m_index + offset];
        }

        /// <summary>
        /// Check whether 'character' would make 'GH' an 'F'
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>Boolean</returns>
        private bool noGhToF(char? character)
        {
            return character == 'B' || character == 'D' || character == 'H';
        }

        /// <summary>
        /// Check whether 'character' would make a 'C' or 'G' soft
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>Boolean</returns>
        private bool soft(char? character)
        {
            return character == 'E' || character == 'I' || character == 'Y';
        }

        /// <summary>
        /// Check whether 'character' is a vowel.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>Boolean</returns>
        private bool isVowel(char? character)
        {
            return UppercaseVowel.IsContained((char)character);
        }

        /// <summary>
        /// Check whether 'character' forms a dipthong when preceding H.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns>Boolean</returns>
        private bool dipthongH(char? character)
        {
            return UppercaseDipthongH.IsContained((char)character);
        }

        /// <summary>
        /// Check whether 'character' is in the alphabet.
        /// </summary>
        /// <param name="character">The character.</param>
        /// <returns></returns>
        private bool alpha(char? character)
        {
            if (character == null)
                return false;

            return UppercaseAlpha.IsContained((char)character);
        }

    }
}
