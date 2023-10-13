namespace System
{
    public static class MathExt
    {
        public static SByte Max(params SByte[] inputs)
        {
            SByte highest = inputs[0];

            foreach (SByte input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static SByte Min(params SByte[] inputs)
        {
            SByte lowest = inputs[0];

            foreach (SByte input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Byte Max(params Byte[] inputs)
        {
            Byte highest = inputs[0];

            foreach (Byte input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Byte Min(params Byte[] inputs)
        {
            Byte lowest = inputs[0];

            foreach (Byte input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Int16 Max(params Int16[] inputs)
        {
            Int16 highest = inputs[0];

            foreach (Int16 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Int16 Min(params Int16[] inputs)
        {
            Int16 lowest = inputs[0];

            foreach (Int16 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Int32 Max(params Int32[] inputs)
        {
            Int32 highest = inputs[0];

            foreach (Int32 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Int32 Min(params Int32[] inputs)
        {
            Int32 lowest = inputs[0];

            foreach (Int32 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Int64 Max(params Int64[] inputs)
        {
            Int64 highest = inputs[0];

            foreach (Int64 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Int64 Min(params Int64[] inputs)
        {
            Int64 lowest = inputs[0];

            foreach (Int64 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static UInt16 Max(params UInt16[] inputs)
        {
            UInt16 highest = inputs[0];

            foreach (UInt16 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static UInt16 Min(params UInt16[] inputs)
        {
            UInt16 lowest = inputs[0];

            foreach (UInt16 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static UInt32 Max(params UInt32[] inputs)
        {
            UInt32 highest = inputs[0];

            foreach (UInt32 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static UInt32 Min(params UInt32[] inputs)
        {
            UInt32 lowest = inputs[0];

            foreach (UInt32 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static UInt64 Max(params UInt64[] inputs)
        {
            UInt64 highest = inputs[0];

            foreach (UInt64 input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static UInt64 Min(params UInt64[] inputs)
        {
            UInt64 lowest = inputs[0];

            foreach (UInt64 input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Single Max(params Single[] inputs)
        {
            Single highest = inputs[0];

            foreach (Single input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Single Min(params Single[] inputs)
        {
            Single lowest = inputs[0];

            foreach (Single input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Double Max(params Double[] inputs)
        {
            Double highest = inputs[0];

            foreach (Double input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Double Min(params Double[] inputs)
        {
            Double lowest = inputs[0];

            foreach (Double input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }

        public static Decimal Max(params Decimal[] inputs)
        {
            Decimal highest = inputs[0];

            foreach (Decimal input in inputs)
                if (input > highest) highest = input;

            return highest;
        }

        public static Decimal Min(params Decimal[] inputs)
        {
            Decimal lowest = inputs[0];

            foreach (Decimal input in inputs)
                if (input < lowest) lowest = input;

            return lowest;
        }
    }
}
