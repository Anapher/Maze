using System;
using System.Security.Cryptography;
using System.Text;

namespace Tasks.Infrastructure.Server.Library
{
    public struct SessionKey
    {
        public SessionKey(byte[] hash)
        {
            Hash = BitConverter.ToString(hash).Replace("-", null);
        }

        public string Hash { get; }

        public static SessionKey Create(string s)
        {
            using (var md5 = MD5.Create())
            {
                return new SessionKey(md5.ComputeHash(Encoding.UTF8.GetBytes(s)));
            }
        }

        public static SessionKey Create(string name, DateTimeOffset dateTimeOffset)
        {
            using (var md5 = MD5.Create())
            {
                return new SessionKey(md5.ComputeHash(Encoding.UTF8.GetBytes($"{name}_{dateTimeOffset:O}")));
            }
        }

        public static SessionKey FromHash(string hashString)
        {
            var hash = Orcus.Server.Connection.Hash.Parse(hashString);
            if (hash.HashData.Length != 16)
                throw new ArgumentException("A 128 bit hash is required.", nameof(hashString));

            return new SessionKey(hash.HashData);
        }
    }
}