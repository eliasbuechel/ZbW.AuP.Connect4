using backend.game;
using backend.game.entities;
using System.Diagnostics;

namespace backend.communication.mqtt
{
    internal class MQTTRoboterAPI : RoboterAPI
    {
        public MQTTRoboterAPI(MQTTNetTopicClient mqttTopicClient)
        {
            _mqttTopicClient = mqttTopicClient;

            _mqttTopicClient.ConnectAsync().Wait();
            _mqttTopicClient.SubscribeToAsync(TOPIC_COLUMN, TopicColumnChanged).Wait();
            _mqttTopicClient.SubscribeToAsync(TOPIC_RESET, TopicResetChanged).Wait();
            _mqttTopicClient.SubscribeToAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged).Wait();
        }
        ~MQTTRoboterAPI()
        {
            _mqttTopicClient.UnsubscribeFromAsync(TOPIC_COLUMN, TopicColumnChanged).Wait();
            _mqttTopicClient.UnsubscribeFromAsync(TOPIC_RESET, TopicResetChanged).Wait();
            _mqttTopicClient.UnsubscribeFromAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged).Wait();
            _mqttTopicClient.DisconnectAsync().Wait();
        }

        public override void ResetConnect4Board()
        {
            StartRequestTimeout(() =>
            {
                string originalTopicValue = false.ToString();
                TopicResetChanged(originalTopicValue);
                _mqttTopicClient.PublishAsync(TOPIC_RESET, originalTopicValue).Wait();
            });

            _resettingBoard = true;
            if (_mqttTopicClient.IsConnected)
            {
                _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString()).Wait();
                return;
            }

            _mqttTopicClient.OnConnected += RequestConnect4BoardReset;
        }

        protected override void PlaceStoneOnApi(Player player, Field field)
        {
            StartRequestTimeout(() =>
            {
                string originalTopicValue = "-1";
                TopicColumnChanged(originalTopicValue);
                _mqttTopicClient.PublishAsync(TOPIC_COLUMN, originalTopicValue).Wait();
            });

            Debug.Assert(_placingPlayer == null && _placingField == null);

            _placingPlayer = player;
            _placingField = field;
            _mqttTopicClient.PublishAsync(TOPIC_COLUMN, field.Column.ToString()).Wait();
        }

        private void RequestConnect4BoardReset()
        {
            _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString()).Wait();
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
        private Task TopicManualColumnChanged(string value)
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

            if (column >= 0 && column <= 6)
            {
                _mqttTopicClient.PublishAsync(TOPIC_MANUAL_COLUMN, "-1").Wait();
                ManualMove(column);
                _currentRequestId = Guid.Empty;
            }

            return Task.CompletedTask;
        }

        private void StartRequestTimeout(Action onTimeout)
        {
            Guid requestId = Guid.NewGuid();
            lock (_lock)
            {
                _currentRequestId = requestId;
            }

            Thread thread = new(() =>
            {
                Thread.Sleep(3000);
                lock(_lock)
                {
                    if (_currentRequestId == requestId)
                        onTimeout();
                }
            });
            thread.Start();
        }

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