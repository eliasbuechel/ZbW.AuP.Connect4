using System.Diagnostics;

namespace backend.communication.signalR.opponentRoboterApi
{
    internal class OpponentRoboterClientApiManager(Func<string, OpponentRoboterClientApi> createOpponentRoboterClientApi)
    {
        public event Action<OpponentRoboterClientApi>? OnCreated;

        public void Create(string hubUrl)
        {
            OpponentRoboterClientApi opponentRoboterClientApi = _createOpponentRoboterClientApi(hubUrl);

            OnCreated?.Invoke(opponentRoboterClientApi);
            opponentRoboterClientApi.OnDisconnected += OnDisconnected;
            _opponentRoboterClientApiDictionary.Add(hubUrl, opponentRoboterClientApi);
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

        private void OnDisconnected(string callerUrl, string connectionId)
        {
            if (!_opponentRoboterClientApiDictionary.TryGetValue(callerUrl, out OpponentRoboterClientApi? opponentRoboterClientApi))
            {
                Debug.Assert(false);
                return;
            }
            opponentRoboterClientApi.OnDisconnected -= OnDisconnected;
            _opponentRoboterClientApiDictionary.Remove(callerUrl);
        }

        private readonly Func<string, OpponentRoboterClientApi> _createOpponentRoboterClientApi = createOpponentRoboterClientApi;
        private readonly Dictionary<string, OpponentRoboterClientApi> _opponentRoboterClientApiDictionary = [];
    }
}