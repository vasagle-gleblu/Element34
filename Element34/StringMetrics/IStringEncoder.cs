namespace Element34.StringMetrics
{
    public interface IStringMetaphoneEncoder<A>
    {
        void Encode (A source);

        A PrimaryKey { get; }

        A AlternateKey { get; }

        A Word { get; }
    }

    public interface IStringEncoder<A>
    {
        A Encode(A source);
    }
}
