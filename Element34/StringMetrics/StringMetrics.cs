using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics
{
    public interface IMetric<A, B>
    {
        B Compare(A a, A b);
    }

    public interface IStringMetric<A> : IMetric<char[], A>
    {
        new A Compare(char[] a, char[] b);
    }

    public class StringMetric<A> : IStringMetric<A>
    {
        public A Compare(char[] a, char[] b)
        {

        }
    }

    public class MetricDecorator<A, B>
    {
        public IMetric<A, B> WithMemorization { get; }
        public Transform<A> WithTransform { get; }

        public MetricDecorator(IMetric<A, B> metric)
        {
            WithMemorization = new MemorizedMetric(metric);
            WithTransform = (Transform<A> transform) => new TransformedMetric(metric, transform);
        }
    }

    public sealed class StringMetricDecorator<A> : MetricDecorator<char[], A>
    {
        public StringMetricDecorator(StringMetric<A> sm) : base(sm) { }
    }

    public class StringMetricDecorator<A>
    {
        private readonly IStringMetric<A> innerMetric;

        public StringMetricDecorator(IStringMetric<A> metric)
        {
            innerMetric = metric;
        }

        public A Compare(string a, string b)
        {
            return innerMetric.Compare(a.ToCharArray(), b.ToCharArray());
        }
    }

    public class MemorizedMetric<A, B> : IMetric<A, B>
    {
        private readonly IMetric<A, B> baseMetric;
        private readonly Dictionary<(A, A), B> memo;

        public MemorizedMetric(IMetric<A, B> metric)
        {
            baseMetric = metric;
            memo = new Dictionary<(A, A), B>();
        }

        public B Compare(A a, A b)
        {
            var t = (a, b);

            if (memo.TryGetValue(t, out var result))
                return result;

            result = baseMetric.Compare(a, b);
            memo[t] = result;
            return result;
        }
    }

    public class TransformedMetric<A, B> : IMetric<A, B>
    {
        private readonly IMetric<A, B> baseMetric;
        private readonly Transform<A> transform;

        public TransformedMetric(IMetric<A, B> metric, Transform<A> transform)
        {
            baseMetric = metric;
            this.transform = transform;
        }

        public B Compare(A a, A b)
        {
            return baseMetric.Compare(transform(a), transform(b));
        }
    }

    public class Alphabet
    {
        public static HashSet<char> LowercaseConsonant = new HashSet<char> { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'z' };
        public static HashSet<char> UppercaseConsonant = new HashSet<char> { 'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Z' };
        public static HashSet<char> Consonant = new HashSet<char>(LowercaseConsonant.Concat(UppercaseConsonant));
        public static HashSet<char> LowercaseVowel = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };
        public static HashSet<char> UppercaseVowel = new HashSet<char> { 'A', 'E', 'I', 'O', 'U' };
        public static HashSet<char> Vowel = new HashSet<char>(LowercaseVowel.Concat(UppercaseVowel));
        public static HashSet<char> LowercaseY = new HashSet<char> { 'y' };
        public static HashSet<char> UppercaseY = new HashSet<char> { 'Y' };
        public static HashSet<char> Y = new HashSet<char>(LowercaseY.Concat(UppercaseY));
        public static HashSet<char> LowercaseAlpha = new HashSet<char>(LowercaseConsonant.Concat(LowercaseVowel).Concat(LowercaseY));
        public static HashSet<char> UppercaseAlpha = new HashSet<char>(UppercaseConsonant.Concat(UppercaseVowel).Concat(UppercaseY));
        public static HashSet<char> Alpha = new HashSet<char>(LowercaseAlpha.Concat(UppercaseAlpha));
    }

    public delegate A Transform<A>(A input);

    public static class Transform
    {
        private static readonly HashSet<char> Ascii = Enumerable.Range(0x00, 0x7F + 1).Select(x => (char)x).ToHashSet();
        private static readonly HashSet<char> ExtendedAscii = Enumerable.Range(0x80, 0xFF - 0x80 + 1).Select(x => (char)x).ToHashSet();
        private static readonly HashSet<char> Latin = Enumerable.Range(0x00, 0x24F + 1).Select(x => (char)x).ToHashSet();
        private static readonly HashSet<char> LowerCase = Enumerable.Range(0x61, 0x7A - 0x61 + 1).Select(x => (char)x).ToHashSet();
        private static readonly HashSet<char> Numbers = Enumerable.Range(0x30, 0x39 - 0x30 + 1).Select(x => (char)x).ToHashSet();
        private static readonly HashSet<char> UpperCase = Enumerable.Range(0x41, 0x5A - 0x41 + 1).Select(x => (char)x).ToHashSet();

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
