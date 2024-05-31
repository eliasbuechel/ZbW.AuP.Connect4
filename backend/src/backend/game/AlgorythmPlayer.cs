using backend.communication.DOTs;
using backend.communication.signalR;
using backend.game.entities;
using backend.services;

namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(IPlayer opponentPlayer, GameManager gameManager) : base(Guid.NewGuid().ToString(), "Algorythm", gameManager)
        {
            _opponentPlayer = opponentPlayer;
        }

        public IPlayer OpponentPlayer => _opponentPlayer;

        public override async void RequestedMatch(IPlayer player)
        {
            base.RequestedMatch(player);
            await AcceptMatchAsync(player);
        }

        public override void GameStarted(Connect4Game connect4Game)
        {
            base.GameStarted(connect4Game);
            _startingPlayer = connect4Game.ActivePlayer;
            ConfirmGameStartAsync();
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
                PlayMoveAsync(bestMove);
            });
            thread.Start();
        }

        private IPlayer? _startingPlayer;
        private readonly IPlayer _opponentPlayer;
    }
}
