using System.Text;

namespace Element34.StringMetrics
{
    public interface IStringEncoder
    {
        string Encode(string source);
    }

    public interface IStringMetaphoneEncoder
    {
        void Encode(string source);

        string PrimaryKey { get; }

        string AlternateKey { get; }

        string Word { get; }
    }
}
