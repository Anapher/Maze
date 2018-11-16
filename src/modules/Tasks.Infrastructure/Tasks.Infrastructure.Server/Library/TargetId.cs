using System;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     The identifier of a single target
    /// </summary>
    public struct TargetId
    {
        private readonly int _clientId;

        /// <summary>
        ///     Initialze a new instance of <see cref="TargetId"/> with a client
        /// </summary>
        /// <param name="clientId">The id of the client</param>
        public TargetId(int clientId)
        {
            _clientId = clientId;
            IsServer = false;
        }

        private TargetId(bool isServer)
        {
            IsServer = isServer;
            _clientId = 0;
        }

        /// <summary>
        ///     Get the identifier for the server
        /// </summary>
        public static TargetId ServerId => new TargetId(true);

        /// <summary>
        ///     True if this value identifies a server
        /// </summary>
        public bool IsServer { get; }

        /// <summary>
        ///     The client id if this identifier identifies a client
        /// </summary>
        public int ClientId
        {
            get
            {
                if (IsServer)
                    throw new InvalidOperationException("Target is server and has not client id");
                return _clientId;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TargetId id &&
                   IsServer == id.IsServer &&
                   (!IsServer && ClientId == id.ClientId);
        }

        public override int GetHashCode()
        {
            if (IsServer)
                return true.GetHashCode();

            return HashCode.Combine(IsServer, ClientId);
        }
    }
}