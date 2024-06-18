﻿using backend.game;

namespace backend.communication.DOTs
{
    internal class GameDTO
    {
        public GameDTO(Game connect4Game)
        {
            Match = new MatchDTO(connect4Game.Match);
            ActivePlayerId = connect4Game.ActivePlayer.Id;
            Connect4Board = connect4Game.FieldAsIds;
            MoveStartTime = new DateTimeOffset(connect4Game.MoveStartTime).ToUnixTimeMilliseconds();
        }

        public MatchDTO Match { get; }
        public string ActivePlayerId { get; }
        public string[][] Connect4Board { get; }
        public long MoveStartTime { get; }
    }
}