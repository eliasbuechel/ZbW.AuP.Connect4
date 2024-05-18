using backend.communication.DOTs;
using backend.services;

namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(string playerId, string username, GameManager gameManager) : base(playerId, username, gameManager)
        { }
        public override void RequestedMatch(IPlayer player)
        {
            AcceptMatch(player);
        }
        public override void GameStarted(Connect4Game connect4Game)
        {
            _connect4Game = connect4Game;
            _playingFirst = connect4Game.ActivePlayer == this;
            ConfirmGameStart();
        }
        public override void OpponentConfirmedGameStart()
        {
            if (!_playingFirst)
                return;


            
            PlayBestMove();
        }
        public override void MovePlayed(IPlayer player, Field field)
        {
            if (player == this)
                return;

            PlayBestMove();
        }

        private void PlayBestMove()
        {
            if (_connect4Game == null)
                return;

            Thread thread = new Thread(() =>
            {
                int bestMove = _connect4Game.GetBestMove();
                PlayMove(bestMove);
            });
            thread.Start();
        }

        private bool _playingFirst = false;
        private Connect4Game? _connect4Game;
    }
}
