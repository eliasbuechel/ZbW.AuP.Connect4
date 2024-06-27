using backend.data.entities;

namespace backend.game.entities
{
    internal class GameResultMatch
    {
        public GameResultMatch(Match inGameMatch)
        {
            Player1 = new PlayerInfo(inGameMatch.Player1);
            Player2 = new PlayerInfo(inGameMatch.Player2);
        }
        public GameResultMatch(DbGameResultMatch match)
        {
            Player1 = new PlayerInfo(match.Player1);
            Player2 = new PlayerInfo(match.Player2);
        }

        public PlayerInfo Player1 { get; }
        public PlayerInfo Player2 { get; }
    }
}
