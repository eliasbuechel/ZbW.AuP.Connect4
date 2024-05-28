using backend.Data.entities;

namespace backend.game.entities
{
    internal class GameResultMatch : Entity
    {
        public GameResultMatch(Match inGameMatch) : base(inGameMatch)
        {
            Player1 = new PlayerInfo(inGameMatch.Player1);
            Player2 = new PlayerInfo(inGameMatch.Player2);
        }
        public GameResultMatch(DbGameResultMatch match) : base(match)
        {
            Player1 = new PlayerInfo(match.Player1);
            Player2 = new PlayerInfo(match.Player2);
        }

        public PlayerInfo Player1 { get; }
        public PlayerInfo Player2 { get; }
    }

    internal class Match : Entity
    {
        public Match(MatchRequest matchRequest) : base(matchRequest)
        {
            Player1 = matchRequest.Requester;
            Player2 = matchRequest.Opponent;
        }

        public IPlayer Player1 { get; }
        public IPlayer Player2 { get; }

        
    }
}
