
using backend.game;
using System.Diagnostics;

namespace backend.communication.mqtt
{
    internal class RoboterAPI
    {
        public RoboterAPI(MqttTopicClient mqttTopicClient)
        {
            _mqttTopicClient = mqttTopicClient;

            _mqttTopicClient.Connect();
            _mqttTopicClient.SubscribeTo(TOPIC_COLUMN, TopicColumnChanged);
            _mqttTopicClient.SubscribeTo(TOPIC_RESET, TopicResetChanged);
            _mqttTopicClient.SubscribeTo(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
        }


        ~RoboterAPI()
        {
            _mqttTopicClient.UnsubscribeFrom(TOPIC_COLUMN, TopicColumnChanged);
            _mqttTopicClient.UnsubscribeFrom(TOPIC_RESET, TopicResetChanged);
            _mqttTopicClient.UnsubscribeFrom(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
            _mqttTopicClient.Disconnect();
        }

        internal void PlaceStone(Field field, Action<IPlayer, Field> stonePlacedCallback)
        {
            _stonePlacedCallback = stonePlacedCallback;
            _placingField = field;
            _mqttTopicClient.Publish(TOPIC_COLUMN, field.Column.ToString());
        }

        internal void TopicColumnChanged(string value)
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

                _stonePlacedCallback?.Invoke(_placingPlayer, _placingField);
                return;
            }
        }
        private void TopicResetChanged(string value)
        {
            throw new NotImplementedException();
        }
        private void TopicManualColumnChanged(string value)
        {
            throw new NotImplementedException();
        }

        private readonly MqttTopicClient _mqttTopicClient;
        private Action<IPlayer, Field>? _stonePlacedCallback;
        private Field? _placingField;
        private IPlayer? _placingPlayer;

        private const string TOPIC_COLUMN = "column";
        private const string TOPIC_RESET = "reset";
        private const string TOPIC_MANUAL_COLUMN = "manualColumn";
    }
}
