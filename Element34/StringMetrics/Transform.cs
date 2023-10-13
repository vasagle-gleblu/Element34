using System;
using System.Linq;

namespace Element34.StringMetrics
{
    public delegate A Transform<A>(A input);

    public static class Transform
    {
        private static readonly char[] Ascii = Enumerable.Range(0x00, 0x7F + 1).Select(x => (char)x).ToArray();
        private static readonly char[] ExtendedAscii = Enumerable.Range(0x80, 0xFF - 0x80 + 1).Select(x => (char)x).ToArray();
        private static readonly char[] Latin = Enumerable.Range(0x00, 0x24F + 1).Select(x => (char)x).ToArray();
        private static readonly char[] LowerCase = Enumerable.Range(0x61, 0x7A - 0x61 + 1).Select(x => (char)x).ToArray();
        private static readonly char[] Numbers = Enumerable.Range(0x30, 0x39 - 0x30 + 1).Select(x => (char)x).ToArray();
        private static readonly char[] UpperCase = Enumerable.Range(0x41, 0x5A - 0x41 + 1).Select(x => (char)x).ToArray();

        private static string Filter(char[] ca, Func<char, bool> f)
        {
            return new string(ca.Where(c => f(c)).ToArray());
        }

        private static string FilterNot(char[] ca, Func<char, bool> f)
        {
            return new string(ca.Where(c => !f(c)).ToArray());
        }

        public static Transform<string> FilterAlpha => ca => Filter(ca.ToCharArray(), c => LowerCase.Contains(c) || UpperCase.Contains(c));

        public static Transform<string> FilterNotAlpha => ca => FilterNot(ca.ToCharArray(), c => LowerCase.Contains(c) || UpperCase.Contains(c));

        public static Transform<string> FilterAlphaNumeric => ca => Filter(ca.ToCharArray(), c => LowerCase.Contains(c) || UpperCase.Contains(c) || Numbers.Contains(c));

        public static Transform<string> FilterNotAlphaNumeric => ca => FilterNot(ca.ToCharArray(), c => LowerCase.Contains(c) || UpperCase.Contains(c) || Numbers.Contains(c));

        public static Transform<string> FilterAscii => ca => Filter(ca.ToCharArray(), c => Ascii.Contains(c));

        public static Transform<string> FilterNotAscii => ca => FilterNot(ca.ToCharArray(), c => Ascii.Contains(c));

        public static Transform<string> FilterExtendedAscii => ca => Filter(ca.ToCharArray(), c => ExtendedAscii.Contains(c));

        public static Transform<string> FilterNotExtendedAscii => ca => FilterNot(ca.ToCharArray(), c => ExtendedAscii.Contains(c));

        public static Transform<string> FilterLatin => ca => Filter(ca.ToCharArray(), c => Latin.Contains(c));

        public static Transform<string> FilterNotLatin => ca => FilterNot(ca.ToCharArray(), c => Latin.Contains(c));

        public static Transform<string> FilterLowerCase => ca => Filter(ca.ToCharArray(), c => LowerCase.Contains(c));

        public static Transform<string> FilterNotLowerCase => ca => FilterNot(ca.ToCharArray(), c => LowerCase.Contains(c));

        public static Transform<string> FilterNumeric => ca => Filter(ca.ToCharArray(), c => Numbers.Contains(c));

        public static Transform<string> FilterNotNumeric => ca => FilterNot(ca.ToCharArray(), c => Numbers.Contains(c));

        public static Transform<string> FilterUpperCase => ca => Filter(ca.ToCharArray(), c => UpperCase.Contains(c));

        public static Transform<string> FilterNotUpperCase => ca => FilterNot(ca.ToCharArray(), c => UpperCase.Contains(c));

        public static Transform<string> IgnoreAlphaCase => ca => new string(ca.Select(c => (c >= 65 && c <= 90) ? (char)(c + 32) : c).ToArray());
    }
}
