using backend.game;

namespace backend.services.player
{
    internal class OpponentRoboterPlayerConnectionManager : PlayerConnectionManager<OpponentRoboterPlayer, string>
    {
        protected override OpponentRoboterPlayer GetOrCreatePlayer(string hubUrl)
        {
            OpponentRoboterPlayer? player = GetConnectedPlayerOrDefault(hubUrl);

            if (player == null)
            {
                player = new OpponentRoboterPlayer(hubUrl);
                _connectedPlayersAndIdentification.Add(new Tuple<OpponentRoboterPlayer, string>(player, hubUrl));
            }

            return player;
        }
        protected override bool IdentificationsAreEqal(string identification1, string identification2)
        {
            return identification1 == identification2;
        }
        protected override void OnDispose()
        {
        }
    }
}