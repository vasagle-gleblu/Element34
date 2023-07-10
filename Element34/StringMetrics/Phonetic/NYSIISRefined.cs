using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Text.RegularExpressions;

namespace Element34.StringMetrics
{
    public class NYSIISRefined : IStringEncoder, IStringComparison
    {
        const int tokenLength = 6;

        public bool Compare(string value1, string value2)
        {
            NYSIISRefined nYSIIS = new NYSIISRefined();
            value1 = nYSIIS.Encode(value1);
            value2 = nYSIIS.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            string key = source.ToUpper();
            string[] suffix;
            char firstChar = '\0';
            // ignore input strings with digits in it
            key = Regex.Replace(key, @"\d", "");

            // trim whitespace from right of string
            key = Regex.Replace(key, @"\s+$", "");

            // remove "JR", "SR", or Roman Numerals from the end of the name
            // (where "Roman Numerals" can be a malformed run of 'I' and 'V' chars)
            suffix = Regex.Split(key, @"\s+[JS]R$");
            if (suffix.Length > 0)
                key = key.Substring(0, suffix[0].Length);

            suffix = Regex.Split(key, @"\s+[VI]+$");
            if (suffix.Length > 0)
                key = key.Substring(0, suffix[0].Length);

            // remove all non-alpha characters
            key = Regex.Replace(key, @"[^A-Z]+", "");

            // BEGIN ALGORITHM *******************************************
            // Save first char for later
            // (if first char is a vowel, it is used as first char of code)
            // if the first character of the key is a vowel, remember it
            if (Regex.IsMatch(key, @"^[AEIOU]"))
            {
                firstChar = key[0];
            }

            // remove all 'S' and 'Z' chars from the end of the key
            if (Regex.IsMatch(key, @"[SZ]+$"))
            {
                key = Regex.Replace(key, @"[SZ]+$", "");
                if (key.Length == 0) key = "S";
            }

            // change initial MAC to MC and initial PF to F
            if (Regex.IsMatch(key, @"^MAC"))
                key = Regex.Replace(key, @"^MAC", "MC");
            else if (Regex.IsMatch(key, @"^PF"))
                key = Regex.Replace(key, @"^PF", "F");

            // Change two-character suffix as follows,
            //	                IX -> IC
            //	                EX -> EC
            //	        YE, EE, IE -> Y
            //	DT, RT, RD, NT, ND -> D
            if (Regex.IsMatch(key, @"IX$"))
                key = Regex.Replace(key, @"IX$", "IC");
            else if (Regex.IsMatch(key, @"EX$"))
                key = Regex.Replace(key, @"EX$", "EC");
            else if (Regex.IsMatch(key, @"YE$|EE$|IE$"))
                key = Regex.Replace(key, @"YE$|EE$|IE$", "Y");
            else
            {
                while (Regex.IsMatch(key, @"DT$|RT$|RD$|NT$|ND$"))
                {
                    key = Regex.Replace(key, @"DT$|RT$|RD$|NT$|ND$", "D");
                }
            }

            // Change 'EV' to 'EF' if not at start of name
            if (Regex.IsMatch(key, @"^EV"))
                key = "EV" + Regex.Replace(key.Substring(2), @"^EV", "EF");
            else
                key = Regex.Replace(key, @"EV", "EF");

            // Save first char for later
            // (if first char is a vowel, it is used as first char of code)
            if (firstChar == '\0')
                firstChar = key[0];

            // Remove any 'W' that follows a vowel
            key = Regex.Replace(key, @"([AEIOU])W", "A");

            // Replace all vowels with 'A' and collapse all strings of 'A' to one 'A'
            key = Regex.Replace(key, @"[AEIOU]+", "A");

            // Change 'GHT' to 'GT'
            key = Regex.Replace(key, @"GHT", "GT");

            // Change 'DG' to 'G'
            key = Regex.Replace(key, @"DG", "G");

            // Change 'PH' to 'F'
            key = Regex.Replace(key, @"PH", "F");

            // If not first character, eliminate all 'H' preceded or followed by a vowel
            if (Regex.IsMatch(key, @"^H"))
                key = "H" + Regex.Replace(key.Substring(1), @"AH|HA", "A");
            else
                key = Regex.Replace(key, @"AH|HA", "A");

            // Change 'KN' to 'N', else 'K' to 'C'
            key = Regex.Replace(key, @"KN", "N");
            key = Regex.Replace(key, @"K", "C");

            // If not first character, change 'M' to 'N'
            if (Regex.IsMatch(key, @"^M"))
                key = "M" + Regex.Replace(key.Substring(1), @"M", "N");
            else
                key = Regex.Replace(key, @"M", "N");

            // If not first character, change 'Q' to 'G'
            if (Regex.IsMatch(key, @"^Q"))
                key = "Q" + Regex.Replace(key.Substring(1), @"Q", "G");
            else
                key = Regex.Replace(key, @"Q", "G");

            // Change 'SH' to 'S'
            key = Regex.Replace(key, @"SH", "S");

            // Change 'SCH' to 'S'
            key = Regex.Replace(key, @"SCH", "S");

            // Change 'YW' to 'Y'
            key = Regex.Replace(key, @"YW", "Y");

            // If not first or last character, change 'Y' to 'A'
            if (Regex.IsMatch(key, @"^Y") && Regex.IsMatch(key, @"Y$"))
                key = "Y" + Regex.Replace(key.Substring(1, key.Length - 1), @"Y", "A") + "Y";
            else if (Regex.IsMatch(key, @"^Y"))
                key = "Y" + Regex.Replace(key.Substring(1), @"Y", "A");
            else if (Regex.IsMatch(key, @"Y$"))
                key = Regex.Replace(key.Substring(0, key.Length - 1), @"Y", "A") + "Y";
            else
                key = Regex.Replace(key, @"Y", "A");

            // Change 'WR' to 'R'
            key = Regex.Replace(key, @"WR", "R");

            // If not first character, change 'Z' to 'S'
            if (Regex.IsMatch(key, @"^Z"))
                key = "Z" + Regex.Replace(key.Substring(1), @"Z", "S");
            else
                key = Regex.Replace(key, @"Z", "S");

            // Change terminal 'AY' to 'Y'
            key = Regex.Replace(key, @"AY$", "Y");

            // remove trailing vowels
            key = Regex.Replace(key, @"A+$", "");

            // Collapse all strings of repeated characters
            // This is more brute force that it needs to be
            key = Regex.Replace(key, @"[AEIOU]+", "A");
            key = Regex.Replace(key, @"B+", "B");
            key = Regex.Replace(key, @"C+", "C");
            key = Regex.Replace(key, @"D+", "D");
            key = Regex.Replace(key, @"F+", "F");
            key = Regex.Replace(key, @"G+", "G");
            key = Regex.Replace(key, @"H+", "H");
            key = Regex.Replace(key, @"J+", "J");
            key = Regex.Replace(key, @"K+", "K");
            key = Regex.Replace(key, @"L+", "L");
            key = Regex.Replace(key, @"M+", "M");
            key = Regex.Replace(key, @"N+", "N");
            key = Regex.Replace(key, @"P+", "P");
            key = Regex.Replace(key, @"Q+", "Q");
            key = Regex.Replace(key, @"R+", "R");
            key = Regex.Replace(key, @"S+", "S");
            key = Regex.Replace(key, @"T+", "T");
            key = Regex.Replace(key, @"V+", "V");
            key = Regex.Replace(key, @"W+", "W");
            key = Regex.Replace(key, @"X+", "X");
            key = Regex.Replace(key, @"Y+", "Y");
            key = Regex.Replace(key, @"Z+", "Z");

            // if first char of original key is a vowel,
            // use it as first char of code (instead of transcoded 'A')
            if (Regex.IsMatch(firstChar.ToString(), @"^[AEIOU]"))
                key = Regex.Replace(key, @"^A*", firstChar.ToString());

            // Technically, the NYSIIS code is only 6 chars long, but curious
            // people want to see the rest of the resulting transcoding
            if (key.Length > tokenLength)
            {
                key = key.Substring(0, tokenLength) + "[" + key.Substring(tokenLength) + "]";
            }

            return key;
        }
    }
}
