﻿using backend.communication.dtos;
using backend.game;
using backend.infrastructure;
using System.Diagnostics;

namespace backend.services.player
{
    internal class PlayerConnectionService : DisposingObject
    {
        public PlayerConnectionService(
            WebPlayerConnectionManager webPlayerConnectionManager,
            AlgorythmPlayerConnectionManager algorythmPlayerManager,
            OpponentRoboterPlayerConnectionManager opponentRoboterPlayerConnectionManager
            )
        {
            WebPlayerConnectionManager = webPlayerConnectionManager;
            AlgorythmPlayerConnectionManager = algorythmPlayerManager;
            OpponentRoboterPlayerConnectionManager = opponentRoboterPlayerConnectionManager;

            WebPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;
            OpponentRoboterPlayerConnectionManager.OnPlayerDisconnected += PlayerDisconnected;
        }

        public event Action<Player>? OnPlayerDisconnected;

        public WebPlayerConnectionManager WebPlayerConnectionManager { get; }
        public AlgorythmPlayerConnectionManager AlgorythmPlayerConnectionManager { get; }
        public OpponentRoboterPlayerConnectionManager OpponentRoboterPlayerConnectionManager { get; }

        public IEnumerable<Player> Players => WebPlayers.Concat<Player>(OpponentRoboterePlayers).Concat<Player>(AlgorythmPlayers);
        public IEnumerable<WebPlayer> WebPlayers => WebPlayerConnectionManager.ConnectedPlayers;
        public IEnumerable<OpponentRoboterPlayer> OpponentRoboterePlayers => OpponentRoboterPlayerConnectionManager.ConnectedPlayers;
        public IEnumerable<AlgorythmPlayer> AlgorythmPlayers => AlgorythmPlayerConnectionManager.ConnectedPlayers;

        public Player GetPlayer(string playerId)
        {
            foreach (var player in WebPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in OpponentRoboterPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

            foreach (var player in AlgorythmPlayerConnectionManager.ConnectedPlayers)
                if (player.Id == playerId)
                    return player;

           throw new ArgumentException($"The PlayerId {playerId} is not contained in the player connections.");
        }
        public Player? GetPlayerOrDefault(string playerId)
        {
            try
            {
                return GetPlayer(playerId);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        public ConnectedPlayersDto GetConnectedPlayersExcept(WebPlayer requestingWebPlayer)
        {
            IEnumerable<ConnectedPlayerDto> connectedWebPlayers = WebPlayers.Where(x => x.Id != requestingWebPlayer.Id).Select(x => new ConnectedPlayerDto(x, requestingWebPlayer));
            IEnumerable<ConnectedPlayerDto> connectedOpponentRoboterPlayers = OpponentRoboterePlayers.Where(x => x.Id != requestingWebPlayer.Id).Select(x =>
            {
                AlgorythmPlayer? algorythmPlayer = AlgorythmPlayerConnectionManager.GetConnectedPlayerByIdentificationOrDefault(x);
                if (algorythmPlayer == null)
                    return new ConnectedPlayerDto(x);

                return new ConnectedPlayerDto(x, algorythmPlayer);
            });

            ConnectedPlayersDto connectedPlayers = new(connectedWebPlayers, connectedOpponentRoboterPlayers);
            return connectedPlayers;
        }
        public void ForeachConnectedPlayer(Action<Player> action)
        {
            foreach (var player in WebPlayerConnectionManager.ConnectedPlayers)
                action(player);

            foreach (var player in OpponentRoboterPlayerConnectionManager.ConnectedPlayers)
                action(player);

            foreach (var player in AlgorythmPlayerConnectionManager.ConnectedPlayers)
                action(player);
        }

        private void PlayerDisconnected(Player player)
        {
            OnPlayerDisconnected?.Invoke(player);
        }

        protected override void OnDispose()
        {
            WebPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
            OpponentRoboterPlayerConnectionManager.OnPlayerDisconnected -= PlayerDisconnected;
        }

    }
}