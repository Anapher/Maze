using System;

namespace Orcus.Server.Connection.Extensions
{
    /// <summary>
    ///     Extensions for <see cref="string" />
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Convert a hex string to a byte array
        /// </summary>
        /// <param name="source">The hex string</param>
        /// <returns>Return the byte array from the hex string</returns>
        public static byte[] ToByteArray(this string source)
        {
            if (source.Length % 2 == 1)
                throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(source));

            int GetHexVal(char hex)
            {
                var isHex = hex >= '0' && hex <= '9' ||
                            hex >= 'a' && hex <= 'f' ||
                            hex >= 'A' && hex <= 'F';
                if (!isHex)
                    throw new ArgumentException($"The char '{hex}' is not a valid hexadecimal character.",
                        nameof(source));

                return hex - (hex < 58 ? 48 : (hex < 97 ? 55 : 87));
            }

            var arr = new byte[source.Length >> 1];
            for (var i = 0; i < source.Length >> 1; ++i)
                arr[i] = (byte) ((GetHexVal(source[i << 1]) << 4) + GetHexVal(source[(i << 1) + 1]));

            return arr;
        }

        /// <summary>
        ///     Format the given bytes
        /// </summary>
        /// <param name="byteCount">The bytes to format</param>
        /// <returns>Return readable string</returns>
        //http://www.somacon.com/p576.php
        public static string BytesToString(long byteCount)
        {
            // Get absolute value
            long absolute = byteCount < 0 ? -1 * byteCount : byteCount;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute >= 0x1000000000000000) // Exabyte
            {
                suffix = "EiB";
                readable = byteCount >> 50;
            }
            else if (absolute >= 0x4000000000000) // Petabyte
            {
                suffix = "PiB";
                readable = byteCount >> 40;
            }
            else if (absolute >= 0x10000000000) // Terabyte
            {
                suffix = "TiB";
                readable = byteCount >> 30;
            }
            else if (absolute >= 0x40000000) // Gigabyte
            {
                suffix = "GiB";
                readable = byteCount >> 20;
            }
            else if (absolute >= 0x100000) // Megabyte
            {
                suffix = "MiB";
                readable = byteCount >> 10;
            }
            else if (absolute >= 0x400) // Kilobyte
            {
                suffix = "KiB";
                readable = byteCount;
            }
            else
            {
                return byteCount.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = readable / 1024;
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }
    }
}