namespace Element34.StringMetrics.Phonetic
{
    // Define the Algorithm interface
    public interface IAlgorithm<T>
    {
        T Encode(T t);
    }

    // Define the StringAlgorithm interface
    public interface IStringAlgorithm : IAlgorithm<char[]>
    {
        string Encode(string a);
    }

    // Implement the StringAlgorithm interface
    public abstract class StringAlgorithmBase : IStringAlgorithm
    {
        public abstract char[] Encode(char[] a);

        public string Encode(string a)
        {
            char[] result = Encode(a.ToCharArray());
            return result == null ? null : new string(result);
        }
    }

    // Define the StringAlgorithmDecorator class
    public class StringAlgorithmDecorator : IStringAlgorithm
    {
        private readonly IStringAlgorithm stringAlgorithm;

        public StringAlgorithmDecorator(IStringAlgorithm sa)
        {
            stringAlgorithm = sa;
        }

        public char[] Encode(char[] a) => stringAlgorithm.Encode(a);

        public string Encode(string a) => stringAlgorithm.Encode(a);
    }

    public static class StringAlgorithm
    {
        // Create instances of phonetic algorithms (MetaphoneAlgorithm, NysiisAlgorithm, RefinedNysiisAlgorithm, RefinedSoundexAlgorithm, etc...)

        private static readonly Caverphone1 cav1 = new Caverphone1();
        private static readonly Caverphone2 cav2 = new Caverphone2();
        private static readonly Metaphone mph1 = new Metaphone();
        private static readonly DoubleMetaphone mph2 = new DoubleMetaphone();
        private static readonly Metaphone3 mph3 = new Metaphone3();
        private static readonly NYSIIS nYSIIS = new NYSIIS();
        private static readonly NYSIISRefined nYSIISRef = new NYSIISRefined();
        private static readonly SoundEx sdx = new SoundEx();
        private static readonly SoundExDM sdxDM = new SoundExDM();
        private static readonly SoundExRefined sdxRef = new SoundExRefined();
        private static readonly SoundExReverse sdxRev = new SoundExReverse();

        public static char[] EncodeWithCaverphone1(char[] a) => cav1.Encode(a);
        public static char[] EncodeWithCaverphone2(char[] a) => cav2.Encode(a);
        public static char[] EncodeWithMetaphone(char[] a) => mph1.Encode(a);
        public static (char[], char[]) EncodeWithDoubleMetaphone(char[] a)
        {
            mph2.Encode(a);
            return (mph2.PrimaryKey.ToCharArray(), mph2.AlternateKey.ToCharArray());
        }
        public static (char[], char[]) EncodeWithMetaphone3(char[] a)
        {
            mph3.Encode(a);
            return (mph3.PrimaryKey.ToCharArray(), mph3.AlternateKey.ToCharArray());
        }
        public static char[] EncodeWithNYSIIS(char[] a) => nYSIIS.Encode(a);
        public static char[] EncodeWithRefinedNYSIIS(char[] a) => nYSIISRef.Encode(a);
        public static char[] EncodeWithSoundEx(char[] a) => sdx.Encode(a);
        public static char[] EncodeWithSoundExDM(char[] a) => sdxDM.Encode(a);
        public static char[] EncodeWithRefinedSoundEx(char[] a) => sdxRef.Encode(a);
        public static char[] EncodeWithReversedSoundEx(char[] a) => sdxRev.Encode(a);
    }
}

