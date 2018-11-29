using System;
using System.Security.Cryptography;
using System.Text;

#if NETCOREAPP2_1
namespace Tasks.Infrastructure.Server.Library

#else
namespace Tasks.Infrastructure.Client.Library

#endif
{
    /// <summary>
    ///     A key that identifies a session. Session keys should be synchronized accross clients to group the results of a task correctly.
    /// </summary>
    public struct SessionKey
    {
        /// <summary>
        ///     Initialize a new instance of <see cref="SessionKey"/> using a byte array
        /// </summary>
        /// <param name="hash">The <see cref="MD5"/> hash value</param>
        public SessionKey(byte[] hash)
        {
            Hash = BitConverter.ToString(hash).Replace("-", null);
        }

        /// <summary>
        ///     The hash of the session key
        /// </summary>
        public string Hash { get; }

        /// <summary>
        ///     Create a new <see cref="SessionKey"/> using a string
        /// </summary>
        /// <param name="s">The string that should identify the session</param>
        /// <returns>Return the <see cref="SessionKey"/> that identifies the session using the string parameter.</returns>
        public static SessionKey Create(string s)
        {
            using (var md5 = MD5.Create())
            {
                return new SessionKey(md5.ComputeHash(Encoding.UTF8.GetBytes(s)));
            }
        }

        /// <summary>
        ///     Create a new <see cref="SessionKey"/> using a name and a <see cref="DateTimeOffset"/>
        /// </summary>
        /// <param name="name">The name of the session</param>
        /// <param name="dateTimeOffset">The date time offset that should be merged to the name</param>
        /// <returns>Return the <see cref="SessionKey"/> that identifies the session using a name and a time.</returns>
        public static SessionKey Create(string name, DateTimeOffset dateTimeOffset)
        {
            if (dateTimeOffset.Offset != TimeSpan.Zero)
                dateTimeOffset = dateTimeOffset.ToUniversalTime();

            using (var md5 = MD5.Create())
            {
                return new SessionKey(md5.ComputeHash(Encoding.UTF8.GetBytes($"{name}_{dateTimeOffset:O}")));
            }
        }

        /// <summary>
        ///     Create a new <see cref="SessionKey"/> using a name and the current time
        /// </summary>
        /// <param name="name">The name of the session</param>
        /// <returns>Return the <see cref="SessionKey"/> that identifies the session using a name and the current time.</returns>
        public static SessionKey CreateUtcNow(string name) => Create(name, DateTimeOffset.UtcNow);

        /// <summary>
        ///     Initialize a <see cref="SessionKey"/> using an MD5 hash string (hex)
        /// </summary>
        /// <param name="hashString">The hexadecimal string</param>
        /// <returns>Return the <see cref="SessionKey"/> of the hash.</returns>
        public static SessionKey FromHash(string hashString)
        {
            var hash = Orcus.Server.Connection.Hash.Parse(hashString);
            if (hash.HashData.Length != 16)
                throw new ArgumentException("A 128 bit hash is required.", nameof(hashString));

            return new SessionKey(hash.HashData);
        }
    }
}