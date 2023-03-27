using System.Text;

namespace Element34.StringMetric
{
    public class DiceSorensenMetric : IStringEncoder, IStringComparison
    {
        public bool Compare(string value1, string value2)
        {
            Caverphone1 cav1 = new Caverphone1();
            value1 = cav1.Encode(value1);
            value2 = cav1.Encode(value2);

            return value1.Equals(value2);
        }

        public string Encode(string source)
        {
            StringBuilder result = new StringBuilder();
            return result.ToString();
        }
    }
}
