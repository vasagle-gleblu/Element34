namespace Element34.StringMetrics.Phonetic
{
    public interface IStringEncoder
    {
        char[] Encode(char[] buffer);

        string Encode(string source);
    }

    public interface IStringMetaphoneEncoder
    {
        void Encode(char[] buffer);

        void Encode(string source);

        string PrimaryKey { get; }

        string AlternateKey { get; }

        string Word { get; }
    }
}
