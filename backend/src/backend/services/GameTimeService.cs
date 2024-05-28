using System.Diagnostics;

namespace backend.services
{
    public class GameTimeService
    {
        public GameTimeService()
        {
            _moveTimers = new Dictionary<string, Stopwatch>();
            _playerMoveTimes = new Dictionary<string, List<double>>();
        }

        public void UpdatePlayerTime(string playerId, double elapsedTimeInSeconds)
        {
            if (_playerMoveTimes.ContainsKey(playerId))
            {
                _playerMoveTimes[playerId].Add(elapsedTimeInSeconds);
            }
        }
        public double GetPlayerTime(string playerId)
        {
            if (_playerMoveTimes.ContainsKey(playerId))
            {
                double totalTime = 0;
                foreach (var time in _playerMoveTimes[playerId])
                {
                    totalTime += time;
                }
                return totalTime;
            }
            else
            {
                return 0;
            }
        }
        public double GetTotalGameTime()
        {
            double totalGameTime = 0;
            foreach (var playerMoveTimes in _playerMoveTimes.Values)
            {
                totalGameTime += playerMoveTimes.Sum();
            }
            return totalGameTime;
        }
        public double GetTotalPlayerTime(string playerId)
        {
            if (_playerMoveTimes.ContainsKey(playerId))
            {
                return _playerMoveTimes[playerId].Sum();
            }
            else
            {
                return 0;
            }
        }
        public void StartMoveTimer(string playerId)
        {
            if (!_moveTimers.ContainsKey(playerId))
            {
                _moveTimers[playerId] = Stopwatch.StartNew();
            }
            else
            {
                _moveTimers[playerId].Reset();
                _moveTimers[playerId].Start();
            }
        }
        public void StopMoveTimer(string playerId)
        {
            if (_moveTimers.ContainsKey(playerId))
            {
                _moveTimers[playerId].Stop();
                double elapsedSeconds = _moveTimers[playerId].Elapsed.TotalSeconds;
                UpdatePlayerTime(playerId, elapsedSeconds);
            }
            else
            {
                throw new InvalidOperationException($"No move timer found for player with ID: {playerId}");
            }
        }

        private readonly Dictionary<string, Stopwatch> _moveTimers;
        private readonly Dictionary<string, List<double>> _playerMoveTimes;
    }
}
