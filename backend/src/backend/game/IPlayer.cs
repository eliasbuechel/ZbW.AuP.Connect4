namespace backend.game
{
    internal interface IPlayer
    {
        event Action<IPlayer, int>? OnMovePlayed;
        event Action<IPlayer, IPlayer>? OnMatch;

        string Id { get; }
        string Username { get; }
        bool HasConfirmedGameStart { get; }

        void ConfirmGameStart();
        //void MakeMove(int column);
        void RequestMatch(IPlayer player);
        void RequestedMatch(IPlayer player);
        void RejectedMatch(IPlayer opponent);
        void PlayerConnected(IPlayer player);
        void PlayerDisconnected(IPlayer player);
        IEnumerable<IPlayer> GetOnlinePlayers();
        void Connected(string connectionId);
        void Disconnected(string connectionId);
        bool HasRequestedMatch(IPlayer you);
        bool HasMatched(IPlayer player);
        void AcceptMatch(IPlayer player);
        void Matched(IPlayer requester);
        void RejectMatch(IPlayer player);

        //void OnOpponentConfirmedGameStart();
        //void OnOpponentMadeMove(int column);
        //void OnErrorWhileMakingMove(string message);
        //void OnWon(Connect4Line connect4Line);
        //void OnLost(Connect4Line connect4Line);
        //void OnDraw();
        //void OnLostConnection();
        //void OnReconnected();
        //void OnOpponentLostconnection();
        //void OpponentReconnected();
        //void OnOpponentQuit();
        //void OnGameCreated(IPlayer player1, IPlayer player2);
    }
}