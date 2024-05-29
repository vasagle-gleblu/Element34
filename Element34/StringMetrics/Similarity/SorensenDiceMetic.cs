using ADOX;
using System.Collections.Generic;
using System.Linq;

namespace Element34.StringMetrics.Similarity
{
    /// <summary>
    /// Sørensen-Dice coefficient:
    /// The Sørensen–Dice coefficient is a statistic used to gauge the similarity of two samples. 
    /// It was independently developed by the botanists Thorvald Sørensen and Lee Raymond Dice, 
    /// who published in 1948 and 1945 respectively.
    /// 
    /// <a href="https://en.wikipedia.org/wiki/S%C3%B8rensen%E2%80%93Dice_coefficient" /> 
    /// </summary>
    public class SorensenDiceMetric : StringMetric<double?>
    {
        private static int N { get; set; }

        public SorensenDiceMetric() : this(1)
        { 
        }

        public SorensenDiceMetric(int n)
        {
            N = n;
        }

        public override double? Compute(char[] a, char[] b)
        {
            if (N <= 0 || a.Length < N || b.Length < N)
                return null;
            else if (a.SequenceEqual(b))
                return 1d;
            else
            {
                NGramTokenizer tokenizer = new NGramTokenizer(N);
                List<string> ca1bg = tokenizer.Tokenizer(new string(a));
                List<string> ca2bg = tokenizer.Tokenizer(new string(b));

                int ms = ScoreMatches(ca1bg, ca2bg);

                return (2d * ms) / (ca1bg.Count + ca2bg.Count);
            }
        }

        public override double? Compute(string a, string b) => Compute(a.ToCharArray(), b.ToCharArray());

        private static int ScoreMatches(List<string> ca1bg, List<string> ca2bg)
        {
            return ca1bg.Intersect(ca2bg).Count();
        }
    }
}
