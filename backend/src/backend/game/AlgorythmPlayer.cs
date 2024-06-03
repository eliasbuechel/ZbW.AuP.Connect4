using backend.communication.signalR;
using backend.game.entities;
using backend.services;

namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(IPlayer opponentPlayer, GameManager gameManager) : base(Guid.NewGuid().ToString(), "Algorythm player", gameManager)
        {
            _opponentPlayer = opponentPlayer;
        }

        public IPlayer OpponentPlayer => _opponentPlayer;

        public override async void RequestedMatch(IPlayer player)
        {
            if (OpponentPlayer is WebPlayer)
            {
                base.RequestedMatch(player);
                await AcceptMatch(player);
            }
        }

        public override void GameStarted(Game connect4Game)
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
        private readonly IPlayer _opponentPlayer;
    }
}
