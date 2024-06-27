using System.Diagnostics;
using System.Net;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace backend.communication.mqtt
{
    internal class MqttTopicClient
    {
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public MqttTopicClient(IPAddress brokerIpAddress, int serverPort)
        {
            _brokerIpAddress = brokerIpAddress;
            _serverPort = serverPort;

            _mqttClient = new MqttClient(brokerIpAddress.ToString(), serverPort, false, null, null, MqttSslProtocols.None);

            _mqttClient.ConnectionClosed += OnConnectionClosed;
            _mqttClient.MqttMsgPublished += OnMessagePublished;
            _mqttClient.MqttMsgPublishReceived += OnMessageRecived;
        }
        ~MqttTopicClient()
        {
            if (_mqttClient.IsConnected)
                Disconnect();
        }

        public bool IsConnected => _mqttClient.IsConnected;

        public bool Connect()
        {
            if (_mqttClient.IsConnected)
            {
                Debug.Assert(false);
                return true;
            }

            Log($"connecting to {_brokerIpAddress}:{_serverPort} ");

            try
            {
                if (_topicCallbackMappings.Count == 0)
                    SubscribeTo("alive", m => Console.WriteLine($"Alive bit = {m}"));

                SubscribeToAllTopics();
                _mqttClient.Connect(_id.ToString());
            }
            catch (Exception e)
            {
                Log($"Not able to connect to server. Error: {e.Message}");
                return false;
            }

            if (_mqttClient.IsConnected)
                OnConnected?.Invoke();

            return _mqttClient.IsConnected;
        }
        public void Disconnect()
        {
            if (!_mqttClient.IsConnected)
            {
                Debug.Assert(false);
                return;
            }

            Log("Disconnecting...");
            UnsubscribeFromAllTopics();
            _mqttClient.Disconnect();
            Log("Disconnected");
        }
        public void Publish(string topic, string message)
        {
            if (!_mqttClient.IsConnected)
            {
                Debug.Assert(false);
                return;
            }

            _mqttClient.Publish(topic, Encoding.UTF8.GetBytes(message));
            Log($"Sent data to server on {topic}: '{message}'");
        }
        public void SubscribeTo(string topic, Action<string> callback)
        {
            if (_topicCallbackMappings.TryAdd(topic, new List<Action<string>>()))
            {
                _mqttClient.Subscribe([topic], [MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE]);
                _topicValueMappings.Add(topic, null);
            }

            _topicCallbackMappings[topic].Add(callback);
        }
        public void UnsubscribeFrom(string topic, Action<string> callback)
        {
            if (!_topicCallbackMappings.TryGetValue(topic, out ICollection<Action<string>>? callbacks))
            {
                Debug.Assert(false);
                return;
            }

            if (!callbacks.Contains(callback))
            {
                Debug.Assert(false);
                return;
            }
            callbacks.Remove(callback);

            if (callbacks.Count == 0)
            {
                _mqttClient.Unsubscribe([topic]);
                _topicCallbackMappings.Remove(topic);
                _topicValueMappings.Remove(topic);
            }
        }

        private void OnMessageRecived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;
            string message = Encoding.UTF8.GetString(e.Message);

            Log($"Received test message for topic: {topic}");

            if (_topicValueMappings[topic] == null)
            {
                _topicValueMappings[topic] = message;
                return;
            }

            IEnumerable<Action<string>> callbacks = _topicCallbackMappings[topic];

            foreach (var callback in callbacks)
            {
                try
                {
                    callback(message);
                }
                catch (Exception ex)
                {
                    Log($"Error while executing callback for topic {topic}: {ex.Message}");
                }
            }
        }
        private void OnMessagePublished(object sender, MqttMsgPublishedEventArgs e)
        {
            Log($"Message got published on topic: {e.MessageId}");
        }
        private void OnConnectionClosed(object sender, EventArgs e)
        {
            OnDisconnected?.Invoke();
            Log($"Connection closed");
        }
        private void SubscribeToAllTopics()
        {
            if (_mqttClient.IsConnected)
            {
                Debug.Assert(false);
                return;
            }

            List<string> topics = [];
            List<byte> qosLevels = [];

            foreach (var topicCallbackMapping in _topicCallbackMappings)
            {
                topics.Add(topicCallbackMapping.Key);
                qosLevels.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
            }

            _mqttClient.Subscribe([.. topics], [.. qosLevels]);
            Log($"Subscribed to topics '{string.Join(", ", topics)}'");
        }
        private void UnsubscribeFromAllTopics()
        {
            Log("Unsubscribing from topics...");

            List<string> topics = [];

            foreach (var topicCallbackMapping in _topicCallbackMappings)
            {
                topics.Add(topicCallbackMapping.Key);
            }

            Log("Unsubscribed from topics");
        }
        private static void Log(string message)
        {
            Console.WriteLine($"MQTT-CLIENT: {message}");
        }

        private readonly Guid _id = Guid.NewGuid();
        private readonly int _serverPort;
        private readonly IPAddress _brokerIpAddress;
        private readonly MqttClient _mqttClient;
        private readonly Dictionary<string, ICollection<Action<string>>> _topicCallbackMappings = [];
        private readonly Dictionary<string, string?> _topicValueMappings = [];
    }
}