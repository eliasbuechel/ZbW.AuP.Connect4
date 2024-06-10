using backend.game;
using backend.game.entities;
using System.Diagnostics;

namespace backend.communication.mqtt
{
    internal class MQTTRoboterAPI : IRoboterAPI
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

        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;
        public event Action<int>? OnManualMove;

        public void PlaceStone(Player player, Field field)
        {
            _placingPlayer = player;
            _placingField = field;
            _mqttTopicClient.PublishAsync(TOPIC_COLUMN, field.Column.ToString()).Wait();
        }
        public void ResetConnect4Board()
        {
            _resettingBoard = true;
            if (_mqttTopicClient.IsConnected)
            {
                _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString()).Wait();
                return;
            }

            _mqttTopicClient.OnConnected += RequestConnect4BoardReset;
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

                OnStonePlaced?.Invoke(_placingPlayer, _placingField);
                _placingField = null;
                _placingPlayer = null;
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

            OnBoardReset?.Invoke();
            _resettingBoard = false;
            return Task.CompletedTask;
        }
        private Task TopicManualColumnChanged(string value)
        {
            throw new NotImplementedException();
        }

        private const string TOPIC_COLUMN = "column";
        private const string TOPIC_RESET = "reset";
        private const string TOPIC_MANUAL_COLUMN = "manualColumn";

        private bool _resettingBoard;
        private Field? _placingField;
        private Player? _placingPlayer;
        private readonly MQTTNetTopicClient _mqttTopicClient;
    }
}