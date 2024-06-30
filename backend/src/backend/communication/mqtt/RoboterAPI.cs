using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using backend.utilities;

namespace backend.communication.mqtt
{
    internal abstract class RoboterApi : DisposingObject
    {
        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;
        public event Action<int>? OnManualMove;
        public event Action<Player, Field>? OnPlacingStone;

        public bool IsVisualizingOnRoboter { get; set; } = true;

        public void PlaceStone(Player player, Field field)
        {
            OnPlacingStone?.Invoke(player, field);
            if (IsVisualizingOnRoboter)
                PlaceStoneOnApi(player, field);

            StartRequestTimeout(IsVisualizingOnRoboter ? ROBOTER_MOVE_REQUEST_TIMOUT_TIME_IN_MS : TESTING_REQUEST_TIMOUT_TIME_IN_MS, () => OnStonePlaced?.Invoke(player, field));
        }
        public void ResetConnect4Board()
        {
            if (IsVisualizingOnRoboter)
                ResetConnect4BoardOnApi();

            StartRequestTimeout(IsVisualizingOnRoboter ? ROBOTER_RESET_REQUEST_TIMOUT_TIME_IN_MS : TESTING_REQUEST_TIMOUT_TIME_IN_MS , () => OnBoardReset?.Invoke());
        }

        protected abstract void ResetConnect4BoardOnApi();
        protected abstract void PlaceStoneOnApi(Player player, Field field);

        protected void StonePlaced(Player player, Field field)
        {
            try
            {
                OnStonePlaced?.Invoke(player, field);
            }
            catch (InvalidPlayerRequestException e)
            {
                Logger.Log(LogLevel.Warning, LogContext.ROBOTER_API, "Not able to process OnStonePlaced requst from roboter.", e);
            }
        }
        protected void BoardReset()
        {
            try
            {
                OnBoardReset?.Invoke();
            }
            catch (InvalidPlayerRequestException e)
            {
                Logger.Log(LogLevel.Warning, LogContext.ROBOTER_API, "Not able to process OnBoardReset requst from roboter.", e);
            }
        }
        protected void ManualMove(int column)
        {
            try
            {
                OnManualMove?.Invoke(column);
            }
            catch (InvalidPlayerRequestException e)
            {
                Logger.Log(LogLevel.Warning, LogContext.ROBOTER_API, "Not able to process OnManualMove request from roboter.", e);
            }
        }

        private void StartRequestTimeout(int timeoutDelay, Action onTimeout)
        {
            Guid requestId = Guid.NewGuid();
            lock (_lock)
            {
                _currentRequestId = requestId;
            }

            Thread thread = new(async () =>
            {
                await Task.Delay(timeoutDelay);
                lock (_lock)
                {
                    if (_currentRequestId == requestId)
                        onTimeout();
                }
            });
            thread.Start();
        }

        private readonly object _lock = new();
        private Guid _currentRequestId = Guid.Empty;
        private const int TESTING_REQUEST_TIMOUT_TIME_IN_MS = 500; // .5s
        private const int ROBOTER_MOVE_REQUEST_TIMOUT_TIME_IN_MS = 10000; // 10s
        private const int ROBOTER_RESET_REQUEST_TIMOUT_TIME_IN_MS = 60000; // 60s
    }
}