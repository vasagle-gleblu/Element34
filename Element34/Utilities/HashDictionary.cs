﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Element34.Utilities
{
    internal class HashDictionary<T> : Dictionary<byte[], T>
    {
        public HashDictionary() : base(new ByteArrayComparer())
        {
        }
    }

    internal class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }

            return left.SequenceEqual(right);
        }

        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return key.GetHashCode();
        }
    }

}