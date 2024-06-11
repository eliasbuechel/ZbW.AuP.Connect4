using backend.game;
using backend.services.player;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterClientApiManager
    {
        public OpponentRoboterClientApiManager(Func<string, OpponentRoboterClientApi> createOpponentRoboterClientApi)
        {
            _createOpponentRoboterClientApi = createOpponentRoboterClientApi;
        }

        public event Action<OpponentRoboterClientApi>? OnCreated;

        public void Create(string hubUrl)
        {
            try
            {
                OpponentRoboterClientApi opponentRoboterClientApi = _createOpponentRoboterClientApi(hubUrl);
                OnCreated?.Invoke(opponentRoboterClientApi);
                _opponentRoboterClientApiDictionary.Add(hubUrl, opponentRoboterClientApi);
            }
            catch
            {
                return;
            }
        }
        public OpponentRoboterClientApi Get(string identification)
        {
            return _opponentRoboterClientApiDictionary[identification];
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