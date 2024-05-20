using backend.communication.DOTs;
using backend.services;

namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(GameManager gameManager) : base(Guid.NewGuid().ToString(), "Algorythm", gameManager)
        {
        }

        public override void RequestedMatch(IPlayer player)
        {
            base.RequestedMatch(player);
            AcceptMatch(player);
        }

        public override void GameStarted(Connect4Game connect4Game)
        {
            base.GameStarted(connect4Game);
            _startingPlayer = connect4Game.ActivePlayer;
            ConfirmGameStart();
        }
        public override void GameStartConfirmed()
        {
            base.GameStartConfirmed();
            if (_startingPlayer != this)
                return;

            PlaceBestStone();
        }
        public override void MovePlayed(IPlayer player, Field field)
        {
            if (player == this)
                return;

            PlaceBestStone();
        }

        private void PlaceBestStone()
        {
            Thread thread = new Thread(() =>
            {
                int bestMove = _gameManager.GetBestMove(this);
                PlayMove(bestMove);
            });
            thread.Start();
        }

        private IPlayer? _startingPlayer; 
    }
}
