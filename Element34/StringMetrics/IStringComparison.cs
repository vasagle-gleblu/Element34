namespace Element34.StringMetrics
{
    public interface IStringComparison
    {
        bool Compare(string value1, string value2);
    }

    public interface IStringMetaphoneComparison
    {
        bool Compare(string word, string value1, string value2);
    }
}
