using backend.database;
using System.Diagnostics;
using System.Collections.Generic;

namespace backend.signalR
{
    internal class PlayerRequestLock
    {
        public object this[PlayerIdentity identity]
        {
            get
            {
                lock (_lock)
                {
                    if (!_playerIdentityLockMap.ContainsKey(identity))
                        _playerIdentityLockMap.Add(identity, new object());

                    return _playerIdentityLockMap[identity];
                }
            }
            set
            {
                lock (_lock)
                {
                    Debug.Assert(_playerIdentityLockMap.ContainsKey(identity));
                    _playerIdentityLockMap[identity] = value;
                }
            }
        }

        private readonly Dictionary<PlayerIdentity, object> _playerIdentityLockMap = new Dictionary<PlayerIdentity, object>();
        private readonly object _lock = new object();
    }
}
