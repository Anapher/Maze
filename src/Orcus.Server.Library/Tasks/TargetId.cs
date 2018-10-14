using System;

namespace Orcus.Server.Library.Tasks
{
    public struct TargetId
    {
        private readonly int _clientId;

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

        public static TargetId ServerId => new TargetId(true);

        public bool IsServer { get; }

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