namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterClientApiManager
    {
        public OpponentRoboterClientApiManager(Func<string, OpponentRoboterClientApi> createOpponentRoboterClientApi)
        {
            _createOpponentRoboterClientApi = createOpponentRoboterClientApi;
        }
        public OpponentRoboterClientApi Create(string opponentRoboterPlayerApi)
        {
            OpponentRoboterClientApi opponentRoboterClientApi = _createOpponentRoboterClientApi(opponentRoboterPlayerApi);
            _opponentRoboterClientApiDictionary.Add(opponentRoboterPlayerApi, opponentRoboterClientApi);
            return opponentRoboterClientApi;
        }
        public OpponentRoboterClientApi Get(string opponentRoboterPlayerApi)
        {
            return _opponentRoboterClientApiDictionary[opponentRoboterPlayerApi];
        }

        public void ForEach(Action<OpponentRoboterClientApi> action)
        {
            foreach (OpponentRoboterClientApi opponentRoboterClientApi in _opponentRoboterClientApiDictionary.Values)
                action(opponentRoboterClientApi);
        }

        private readonly Func<string, OpponentRoboterClientApi> _createOpponentRoboterClientApi;
        private readonly Dictionary<string, OpponentRoboterClientApi> _opponentRoboterClientApiDictionary = new Dictionary<string, OpponentRoboterClientApi>();
    }
}