using System.Collections;

namespace backend.game
{
    internal abstract class Player
    {
        public Player(string playerId, string username)
        {
            Id = playerId;
            Username = username;
        }

        public string Id { get; }
        public string Username { get; }
        public bool HasConfirmedGameStart { get; set; }

        public readonly ICollection<Player> MatchingRequests = new List<Player>();
        public Player? Matching { get; set; }
    }
        //internal abstract class Player : DisposingObject
        //{
        //    public Player(string playerId, string username, GameManager gameManager)
        //    {
        //        Id = playerId;
        //        Username = username;
        //        _gameManager = gameManager;

        //        _gameManager.OnRequestedMatch += RequestedMatch;
        //        _gameManager.OnRejectedMatch += RejectedMatch;
        //        _gameManager.OnMatched += Matched;
        //        _gameManager.OnGameStarted += GameStarted;
        //        _gameManager.OnConfirmedGameStart += ConfirmedGameStart;
        //        _gameManager.OnMovePlayed += MovePlayed;
        //        _gameManager.OnQuitedGame += QuitGame;
        //    }

        //    public event Action<Player> OnRequestedMatch;
        //    public event Action<Player>? OnRejectedMatch;
        //    public event Action<Match>? OnMatched;
        //    public event Action<Game>? OnGameStarted;
        //    public event Action? OnConfirmedGameStart;
        //    public event Action<Field>? OnMovePlayed;
        //    public event Action? OnQuitGame;

        //    public string Id { get; }
        //    public string Username { get; }
        //    public bool HasConfirmedGameStart { get; set; }

        //    public void RequestMatch(Player opponent)
        //    {
        //        _gameManager.RequestMatch(this, opponent);
        //    }
        //    public void AcceptMatch(Player opponent)
        //    {
        //        _gameManager.AcceptMatch(this, opponent);
        //    }
        //    public void RejectMatch(Player opponent)
        //    {
        //        _gameManager.RejectMatch(this, opponent);
        //    }
        //    public void ConfirmGameStart()
        //    {
        //        _gameManager.ConfirmGameStart(this);
        //    }
        //    public void PlayMove(int column)
        //    {
        //        _gameManager.PlayMove(this, column);
        //    }
        //    public void QuitGame()
        //    {
        //        _gameManager.QuitGame(this);
        //    }

        //    protected void RequestedMatch(Player requester, Player opponent)
        //    {
        //        if (opponent != this)
        //            return;

        //        OnRequestedMatch?.Invoke(requester);
        //    }
        //    protected void RejectedMatch(Player rejector, Player opponent)
        //    {
        //        if (opponent != this)
        //            return;

        //        OnRejectedMatch?.Invoke(rejector);
        //    }
        //    protected void Matched(Match match)
        //    {
        //        if (match.Player1 != this && match.Player2 != this)
        //            return;

        //        OnMatched?.Invoke(match);
        //    }
        //    protected void GameStarted(Game game)
        //    {
        //        if (game.Match.Player1 != this && game.Match.Player2 != this)
        //            return;

        //        OnGameStarted?.Invoke(game);
        //    }
        //    protected void ConfirmedGameStart(Player player)
        //    {
        //        if (player == )
        //    }
        //    protected void MovePlayed(Player player, Field field);
        //    protected void QuitGame(Player player);

        //    protected override void OnDispose()
        //    {
        //        _gameManager.OnRequestedMatch -= OnRequestedMatch;
        //        _gameManager.OnRejectedMatch -= OnRejectedMatch;
        //        _gameManager.OnMatched -= OnMatched;
        //        _gameManager.OnGameStarted -= OnGameStarted;
        //        _gameManager.OnConfirmedGameStart -= OnConfirmedGameStart;
        //        _gameManager.OnMovePlayed -= OnMovePlayed;
        //        _gameManager.OnQuitedGame -= OnQuitGame;
        //    }

        //    protected readonly GameManager _gameManager;
}
