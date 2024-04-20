using backend.database;
using backend.game;
using backend.signalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Collections;
using System.Diagnostics;

namespace backend.services
{
    internal class PlayerManager
    {
        public PlayerManager(UserManager<IdentityUser> usermanager, GameManager gameManager, IHubContext<PlayerHub> playerHubContext)
        {
            _usermanager = usermanager;
            _gameManager = gameManager;
            _playerHubContext = playerHubContext;
            IList<IdentityUser> users = _usermanager.Users.ToList();
        }

        public IEnumerable<IPlayer> Players => _identityToPlayerMapping.Values.ToList();

        public IPlayer GetPlayerByUserId(string userId)
        {
            IdentityUser? identity = _usermanager.FindByIdAsync(userId).Result;
            Debug.Assert(identity != null);

            IPlayer? player = _identityToPlayerMapping.GetValueOrDefault(identity);

            if (player == null)
            {
                player = new Player(_gameManager, _playerHubContext);
            }

            return player;
        }

        private readonly UserManager<IdentityUser> _usermanager;
        private readonly GameManager _gameManager;
        private readonly IHubContext<PlayerHub> _playerHubContext;
        private readonly Dictionary<IdentityUser, IPlayer> _identityToPlayerMapping = new Dictionary<IdentityUser, IPlayer>();
    }
}
