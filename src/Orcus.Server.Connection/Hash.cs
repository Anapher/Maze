using System;
using System.Linq;
using Newtonsoft.Json;
using Orcus.Server.Connection.Extensions;
using Orcus.Server.Connection.JsonConverters;

namespace Orcus.Server.Connection
{
    /// <summary>
    ///     Represents a hash value
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(HashConverter))]
    public class Hash : IEquatable<Hash>
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="Hash" />
        /// </summary>
        /// <param name="hashData">The data of the hash</param>
        public Hash(byte[] hashData)
        {
            HashData = hashData;
        }

        /// <summary>
        ///     The data of the hash
        /// </summary>
        public byte[] HashData { get; }

        /// <summary>
        ///     True if the hash length matches the length of a SHA256 hash (32 bytes/256 bits)
        /// </summary>
        public bool IsSha256Hash => HashData.Length == 32;

        /// <summary>
        ///     Returns a value indicating whether this instance and a specified <see cref="Hash" /> object represent the same
        ///     value.
        /// </summary>
        /// <param name="other">A <see cref="Hash" /> to compare to this instance.</param>
        /// <returns>true if obj is equal to this instance; otherwise, false.</returns>
        public bool Equals(Hash other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return HashData.SequenceEqual(other.HashData);
        }

        /// <summary>
        ///     Parse a hex string
        /// </summary>
        /// <param name="value">The hex string which represents a hash value</param>
        /// <returns>Return</returns>
        public static Hash Parse(string value)
        {
            return new Hash(value.ToByteArray());
        }

        /// <summary>
        ///     Try parse a hex string
        /// </summary>
        /// <param name="value">The hex string which represents a hash value</param>
        /// <param name="hash">The result hash</param>
        /// <returns>True if the <see cref="value" /> was successfully parsed, else false.</returns>
        public static bool TryParse(string value, out Hash hash)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                hash = null;
                return false;
            }

            if (value.Length % 2 == 1)
            {
                hash = null;
                return false;
            }

            try
            {
                hash = new Hash(value.ToByteArray());
                return true;
            }
            catch (Exception)
            {
                hash = null;
                return false;
            }
        }

        /// <summary>
        ///     Convert the <see cref="HashData" /> to a hexadecimal string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BitConverter.ToString(HashData).Replace("-", null).ToLowerInvariant();
        }

        public static bool operator ==(Hash obj1, Hash obj2)
        {
            return Equals(obj1, obj2);
        }

        public static bool operator !=(Hash obj1, Hash obj2)
        {
            return !(obj1 == obj2);
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns>true if obj is equal to this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Hash) obj);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Hash" />.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 0;
                foreach (var b in HashData)
                    result = (result * 31) ^ b;
                return result;
            }
        }
    }
}