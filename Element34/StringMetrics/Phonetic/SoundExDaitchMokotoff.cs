using Element34.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Element34.StringMetrics
{
    public class SoundExDaitchMokotoff : IStringEncoder, IStringComparison
    {
        private readonly char[] vowels = new char[] { 'A', 'E', 'I', 'J', 'O', 'U', 'Y' };

        #region Fields
        private int tokenLength = 6;
        private string input;
        private string primaryKey;
        private string alternateKey;
        private int inputLength;
        private char SEPARATOR = ' ';
        private string GROUPSEPARATOR = " ";
        #endregion

        public bool Compare(string value1, string value2)
        {
            SoundExDaitchMokotoff dsm = new SoundExDaitchMokotoff();
            value1 = dsm.Encode(value1);
            value2 = dsm.Encode(value2);

            return value1.Equals(value2);
        }

        /// <summary>
        /// generate a Daitch-Mokotoff Soundex key for the given string 
        /// with the default Length of 6
        /// </summary>
        /// <param name="strInput">string for which to compute the key</param>
        /// <returns>primary key</returns>
        public string Encode(string source)
        {
            string result = string.Empty;
            string[] sourceArray;

            source = Regex.Replace(source, @" v | v\. | vel | aka | f | f. | r | r. | false | recte | on zhe", "/");
            sourceArray = Regex.Split(source, @"[\s|,]+");

            for (int k = 0; k < sourceArray.Length; k++)
            {
                if (sourceArray[k].Length > 0)
                    if (k != 0)
                    {
                        result += GROUPSEPARATOR;
                    }

                result += GenerateKey(source, 6);
            }

            return result;
        }

        /// <summary>
        /// generate a Daitch-Mokotoff Soundex key for the given string
        /// </summary>
        /// <param name="strInput">string for which to compute the key</param>
        /// <param name="keyLength">Length of the key to generate</param>
        /// <returns>primary key</returns>
        public string GenerateKey(string sInput, int keyLength)
        {
            string tmp = string.Empty;


            // reset the internal input string
            this.Reset();

            this.tokenLength = keyLength;

            sInput = CheckInput(sInput);

            if (sInput == null)
                return primaryKey;

            input = sInput;

            //just to speed things up
            int inputLength = input.Length;
            char lastChar = '\0';

            // main loop
            for (int i = 0; i < inputLength; i++)
            {
                if (lastChar == input[i])
                    continue;

                switch (input[i])
                {
                    case 'A':
                        i = HandleA(i);
                        break;
                    case 'B':
                        AddToKey("7");
                        break;
                    case 'C':
                        i = HandleC(i);
                        break;
                    case 'D':
                        i = HandleD(i);
                        break;
                    case 'E':
                        i = HandleE(i);
                        break;
                    case 'F':
                        i = HandleF(i);
                        break;
                    case 'G':
                        AddToKey("5");
                        break;
                    case 'H':
                        i = HandleH(i);
                        break;
                    case 'I':
                        i = HandleI(i);
                        break;
                    case 'J':
                        AddToKey("1", "4");
                        break;
                    case 'K':
                        i = HandleK(i);
                        break;
                    case 'L':
                        AddToKey("8");
                        break;
                    case 'M':
                        i = HandleM(i);
                        break;
                    case 'N':
                        i = HandleN(i);
                        break;
                    case 'O':
                        i = HandleO(i);
                        break;
                    case 'P':
                        i = HandleP(i);
                        break;
                    case 'Q':
                        AddToKey("5");
                        break;
                    case 'R':
                        i = HandleR(i);
                        break;
                    case 'S':
                        i = HandleS(i);
                        break;
                    case 'T':
                        i = HandleT(i);
                        break;
                    case 'U':
                        i = HandleU(i);
                        break;
                    case 'V':
                        AddToKey("7");
                        break;
                    case 'W':
                        AddToKey("7");
                        break;
                    case 'X':
                        i = HandleX(i);
                        break;
                    case 'Y':
                        i = HandleY(i);
                        break;
                    case 'Z':
                        i = HandleZ(i);
                        break;
                    case '/':
                        input = input.Substring(i + 1);
                        break;
                    default:
                        if (sInput[i] == '(' || sInput[i] == SEPARATOR)
                        {
                            break;
                        }
                        tmp = tmp + input[i];
                        break;
                }

                lastChar = input[i];
            }

            if (primaryKey.Length > keyLength)
                return primaryKey.Substring(0, keyLength);

            return primaryKey.PadRight(keyLength, '0');
        }

        /// <summary>
        /// Handle the Z
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleZ(int i)
        {
            if (IsMatch(i, "ZDZ", "ZDZH", "ZHDZH"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if (IsMatch(i, "ZHDZH"))
                    return i + 4;
                else if (IsMatch(i, "ZDZH"))
                    return i + 3;
                else
                    return i + 2;
            }// end ZDZ,ZDZH, ZHDZH

            if (IsMatch(i, "ZD", "ZHD"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                if (IsMatch(i, "ZHD"))
                    return i + 2;
                else
                    return i + 1;
            }// End ZD, ZHD

            AddToKey("4");
            if (IsMatch(i, "ZSCH"))
                return i + 3;
            else if (IsMatch(i, "ZSH"))
                return i + 2;
            else if (IsMatch(i, "ZS", "ZH"))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the Y
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleY(int i)
        {
            if (i == 0)
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the X
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleX(int i)
        {
            if (i == 0)
                AddToKey("5");
            else
                AddToKey("54");
            return i;
        }

        /// <summary>
        /// Handle the U
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleU(int i)
        {
            if (IsChar(i + 1, 'I', 'J', 'Y'))
            {
                if (i == 0)
                    AddToKey("0");
                else if (IsVowel(i + 2))
                    AddToKey("1");
                return i + 1;
            }

            if (i == 0)
                AddToKey("0");

            if (IsChar(i, 'E'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the T
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleT(int i)
        {
            if (IsMatch(i, "TCH", "TTCH", "TTSCH"))
            {
                AddToKey("4");
                if (IsMatch(i, "TTSCH"))
                    return i + 4;
                else if (IsMatch(i, "TTCH"))
                    return i + 3;
                else
                    return i + 2;
            } // end TCH, TTCH, TTSCH

            if (IsMatch(i, "TH"))
            {
                AddToKey("3");
                return i + 1;
            } // end TH

            if (IsMatch(i, "TRZ", "TRS"))
            {
                AddToKey("4");
                return i + 2;
            }// end TRZ,TRS

            if (IsMatch(i, "TSCH", "TSH"))
            {
                AddToKey("4");
                if (IsMatch(i, "TSCH"))
                    return i + 3;
                else
                    return i + 2;
            }// end TSCH, TSH

            if (IsMatch(i, "TS", "TTS", "TTSZ", "TC"))
            {
                AddToKey("4");
                if (IsMatch(i, "TTSZ"))
                    return i + 3;
                else if (IsMatch(i, "TTS"))
                    return i + 2;
                else
                    return i + 1;
            }// end TS, TTS, TTSZ, TC

            if (IsMatch(i, "TZ", "TTZ", "TZS", "TSZ"))
            {
                AddToKey("4");
                if (IsMatch(i, "TTZ", "TZS", "TSZ"))
                    return i + 2;
                else
                    return i + 1;
            }// end TZ, TTZ, TZS, TSZ

            AddToKey("3");
            return i;
        }

        /// <summary>
        /// Handle the S
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleS(int i)
        {
            if (IsMatch(i, "SCH"))
            {
                if (IsMatch(i, "SCHTSCH", "SCHTSH", "SCHTCH") && i == 0)
                    AddToKey("2");
                else if (IsMatch(i, "SCHT", "SCHD"))
                    if (i == 0)
                        AddToKey("2");
                    else
                        AddToKey("43");
                else
                    AddToKey("4");

                if (IsMatch(i, "SCHTSCH"))
                    return i + 6;
                else if (IsMatch(i, "SCHTSH", "SCHTCH"))
                    return i + 5;
                else if (IsMatch(i, "SCHT", "SCHD"))
                    return i + 3;
                else
                    return i + 2;
            }// end SCH

            if (IsMatch(i, "SHTCH", "SHCH", "SHTSH"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if (IsMatch(i, "SHCH"))
                    return i + 3;
                else
                    return i + 4;
            } // end SHTCH SHCH SHTSH

            if (IsMatch(i, "SHT"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");

                return i + 2;
            } //end SHT

            if (IsMatch(i, "SH"))
            {
                AddToKey("4");
                return i + 1;
            } // end SH

            if (IsMatch(i, "STCH", "STSCH", "SC"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                if (IsMatch(i, "STSCH"))
                    return i + 4;
                else if (IsMatch(i, "STCH"))
                    return i + 3;
                else
                    return i + 1;
            }// end STCH, STSCH, SC


            if (IsMatch(i, "STRZ", "STRS", "STSH"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");

                return i + 3;
            }//End STRZ, STRS, STSH

            if (IsMatch(i, "ST"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i + 1;
            }// end ST

            if (IsMatch(i, "SZCZ", "SZCS"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("4");
                return i + 3;
            } // end SZCZ, SZCS

            if (IsMatch(i, "SZT", "SHD", "SZD"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i + 2;
            } // end SZT, SHD, SZD

            if (IsMatch(i, "SD"))
            {
                if (i == 0)
                    AddToKey("2");
                else
                    AddToKey("43");
                return i + 1;
            }// end SD

            AddToKey("4");
            if (IsMatch(i, "SZ"))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the R
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleR(int i)
        {
            if (IsChar(i + 1, 'S', 'Z'))
            {
                AddToKey("94", "4");
                return i + 1;
            }

            AddToKey("9");
            return i;
        }

        /// <summary>
        /// Handle the P
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleP(int i)
        {
            AddToKey("7");

            if (IsChar(i + 1, 'F', 'H'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the O
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleO(int i)
        {
            if (IsChar(i + 1, 'I', 'J', 'Y'))
            {
                if (i == 0)
                    AddToKey("0");
                else if (IsVowel(i + 2))
                    AddToKey("1");
                return i + 1;
            }

            if (i == 0)
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the N
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleN(int i)
        {
            if (IsChar(i + 1, 'M') && i != 0)
            {
                AddToKey("66");
                return i + 1;
            }

            AddToKey("6");
            return i;
        }

        /// <summary>
        /// Handle the M
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleM(int i)
        {
            if (IsChar(i + 1, 'N') && i != 0)
            {
                AddToKey("66");
                return i + 1;
            }

            AddToKey("6");
            return i;
        }

        /// <summary>
        /// Handle the K
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleK(int i)
        {
            if (IsChar(i + 1, 'S'))
            {
                if (i == 0)
                    AddToKey("5");
                else
                    AddToKey("54");
                return i + 1;
            }

            AddToKey("5");
            if (IsChar(i + 1, 'H'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the I
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleI(int i)
        {
            if (IsChar(i + 1, 'A', 'E', 'O', 'U') && i == 0)
            {
                AddToKey("1");
                return i + 1;
            }

            if (i == 0)
                AddToKey("0");
            return i;

        }

        /// <summary>
        /// Handle the H
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleH(int i)
        {
            if (i == 0 || IsVowel(i + 1))
                AddToKey("5");
            return i;

        }

        /// <summary>
        /// Handle the F
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleF(int i)
        {
            AddToKey("7");
            if (IsChar(i + 1, 'B'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the E
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleE(int i)
        {
            if (IsChar(i + 1, 'I', 'J', 'Y'))
            {
                if (i == 0)
                    AddToKey("0");
                else if (IsVowel(i + 2))
                    AddToKey("1");

                return i + 1;
            }

            if (IsChar(i + 1, 'U'))
            {
                if (i == 0 || IsVowel(i + 2))
                    AddToKey("1");

                return i + 1;
            }

            if (i == 0)
                AddToKey("0");
            return i;
        }

        /// <summary>
        /// Handle the D
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleD(int i)
        {
            if (IsMatch(i, "DRZ", "DRS", "DSH", "DSZ", "DZH", "DZS"))
            {
                AddToKey("4");
                return i + 2;
            }

            if (IsMatch(i, "DS", "DZ"))
            {
                AddToKey("4");
                return i + 1;
            }

            AddToKey("3");
            if (IsChar(i + 1, 'T'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Handle the C
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleC(int i)
        {
            if (IsChar(i + 1, 'H'))
            {
                AddToKey("5", "4");
                return i + 1;
            }

            if (IsChar(i + 1, 'K'))
            {
                AddToKey("5", "45");
                return i + 1;
            }

            if (IsMatch(i, "CS", "CSZ", "CZS"))
            {
                AddToKey("4");
                if (IsMatch(i, "CSZ", "CZS"))
                    return i + 2;
                else
                    return i + 1;
            }

            AddToKey("5", "4");
            return i;

        }

        /// <summary>
        /// Handle the A
        /// </summary>
        /// <param name="i">Current position in the input string</param>
        /// <returns>new position in the string</returns>
        private int HandleA(int i)
        {
            if (i == 0)
                AddToKey("0");
            else if (IsChar(i + 1, 'I', 'J', 'Y') && IsVowel(i + 2))
                AddToKey("1");
            else if (IsChar(i + 1, 'u') && IsVowel(i + 2))
                AddToKey("7");

            if (IsChar(i + 1, 'I', 'J', 'Y', 'U'))
                return i + 1;
            else
                return i;
        }

        /// <summary>
        /// Check whatever the letter at the given position in the input string is a vowel
        /// Vowels are A, E, I, O, U and Y
        /// </summary>
        /// <param name="pos">Position in the input string</param>
        /// <returns>Vowel or not</returns>
        private bool IsVowel(int pos)
        {
            return IsChar(pos, vowels);
        }

        /// <summary>
        /// Get the primary key
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                return primaryKey.PadRight(tokenLength, '0');
            }
        }

        /// <summary>
        /// Get the alternate key
        /// If no alternate key is present the primary key is returned
        /// </summary>
        public string AlternateKey
        {
            get
            {
                return alternateKey.PadRight(tokenLength, '0');
            }
        }

        /// <summary>
        /// Reset the primaryKey, alternateKey and input;
        /// </summary>
        private void Reset()
        {
            input = "";
            primaryKey = "";
            alternateKey = "";
            inputLength = 0;
        }

        /// <summary>
        /// Add a string to the key
        /// </summary>
        /// <param name="primary">string to add</param>
        private void AddToKey(string primary)
        {
            AddToKey(primary, null);
        }

        /// <summary>
        /// Add a string to the primaryKey and alternate key
        /// </summary>
        /// <param name="primary">string to add to the primary key</param>
        /// <param name="alternate">string to add to the alternate key</param>
        private void AddToKey(string primary, string alternate)
        {
            primaryKey += primary;

            if (null == alternate)
                alternateKey += primary;
            else
                alternateKey += alternate;
        }

        /// <summary>
        /// Do a simple check if the input can be computed
        /// </summary>
        /// <param name="strInput"></param>
        private string CheckInput(string strInput)
        {
            strInput = strInput.Trim();

            // Cant work with empty string
            if (strInput.Equals(""))
                return null;

            return strInput.ToUpper();
        }

        /// <summary>
        /// Check whatever the char at the given position is char given
        /// </summary>
        /// <param name="pos">Position of the char</param>
        /// <param name="c">Char which to check</param>
        /// <returns>match</returns>
        protected bool IsChar(int pos, Char c)
        {
            bool result = false;

            if (pos >= 0 && pos < inputLength)
                if (input[pos] == c)
                    result = true;

            return result;
        }

        /// <summary>
        /// Check whatever the char matched any of the chars given
        /// </summary>
        /// <param name="pos">position of the char in th input string</param>
        /// <param name="charList">array of char to match</param>
        /// <returns>match</returns>
        private bool IsChar(int pos, params char[] charList)
        {
            bool result = false;

            if (pos >= 0 && pos < inputLength)
            {
                char inputChar = input[pos];

                foreach (char c in charList)
                    if (c == inputChar)
                        result = true;
            }

            return result;
        }

        /// <summary>
        /// Check id the a string the match is present at the given position
        /// </summary>
        /// <param name="pos">position in the input string</param>
        /// <param name="toMatch">string to match on the given position</param>
        /// <returns></returns>
        private bool IsMatch(int pos, string toMatch)
        {
            bool result = false;

            int Length = toMatch.Length;

            if ((pos + Length) <= inputLength && pos >= 0)
                if (input.Substring(pos, Length) == toMatch)
                    result = true;

            return result;
        }

        /// <summary>
        /// Check if any of the given matches match the string form the given position in the input string
        /// </summary>
        /// <param name="pos">position in the input string</param>
        /// <param name="toMatch">strings to match on the given position</param>
        /// <returns></returns>
        private bool IsMatch(int pos, params string[] toMatch)
        {
            bool result = false;

            foreach (string match in toMatch)
                if (IsMatch(pos, match))
                    result = true;

            return result;
        }
    }

    //=============================================================================

    public class DaitchMokotoffSoundEx
    {
        /** The code Length of a DM soundex value. */
        protected static int MAX_LENGTH = 6;

        /** Whether to use ASCII folding prior to encoding. */
        private bool folding;

        /** Transformation rules indexed by the first character of their pattern. */
        private Dictionary<string, List<Rule>> RULES = new Dictionary<string, List<Rule>>() {
              // Vowels
              { "a", new List<Rule>() { new Rule("a", "0", "", "") } },
              { "e", new List<Rule>() { new Rule("e", "0", "", "") } },
              { "i", new List<Rule>() { new Rule("i", "0", "", "") } },
              { "o", new List<Rule>() { new Rule("o", "0", "", "") } },
              { "u", new List<Rule>() { new Rule("u", "0", "", "") } },

              // Consonants
              { "b", new List<Rule>() { new Rule("b", "7", "7", "7") } },
              { "d", new List<Rule>() { new Rule("d", "3", "3", "3") } },
              { "f", new List<Rule>() { new Rule("f", "7", "7", "7") } },
              { "g", new List<Rule>() { new Rule("g", "5", "5", "5") } },
              { "h", new List<Rule>() { new Rule("h", "5", "5", "") } },
              { "k", new List<Rule>() { new Rule("k", "5", "5", "5") } },
              { "l", new List<Rule>() { new Rule("l", "8", "8", "8") } },
              { "m", new List<Rule>() { new Rule("m", "6", "6", "6") } },
              { "n", new List<Rule>() { new Rule("n", "6", "6", "6") } },
              { "p", new List<Rule>() { new Rule("p", "7", "7", "7") } },
              { "q", new List<Rule>() { new Rule("q", "5", "5", "5") } },
              { "r", new List<Rule>() { new Rule("r", "9", "9", "9") } },
              { "s", new List<Rule>() { new Rule("s", "4", "4", "4") } },
              { "t", new List<Rule>() { new Rule("t", "3", "3", "3") } },
              { "v", new List<Rule>() { new Rule("v", "7", "7", "7") } },
              { "w", new List<Rule>() { new Rule("w", "7", "7", "7") } },
              { "x", new List<Rule>() { new Rule("x", "5", "54", "54") } },
              { "y", new List<Rule>() { new Rule("y", "1", "", "") } },
              { "z", new List<Rule>() { new Rule("z", "4", "4", "4") } },

              // Romanian t-cedilla and t-comma should be equivalent
              { "ţ", new List<Rule>() { new Rule("ţ", "3|4", "3|4", "3|4") } },
              { "ț", new List<Rule>() { new Rule("ț", "3|4", "3|4", "3|4") } },

              // Polish characters (e-ogonek and a-ogonek): default case branch either
              // not coded or 6
              { "ę", new List<Rule>() { new Rule("ę", "", "", "|6") } },
              { "ą", new List<Rule>() { new Rule("ą", "", "", "|6") } },

              // Other terms
              { "schtsch", new List<Rule>() { new Rule("schtsch", "2", "4", "4") } },
              { "schtsh", new List<Rule>() { new Rule("schtsh", "2", "4", "4") } },
              { "schtch", new List<Rule>() { new Rule("schtch", "2", "4", "4") } },
              { "shtch", new List<Rule>() { new Rule("shtch", "2", "4", "4") } },
              { "shtsh", new List<Rule>() { new Rule("shtsh", "2", "4", "4") } },
              { "stsch", new List<Rule>() { new Rule("stsch", "2", "4", "4") } },
              { "ttsch", new List<Rule>() { new Rule("ttsch", "4", "4", "4") } },
              { "zhdzh", new List<Rule>() { new Rule("zhdzh", "2", "4", "4") } },
              { "shch", new List<Rule>() { new Rule("shch", "2", "4", "4") } },
              { "scht", new List<Rule>() { new Rule("scht", "2", "43", "43") } },
              { "schd", new List<Rule>() { new Rule("schd", "2", "43", "43") } },
              { "stch", new List<Rule>() { new Rule("stch", "2", "4", "4") } },
              { "strz", new List<Rule>() { new Rule("strz", "2", "4", "4") } },
              { "strs", new List<Rule>() { new Rule("strs", "2", "4", "4") } },
              { "stsh", new List<Rule>() { new Rule("stsh", "2", "4", "4") } },
              { "szcz", new List<Rule>() { new Rule("szcz", "2", "4", "4") } },
              { "szcs", new List<Rule>() { new Rule("szcs", "2", "4", "4") } },
              { "ttch", new List<Rule>() { new Rule("ttch", "4", "4", "4") } },
              { "tsch", new List<Rule>() { new Rule("tsch", "4", "4", "4") } },
              { "ttsz", new List<Rule>() { new Rule("ttsz", "4", "4", "4") } },
              { "zdzh", new List<Rule>() { new Rule("zdzh", "2", "4", "4") } },
              { "zsch", new List<Rule>() { new Rule("zsch", "4", "4", "4") } },
              { "chs", new List<Rule>() { new Rule("chs", "5", "54", "54") } },
              { "csz", new List<Rule>() { new Rule("csz", "4", "4", "4") } },
              { "czs", new List<Rule>() { new Rule("czs", "4", "4", "4") } },
              { "drz", new List<Rule>() { new Rule("drz", "4", "4", "4") } },
              { "drs", new List<Rule>() { new Rule("drs", "4", "4", "4") } },
              { "dsh", new List<Rule>() { new Rule("dsh", "4", "4", "4") } },
              { "dsz", new List<Rule>() { new Rule("dsz", "4", "4", "4") } },
              { "dzh", new List<Rule>() { new Rule("dzh", "4", "4", "4") } },
              { "dzs", new List<Rule>() { new Rule("dzs", "4", "4", "4") } },
              { "sch", new List<Rule>() { new Rule("sch", "4", "4", "4") } },
              { "sht", new List<Rule>() { new Rule("sht", "2", "43", "43") } },
              { "szt", new List<Rule>() { new Rule("szt", "2", "43", "43") } },
              { "shd", new List<Rule>() { new Rule("shd", "2", "43", "43") } },
              { "szd", new List<Rule>() { new Rule("szd", "2", "43", "43") } },
              { "tch", new List<Rule>() { new Rule("tch", "4", "4", "4") } },
              { "trz", new List<Rule>() { new Rule("trz", "4", "4", "4") } },
              { "trs", new List<Rule>() { new Rule("trs", "4", "4", "4") } },
              { "tsh", new List<Rule>() { new Rule("tsh", "4", "4", "4") } },
              { "tts", new List<Rule>() { new Rule("tts", "4", "4", "4") } },
              { "ttz", new List<Rule>() { new Rule("ttz", "4", "4", "4") } },
              { "tzs", new List<Rule>() { new Rule("tzs", "4", "4", "4") } },
              { "tsz", new List<Rule>() { new Rule("tsz", "4", "4", "4") } },
              { "zdz", new List<Rule>() { new Rule("zdz", "2", "4", "4") } },
              { "zhd", new List<Rule>() { new Rule("zhd", "2", "43", "43") } },
              { "zsh", new List<Rule>() { new Rule("zsh", "4", "4", "4") } },
              { "ai", new List<Rule>() { new Rule("ai", "0", "1", "") } },
              { "aj", new List<Rule>() { new Rule("aj", "0", "1", "") } },
              { "ay", new List<Rule>() { new Rule("ay", "0", "1", "") } },
              { "au", new List<Rule>() { new Rule("au", "0", "7", "") } },
              { "cz", new List<Rule>() { new Rule("cz", "4", "4", "4") } },
              { "cs", new List<Rule>() { new Rule("cs", "4", "4", "4") } },
              { "ds", new List<Rule>() { new Rule("ds", "4", "4", "4") } },
              { "dz", new List<Rule>() { new Rule("dz", "4", "4", "4") } },
              { "dt", new List<Rule>() { new Rule("dt", "3", "3", "3") } },
              { "ei", new List<Rule>() { new Rule("ei", "0", "1", "") } },
              { "ej", new List<Rule>() { new Rule("ej", "0", "1", "") } },
              { "ey", new List<Rule>() { new Rule("ey", "0", "1", "") } },
              { "eu", new List<Rule>() { new Rule("eu", "1", "1", "") } },
              { "fb", new List<Rule>() { new Rule("fb", "7", "7", "7") } },
              { "ia", new List<Rule>() { new Rule("ia", "1", "", "") } },
              { "ie", new List<Rule>() { new Rule("ie", "1", "", "") } },
              { "io", new List<Rule>() { new Rule("io", "1", "", "") } },
              { "iu", new List<Rule>() { new Rule("iu", "1", "", "") } },
              { "ks", new List<Rule>() { new Rule("ks", "5", "54", "54") } },
              { "kh", new List<Rule>() { new Rule("kh", "5", "5", "5") } },
              { "mn", new List<Rule>() { new Rule("mn", "66", "66", "66") } },
              { "nm", new List<Rule>() { new Rule("nm", "66", "66", "66") } },
              { "oi", new List<Rule>() { new Rule("oi", "0", "1", "") } },
              { "oj", new List<Rule>() { new Rule("oj", "0", "1", "") } },
              { "oy", new List<Rule>() { new Rule("oy", "0", "1", "") } },
              { "pf", new List<Rule>() { new Rule("pf", "7", "7", "7") } },
              { "ph", new List<Rule>() { new Rule("ph", "7", "7", "7") } },
              { "sh", new List<Rule>() { new Rule("sh", "4", "4", "4") } },
              { "sc", new List<Rule>() { new Rule("sc", "2", "4", "4") } },
              { "st", new List<Rule>() { new Rule("st", "2", "43", "43") } },
              { "sd", new List<Rule>() { new Rule("sd", "2", "43", "43") } },
              { "sz", new List<Rule>() { new Rule("sz", "4", "4", "4") } },
              { "th", new List<Rule>() { new Rule("th", "3", "3", "3") } },
              { "ts", new List<Rule>() { new Rule("ts", "4", "4", "4") } },
              { "tc", new List<Rule>() { new Rule("tc", "4", "4", "4") } },
              { "tz", new List<Rule>() { new Rule("tz", "4", "4", "4") } },
              { "ui", new List<Rule>() { new Rule("ui", "0", "1", "") } },
              { "uj", new List<Rule>() { new Rule("uj", "0", "1", "") } },
              { "uy", new List<Rule>() { new Rule("uy", "0", "1", "") } },
              { "ue", new List<Rule>() { new Rule("ue", "0", "1", "") } },
              { "zd", new List<Rule>() { new Rule("zd", "2", "43", "43") } },
              { "zh", new List<Rule>() { new Rule("zh", "4", "4", "4") } },
              { "zs", new List<Rule>() { new Rule("zs", "4", "4", "4") } },

              // Branching cases
              { "c", new List<Rule>() { new Rule("c", "4|5", "4|5", "4|5") } },
              { "ch", new List<Rule>() { new Rule("ch", "4|5", "4|5", "4|5") } },
              { "ck", new List<Rule>() { new Rule("ck", "5|45", "5|45", "5|45") } },
              { "rs", new List<Rule>() { new Rule("rs", "4|94", "4|94", "4|94") } },
              { "rz", new List<Rule>() { new Rule("rz", "4|94", "4|94", "4|94") } },
              { "j", new List<Rule>() { new Rule("j", "1|4", "|4", "|4") } }
            };

        /** Folding rules. */
        private Dictionary<char, char> FOLDINGS = new Dictionary<char, char>() {
          { 'ß', 's' }, { 'à', 'a' }, { 'á', 'a' }, { 'â', 'a' }, { 'ã', 'a' },
          { 'ä', 'a' }, { 'å', 'a' }, { 'æ', 'a' }, { 'ç', 'c' }, { 'è', 'e' },
          { 'é', 'e' }, { 'ê', 'e' }, { 'ë', 'e' }, { 'ì', 'i' }, { 'í', 'i' },
          { 'î', 'i' }, { 'ï', 'i' }, { 'ð', 'd' }, { 'ñ', 'n' }, { 'ò', 'o' },
          { 'ó', 'o' }, { 'ô', 'o' }, { 'õ', 'o' }, { 'ö', 'o' }, { 'ø', 'o' },
          { 'ù', 'u' }, { 'ú', 'u' }, { 'û', 'u' }, { 'ý', 'y' }, { 'ỳ', 'y' },
          { 'þ', 'b' }, { 'ÿ', 'y' }, { 'ć', 'c' }, { 'ł', 'l' }, { 'ś', 's' },
          { 'ż', 'z' }, { 'ź', 'z' }
        };

        /**
         * Creates a new instance with ASCII-folding enabled.
         */
        public DaitchMokotoffSoundEx()
        {
            this.folding = true;
        }

        /**
         * Creates a new instance.
         * <p>
         * With ASCII-folding enabled, certain accented characters will be transformed to equivalent ASCII characters, e.g.
         * è -&gt; e.
         * </p>
         *
         * @param folding
         *            if ASCII-folding shall be performed before encoding
         */
        public DaitchMokotoffSoundEx(bool folding)
        {
            this.folding = folding;
        }

        /**
         * Performs a cleanup of the input string before the actual soundex transformation.
         * <p>
         * Removes all whitespace characters and performs ASCII folding if enabled.
         * </p>
         *
         * @param input
         *            the input string to clean up
         * @return a cleaned up string
         */
        private string cleanup(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in input.ToCharArray())
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                char _ch = char.ToLower(ch);
                char character = FOLDINGS.GetValue(_ch);

                if (folding && character != '\0')
                {
                    _ch = (char)character;
                }
                sb.Append(_ch);
            }
            return sb.ToString();
        }

        /**
         * Encodes a string using the Daitch-Mokotoff soundex algorithm without branching.
         *
         * @see #soundex(string)
         *
         * @param source
         *            A string object to encode
         * @return A DM Soundex code corresponding to the string supplied
         * @throws IllegalArgumentException
         *             if a character is not mapped
         */
        public string Encode(string source)
        {
            if (source == null)
            {
                return null;
            }
            return soundex(source, false)[0];
        }

        /**
         * Encodes a string using the Daitch-Mokotoff soundex algorithm with branching.
         * <p>
         * In case a string is encoded into multiple codes (see branching rules), the result will contain all codes,
         * separated by '|'.
         * </p>
         * <p>
         * Example: the name "AUERBACH" is encoded as both
         * </p>
         * <ul>
         * <li>097400</li>
         * <li>097500</li>
         * </ul>
         * <p>
         * Thus the result will be "097400|097500".
         * </p>
         *
         * @param source
         *            A string object to encode
         * @return A string containing a set of DM Soundex codes corresponding to the string supplied
         * @throws IllegalArgumentException
         *             if a character is not mapped
         */
        public string soundex(string source)
        {
            string[] branches = soundex(source, true);
            StringBuilder sb = new StringBuilder();
            int index = 0;
            foreach (string branch in branches)
            {
                sb.Append(branch);
                if (++index < branches.Length)
                {
                    sb.Append('|');
                }
            }
            return sb.ToString();
        }

        /**
       * Perform the actual DM Soundex algorithm on the input string.
       *
       * @param source
       *            A string object to encode
       * @param branching
       *            If branching shall be performed
       * @return A string array containing all DM Soundex codes corresponding to the string supplied depending on the
       *         selected branching mode
       */
        private string[] soundex(string source, bool branching)
        {
            if (source == null)
            {
                return null;
            }

            string input = cleanup(source);

            List<Branch> currentBranches = new List<Branch>();
            currentBranches.Add(new Branch());

            string lastChar = "\0";
            for (int i = 0; i < input.Length; i++)
            {
                string ch = input[i].ToString();

                // ignore whitespace inside a name
                if (ch.IsWhiteSpace())
                {
                    continue;
                }

                string inputContext = input.Substring(i);
                List<Rule> rules = RULES[ch];
                if (rules == null)
                {
                    continue;
                }

                // use an EMPTY_LIST to avoid false positive warnings with potential null pointer access
                List<Branch> nextBranches = branching ? new List<Branch>() : null;

                foreach (Rule rule in rules)
                {
                    if (rule.matches(inputContext))
                    {
                        if (branching)
                        {
                            nextBranches.Clear();
                        }
                        string[] replacements = rule.getReplacements(inputContext, lastChar == "\0");
                        bool branchingRequired = replacements.Length > 1 && branching;

                        foreach (Branch branch in currentBranches)
                        {
                            foreach (string nextReplacement in replacements)
                            {
                                // if we have multiple replacements, always create a new branch
                                Branch nextBranch = branchingRequired ? branch.createBranch() : branch;

                                // special rule: occurrences of mn or nm are treated differently
                                bool force = (lastChar == "m" && ch == "n") || (lastChar == "n" && ch == "m");

                                nextBranch.processNextReplacement(nextReplacement, force);

                                if (!branching)
                                {
                                    break;
                                }
                                nextBranches.Add(nextBranch);
                            }
                        }

                        if (branching)
                        {
                            currentBranches.Clear();
                            currentBranches.AddRange(nextBranches);
                        }
                        i += rule.getPatternLength() - 1;
                        break;
                    }
                }

                lastChar = ch;
            }

            string[] result = new string[currentBranches.Capacity];
            int index = 0;
            foreach (Branch branch in currentBranches)
            {
                branch.finish();
                result[index++] = branch.ToString();
            }

            return result;
        }

        /**
         * Inner class representing a branch during DM soundex encoding.
         */
        internal class Branch
        {
            private StringBuilder builder;
            private string cachedstring;
            private string lastReplacement;

            public Branch()
            {
                builder = new StringBuilder();
                lastReplacement = null;
                cachedstring = null;
            }

            /**
             * Creates a new branch, identical to this branch.
             *
             * @return a new, identical branch
             */
            public Branch createBranch()
            {
                Branch branch = new Branch();
                branch.builder.Append(ToString());
                branch.lastReplacement = this.lastReplacement;
                return branch;
            }

            public override bool Equals(object other)
            {
                if (this == other)
                {
                    return true;
                }

                if (other.GetType() != typeof(Branch))
                {
                    return false;
                }

                return ToString().Equals(((Branch)other).ToString());
            }

            /**
             * Finish this branch by appending '0's until the maximum code Length has been reached.
             */
            public void finish()
            {
                while (builder.Length < MAX_LENGTH)
                {
                    builder.Append('0');
                    cachedstring = null;
                }
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            /**
             * Process the next replacement to be added to this branch.
             *
             * @param replacement
             *            the next replacement to append
             * @param forceAppend
             *            indicates if the default processing shall be overridden
             */
            public void processNextReplacement(string replacement, bool forceAppend)
            {
                bool append = lastReplacement == null || !lastReplacement.EndsWith(replacement) || forceAppend;

                if (append && builder.Length < MAX_LENGTH)
                {
                    builder.Append(replacement);
                    // remove all characters after the maximum Length
                    if (builder.Length > MAX_LENGTH)
                    {
                        builder.Remove(MAX_LENGTH, builder.Length);
                    }
                    cachedstring = null;
                }

                lastReplacement = replacement;
            }

            public override string ToString()
            {
                if (cachedstring == null)
                {
                    cachedstring = builder.ToString();
                }

                return cachedstring;
            }
        }

        /**
         * Inner class for storing rules.
         */
        internal class Rule
        {
            private string pattern;
            private char VERTICAL_BAR = '|';
            private string[] replacementAtStart;
            private string[] replacementBeforeVowel;
            private string[] replacementDefault;

            public Rule(string pattern, string replacementAtStart, string replacementBeforeVowel, string replacementDefault)
            {
                this.pattern = pattern;
                this.replacementAtStart = replacementAtStart.Split(VERTICAL_BAR);
                this.replacementBeforeVowel = replacementBeforeVowel.Split(VERTICAL_BAR);
                this.replacementDefault = replacementDefault.Split(VERTICAL_BAR);
            }

            public char getPattern(int pos)
            {
                return pattern[pos];
            }

            public int getPatternLength()
            {
                return pattern.Length;
            }

            public string[] getReplacements(string context, bool atStart)
            {
                if (atStart)
                {
                    return replacementAtStart;
                }

                int nextIndex = getPatternLength();
                bool nextCharIsVowel = nextIndex < context.Length && isVowel(context[nextIndex]);
                if (nextCharIsVowel)
                {
                    return replacementBeforeVowel;
                }

                return replacementDefault;
            }

            private bool isVowel(char ch)
            {
                return ch == 'a' || ch == 'e' || ch == 'i' || ch == 'o' || ch == 'u';
            }

            public bool matches(string context)
            {
                return context.StartsWith(pattern);
            }

            public override string ToString()
            {
                return string.Format("{0}=({1},{2},{3})", pattern, replacementAtStart.ToList<string>(), replacementBeforeVowel.ToList<string>(), replacementDefault.ToList<string>());
            }
        }
    }

}