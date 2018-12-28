using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Maze.Sockets.Internal;

namespace Maze.Server.MazeSockets.Internal
{
    internal static class HandshakeHelpers
    {
        private static readonly IReadOnlyDictionary<string, string> _headers =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {MazeSocketHeaders.Upgrade, MazeSocketHeaders.UpgradeSocket},
                {MazeSocketHeaders.Connection, MazeSocketHeaders.ConnectionUpgrade},
                {MazeSocketHeaders.SecWebSocketVersion, MazeSocketHeaders.SupportedVersion},
                {MazeSocketHeaders.SecWebSocketKey, null}
            };

        /// <summary>
        ///     Gets request headers needed process the handshake on the server.
        /// </summary>
        public static readonly IEnumerable<string> NeededHeaders = _headers.Keys;

        public static bool CheckSupportedRequest(IEnumerable<KeyValuePair<string, string>> headers)
        {
            var validation = _headers.ToDictionary(x => x.Key, x => false);

            foreach (var header in headers)
                if (string.Equals(header.Key, MazeSocketHeaders.SecWebSocketKey, StringComparison.OrdinalIgnoreCase))
                {
                    validation[header.Key] = IsRequestKeyValid(header.Value);
                }
                else
                {
                    if (string.Equals(_headers[header.Key], header.Value, StringComparison.OrdinalIgnoreCase))
                        validation[header.Key] = true;
                }

            return validation.All(x => x.Value);
        }

        /// <summary>
        ///     Validates the Sec-WebSocket-Key request header
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsRequestKeyValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            try
            {
                var data = Convert.FromBase64String(value);
                return data.Length == 16;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string CreateResponseKey(string requestKey)
        {
            // "The value of this header field is constructed by concatenating /key/, defined above in step 4
            // in Section 4.2.2, with the string "258EAFA5- E914-47DA-95CA-C5AB0DC85B11", taking the SHA-1 hash of
            // this concatenated value to obtain a 20-byte value and base64-encoding"
            // https://tools.ietf.org/html/rfc6455#section-4.2.2

            if (requestKey == null) throw new ArgumentNullException(nameof(requestKey));

            using (var algorithm = SHA1.Create())
            {
                var merged = requestKey + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                byte[] mergedBytes = Encoding.UTF8.GetBytes(merged);
                byte[] hashedBytes = algorithm.ComputeHash(mergedBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> GenerateResponseHeaders(string key)
        {
            yield return new KeyValuePair<string, string>(MazeSocketHeaders.Connection, MazeSocketHeaders.ConnectionUpgrade);
            yield return new KeyValuePair<string, string>(MazeSocketHeaders.Upgrade, MazeSocketHeaders.UpgradeSocket);
            yield return new KeyValuePair<string, string>(MazeSocketHeaders.SecWebSocketAccept, CreateResponseKey(key));
        }
    }
}