namespace Element34.StringMetrics.Similarity
{
/// <summary>
/// Something, something...
/// </summary>

    public abstract class StringMetric<A>
    {
        public abstract A Compute(char[] a, char[] b);

        public abstract A Compute(string a, string b);

    }
}
