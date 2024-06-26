using backend.game;
using backend.game.entities;
using backend.Infrastructure;
using backend.utilities;
using System.Diagnostics;

namespace backend.communication.mqtt
{
    internal class MQTTRoboterAPI : RoboterAPI
    {
        public MQTTRoboterAPI(MQTTNetTopicClient mqttTopicClient)
        {
            _mqttTopicClient = mqttTopicClient;
            Connect();
        }

        public override async void ResetConnect4Board()
        {
            StartRequestTimeout(async () =>
            {
                string originalTopicValue = false.ToString();
                //await _mqttTopicClient.PublishAsync(TOPIC_RESET, originalTopicValue);
                try
                {
                    await TopicResetChanged(originalTopicValue);
                }
                catch (InvalidPlayerRequestException)
                { }
            });

            _resettingBoard = true;
            if (_mqttTopicClient.IsConnected)
            {
                await _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString());
                return;
            }

            _mqttTopicClient.OnConnected += RequestConnect4BoardReset;
        }

        protected override async void PlaceStoneOnApi(Player player, Field field)
        {
            StartRequestTimeout(async () =>
            {
                _placingField = null;
                _placingPlayer = null;

                //await _mqttTopicClient.PublishAsync(TOPIC_COLUMN, "-1");
                try
                {
                    StonePlaced(player, field);
                }
                catch (InvalidPlayerRequestException)
                { }
            });

            Debug.Assert(_placingPlayer == null && _placingField == null);

            _placingPlayer = player;
            _placingField = field;
            await _mqttTopicClient.PublishAsync(TOPIC_COLUMN, field.Column.ToString());
        }
        protected override async void OnDispose()
        {
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_COLUMN, TopicColumnChanged);
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_RESET, TopicResetChanged);
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
            await _mqttTopicClient.DisconnectAsync();
        }

        private async void Connect()
        {
            await _mqttTopicClient.SubscribeToAsync(TOPIC_COLUMN, TopicColumnChanged);
            await _mqttTopicClient.SubscribeToAsync(TOPIC_RESET, TopicResetChanged);
            await _mqttTopicClient.SubscribeToAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
            await _mqttTopicClient.ConnectAsync();
        }
        private async void RequestConnect4BoardReset()
        {
            await _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString());
            _mqttTopicClient.OnConnected -= RequestConnect4BoardReset;
        }
        private Task TopicColumnChanged(string value)
        {
            int column;
            try
            {
                column = Convert.ToInt32(value);
            }
            catch
            {
                Debug.Assert(false);
                return Task.CompletedTask;
            }

            if (column == -1)
            {
                if (_placingField == null)
                {
                    //Debug.Assert(false);
                    return Task.CompletedTask;
                }
                if (_placingPlayer == null)
                {
                    //Debug.Assert(false);
                    return Task.CompletedTask;
                }
                Field placingField = _placingField;
                Player placingPlayer = _placingPlayer;

                _placingField = null;
                _placingPlayer = null;

                StonePlaced(placingPlayer, placingField);
                _currentRequestId = Guid.Empty;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
        private Task TopicResetChanged(string value)
        {
            bool resetValue;
            try
            {
                resetValue = Convert.ToBoolean(value);
            }
            catch
            {
                Debug.Assert(false);
                return Task.CompletedTask;
            }

            if (resetValue)
                return Task.CompletedTask;

            if (!_resettingBoard)
            {
                Debug.Assert(true);
                return Task.CompletedTask;
            }

            BoardReset();
            _currentRequestId = Guid.Empty;
            _resettingBoard = false;
            return Task.CompletedTask;
        }
        private async Task TopicManualColumnChanged(string value)
        {
            int column;
            try
            {
                column = Convert.ToInt32(value);
            }
            catch
            {
                Debug.Assert(false);
                return;
            }

            if (column >= 0 && column <= 6)
            {
                await _mqttTopicClient.PublishAsync(TOPIC_MANUAL_COLUMN, "-1");
                try
                {
                    ManualMove(column);
                }
                catch (InvalidPlayerRequestException e)
                {
                    Logger.Log(LogLevel.Warning, LogContext.MQTT_CLIENT, "Not able to process manual move from roboter.", e);
                }
            }

            return;
        }
        private void StartRequestTimeout(Action onTimeout)
        {
            Guid requestId = Guid.NewGuid();
            lock (_lock)
            {
                _currentRequestId = requestId;
            }

            Thread thread = new(async () =>
            {
                await Task.Delay(TIMOUT_TIME_IN_MS);
                lock(_lock)
                {
                    if (_currentRequestId == requestId)
                        onTimeout();
                }
            });
            thread.Start();
        }

        private const int TIMOUT_TIME_IN_MS = 500;
        private const string TOPIC_COLUMN = "column";
        private const string TOPIC_RESET = "reset";
        private const string TOPIC_MANUAL_COLUMN = "manualColumn";

        private readonly object _lock = new();
        private bool _resettingBoard;
        private Field? _placingField;
        private Player? _placingPlayer;
        private readonly MQTTNetTopicClient _mqttTopicClient;
        private Guid _currentRequestId = Guid.Empty;
    }
}