
using backend.game;
using backend.Infrastructure;
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

        public event Action<IPlayer, Field>? OnStonePlaced;
        public event Action? OnBoardReset;
        public event Action<int>? OnManualMove;

        public void PlaceStone(IPlayer player, Field field)
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

        private async Task TopicColumnChanged(string value)
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

            if (column == -1)
            {
                Debug.Assert(_placingField != null);
                Debug.Assert(_placingPlayer != null);

                OnStonePlaced?.Invoke(_placingPlayer, _placingField);
                _placingField = null;
                _placingPlayer = null;
                return;
            }
        }
        private async Task TopicResetChanged(string value)
        {
            bool resetValue;
            try
            {
                resetValue = Convert.ToBoolean(value);
            }
            catch
            {
                Debug.Assert(false);
                return;
            }

            if (!resetValue)
            {
                Debug.Assert(_resettingBoard);
                OnBoardReset?.Invoke();
                _resettingBoard = false;
                return;
            }
        }
        private async Task TopicManualColumnChanged(string value)
        {
            throw new NotImplementedException();
        }

        private const string TOPIC_COLUMN = "column";
        private const string TOPIC_RESET = "reset";
        private const string TOPIC_MANUAL_COLUMN = "manualColumn";

        private bool _resettingBoard;
        private Field? _placingField;
        private IPlayer? _placingPlayer;
        private readonly MQTTNetTopicClient _mqttTopicClient;
    }
}
