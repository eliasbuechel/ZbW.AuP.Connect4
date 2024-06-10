namespace backend.game
{
    internal class AlgorythmPlayer : Player
    {
        public AlgorythmPlayer(Player opponentPlayer) : base(Guid.NewGuid().ToString(), "R4D4-Algorythm")
        {
            OpponentPlayer = opponentPlayer;
        }

        public Player OpponentPlayer { get; }
    }

    //internal class AlgorythmPlayer : Player
    //{
    //    public AlgorythmPlayer(Player opponentPlayer, GameManager gameManager) : base(Guid.NewGuid().ToString(), "R4D4-Algorythm", gameManager)
    //    {
    //        _opponentPlayer = opponentPlayer;
    //    }

    //    public Player OpponentPlayer => _opponentPlayer;

    //    public override void RequestedMatch(Player player)
    //    {
    //        AcceptMatch(OpponentPlayer);
    //    }
    //    public override void RejectedMatch(Player player) { }
    //    public override void Matched(Match match) { }
    //    public override void GameStarted(Game connect4Game)
    //    {
    //        _startingPlayer = connect4Game.ActivePlayer;
    //        ConfirmGameStart();
    //    }
    //    public override void ConfirmedGameStart(Player player)
    //    {
    //        if (_startingPlayer != this)
    //            return;

    //        PlaceBestStone();
    //    }
    //    public override void MovePlayed(Player player, Field field)
    //    {
    //        if (player == this)
    //            return;

    //        PlaceBestStone();
    //    }

    //    private void PlaceBestStone()
    //    {
    //        Thread thread = new Thread(() =>
    //        {
    //            int bestMove = _gameManager.GetBestMove(this);
    //            PlayMove(bestMove);
    //        });
    //        thread.Start();
    //    }

    //    private Player? _startingPlayer;
    //    private readonly Player _opponentPlayer;
    //}
}
