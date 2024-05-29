using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Element34.Utilities
{
    /// <summary>
    /// Represents a dictionary that uses a byte array as the key.
    /// </summary>
    /// <remarks>
    /// This dictionary implementation allows byte arrays to be used as keys
    /// by providing a custom comparer that computes hash codes based on the
    /// contents of the byte arrays and compares byte arrays for equality.
    /// </remarks>
    /// <typeparam name="T">The type of the values in the dictionary.</typeparam>
    class HashDictionary<T> : Dictionary<byte[], T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HashDictionary{T}"/> class.
        /// </summary>
        public HashDictionary() : base(new ByteArrayComparer())
        {
        }
    }

    /// <summary>
    /// Provides a byte array comparer for dictionary keys.
    /// </summary>
    /// <remarks>
    /// This comparer uses a custom hash function to generate hash codes for
    /// byte arrays and compares two byte arrays for equality based on their
    /// contents rather than their references.
    /// </remarks>
    internal class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        /// <summary>
        /// Determines whether the specified byte arrays are equal.
        /// </summary>
        /// <param name="left">The first byte array to compare.</param>
        /// <param name="right">The second byte array to compare.</param>
        /// <returns>true if the specified byte arrays are equal; otherwise, false.</returns>
        public bool Equals(byte[] left, byte[] right)
        {
            if (left == null || right == null)
            {
                return left == right;
            }

            return left.SequenceEqual(right);
        }

        /// <summary>
        /// Returns a hash code for the specified byte array.
        /// </summary>
        /// <param name="key">The byte array for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified byte array.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the byte array is null.</exception>
        public int GetHashCode(byte[] key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                foreach (byte element in key)
                    hash = hash * 31 + element;
                return hash;
            }
        }
    }

}