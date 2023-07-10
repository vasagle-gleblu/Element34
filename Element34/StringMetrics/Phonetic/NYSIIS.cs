using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System.Text.RegularExpressions;

namespace Element34.StringMetrics
{
    public class NYSIIS : IStringEncoder, IStringComparison
    {
        const int tokenLength = 6;

        public bool Compare(string value1, string value2)
        {
            NYSIIS nYSIIS = new NYSIIS();
            value1 = nYSIIS.Encode(value1);
            value2 = nYSIIS.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            string key = source.ToUpper();
            string[] suffix;

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
            if(suffix.Length > 0)
                key = key.Substring(0, suffix[0].Length);

            // remove all non-alpha characters
            key = Regex.Replace(key, @"[^A-Z]+", "");

            // BEGIN ALGORITHM *******************************************

            // Transcode first characters of name:
            //	   MAC -> MCC
            //	    KN -> NN
            //	     K -> C
            //	PH, PF -> FF
            //	   SCH -> SSS
            if (Regex.IsMatch(key, @"^MAC"))
                key = Regex.Replace(key, @"^MAC", "MCC");
            else if (Regex.IsMatch(key, @"^KN"))
                key = Regex.Replace(key, @"^KN", "NN");
            else if (Regex.IsMatch(key, @"^K"))
                key = Regex.Replace(key, @"^K", "C");
            else if (Regex.IsMatch(key, @"^PH|^PF"))
                key = Regex.Replace(key, @"^PH|^PF", "FF");
            else if (Regex.IsMatch(key, @"^SCH"))
                key = Regex.Replace(key, @"^SCH", "SSS");

            // Transcode two-character suffix as follows,
            //	            EE, IE -> Y
            //	DT, RT, RD, NT, ND -> D
            if (Regex.IsMatch(key, @"EE$|IE$"))
                key = Regex.Replace(key, @"EE$|IE$", "Y");
            else if (Regex.IsMatch(key, @"DT$|RT$|RD$|NT$|ND$"))
                key = Regex.Replace(key, @"DT$|RT$|RD$|NT$|ND$", "D");

            // Save first char for later, to be used as first char of key
            char firstChar = key[0];
            key = key.Substring(1);

            // Translate remaining characters by following these rules, incrementing by one character each time:
            //	EV	->	AF else
            if (Regex.IsMatch(key, @"EV"))
                key = Regex.Replace(key, @"EV", "AF");

            //  A,E,I,O,U	->	A
            key = Regex.Replace(key, @"[AEIOU]+", "A");

            //	Q	->	G
            key = Regex.Replace(key, @"Q", "G");
            //	Z	->	S
            key = Regex.Replace(key, @"Z", "S");
            //	M	->	N
            key = Regex.Replace(key, @"M", "N");
            //	KN	->	N, else K	->	C
            key = Regex.Replace(key, @"KN", "N");
            key = Regex.Replace(key, @"K", "C");
            //	SCH	->	SSS
            key = Regex.Replace(key, @"SCH", "SSS");
            //	PH	->	FF
            key = Regex.Replace(key, @"PH", "FF");
            //	H	->	If previous or next is non-vowel, previous
            key = Regex.Replace(key, @"([^AEIOU])H", "$1");
            key = Regex.Replace(key, @"(.)H[^AEIOU]", "$1");
            //	W 	->	If previous is vowel, previous
            key = Regex.Replace(key, @"[AEIOU]W", "A");

            // If last character is S, remove it
            key = Regex.Replace(key, @"S$", "");

            // If last characters are AY, replace with Y
            key = Regex.Replace(key, @"AY$", "Y");

            // If last character is A, remove it
            key = Regex.Replace(key, @"A$", "");

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

            // Use original first char of key as first char of key
            key = firstChar + key;

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
