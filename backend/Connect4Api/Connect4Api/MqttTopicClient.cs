using System.Diagnostics;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Text;
using System.Net;

namespace Connect4Api
{
    public class MqttTopicClient
    {
        public event Func<Task>? OnConnected;

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
                if (_topicCallbacksMappings.Count == 0)
                    SubscribeTo("alive", m => Console.WriteLine($"Alive bit = {m}"));

                SubscribeToAllTopics();
                _mqttClient.Connect(_mqttClientId.ToString());
            }
            catch (Exception e)
            {
                Log($"Not able to connect to server. Error: {e.Message}");
                return false;
            }

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
            if (!_topicCallbacksMappings.ContainsKey(topic))
            {
                _topicCallbacksMappings.Add(topic, new List<Action<string>>());
                _mqttClient.Subscribe([topic], [MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE]);
            }

           _topicCallbacksMappings[topic].Add(callback);
        }
        public void UnsubscribeFrom(string topic, Action<string> callback)
        {
            if (!_topicCallbacksMappings.ContainsKey(topic))
            {
                Debug.Assert(false);
                return;
            }

            ICollection<Action<string>> callbacks = _topicCallbacksMappings[topic];
            if (!callbacks.Contains(callback))
            {
                Debug.Assert(false);
                return;
            }
            callbacks.Remove(callback);

            if (callbacks.Count == 0)
            {
                _mqttClient.Unsubscribe([topic]);
                _topicCallbacksMappings.Remove(topic);
            }
        }
        private void OnMessageRecived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;
            string message = Encoding.UTF8.GetString(e.Message);

            Log($"Received test message for topic: {topic}");

            IEnumerable<Action<string>> callbacks = _topicCallbacksMappings[topic];

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
            Log($"Connection closed");
        }
        private void SubscribeToAllTopics()
        {
            if (_mqttClient.IsConnected)
            {
                Debug.Assert(false);
                return;
            }

            ICollection<string> topics = new List<string>();
            ICollection<byte> qosLevels = new List<byte>();

            foreach (var topicCallbackMapping in _topicCallbacksMappings)
            {
                topics.Add(topicCallbackMapping.Key);
                qosLevels.Add(MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE);
            }

            _mqttClient.Subscribe(topics.ToArray(), qosLevels.ToArray());
            Log($"Subscribed to topics '{string.Join(", ", topics)}'");
        }
        private void UnsubscribeFromAllTopics()
        {
            Log("Unsubscribing from topics...");

            ICollection<string> topics = new List<string>();

            foreach (var topicCallbackMapping in _topicCallbacksMappings)
            {
                topics.Add(topicCallbackMapping.Key);
            }

            Log("Unsubscribed from topics");
        }
        private void Log(string message)
        {
            Console.WriteLine($"MQTT-CLIENT: {message}");
        }

        private readonly int _serverPort;
        private readonly IPAddress _brokerIpAddress;
        private readonly MqttClient _mqttClient;
        private readonly Guid _mqttClientId = Guid.NewGuid();
        private readonly Dictionary<string, ICollection<Action<string>>> _topicCallbacksMappings = new Dictionary<string, ICollection<Action<string>>>();
    }


    //public class MqttTopicClient : IAsyncDisposable
    //{
    //    public event Func<Task>? OnConnected;

    //    public MqttTopicClient(IPAddress serverIP, int serverPort)
    //    {
    //        _serverIP = serverIP;
    //        _serverPort = serverPort;

    //        _mqttClientOptions = new MqttClientOptionsBuilder()
    //            .WithTcpServer(_serverIP.ToString(), 1883)
    //            .WithCredentials("", "")
    //            .WithClientId(Guid.NewGuid().ToString())
    //            .WithCleanSession()
    //            .Build();
    //    }
    //    ~MqttTopicClient()
    //    {
    //        Debug.Assert(_disposed);

    //        if (!_disposed)
    //        {
    //            Task.WaitAll(DisposeAsync().AsTask());
    //        }
    //    }

    //    public async Task ConnectAsync()
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            _mqttClient.ConnectedAsync += OnConnectedAsync;
    //            _mqttClient.ConnectingAsync += OnConnectingAsync;
    //            _mqttClient.DisconnectedAsync += OnDisconnectedAsync;
    //            _mqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;


    //            Console.WriteLine("MQTT client Connecting to server...");

    //            try
    //            {
    //                using (var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
    //                {
    //                    MqttClientConnectResult connectResult = await _mqttClient.ConnectAsync(_mqttClientOptions, cancellationToken.Token);
    //                    if (connectResult.ResultCode != MqttClientConnectResultCode.Success)
    //                        Console.WriteLine($"MQTT client not able to connect to server. Error: {connectResult.ResultCode}");
    //                }

    //            }
    //            catch (Exception e)
    //            {
    //                Console.WriteLine($"MQTT client not able to connect to server. Error: {e.Message}");
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    public async Task SendAsync(string topic, string message)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            try
    //            {
    //                await _mqttClient.PublishStringAsync(topic, message);
    //                Console.WriteLine($"MQTT client sent data to server. Topic: {topic} Message: {message}");
    //            }
    //            catch (Exception e)
    //            {
    //                Console.WriteLine($"Error sending message to server: Topic: {topic} Message: {message} Error: {e.Message}");
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    public async Task DisconnectAsync()
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {

    //            Debug.Assert(!_disposed);

    //            _mqttClient.ConnectedAsync -= OnConnectedAsync;
    //            _mqttClient.ConnectingAsync -= OnConnectingAsync;
    //            _mqttClient.DisconnectedAsync -= OnDisconnectedAsync;
    //            _mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;

    //            await UnsubscribeFromAllTopicsAsync();

    //            if (_mqttClient == null)
    //                return;

    //            Console.WriteLine("MQTT client disconnecting from server...");
    //            await _mqttClient.DisconnectAsync();
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    public async Task SubscribeToAsync(string topic, Func<string, Task> callback)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            if (!_topicCallbacksMappings.ContainsKey(topic))
    //            {
    //                _topicCallbacksMappings.Add(topic, new List<Func<string, Task>>());
    //                await _mqttClient.SubscribeAsync(topic);
    //            }

    //            ICollection<Func<string, Task>> callbacks = _topicCallbacksMappings[topic];
    //            callbacks.Add(callback);
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    public async Task UnsubscribeFromAsync(string topic, Func<string, Task> callback)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            if (_topicCallbacksMappings.ContainsKey(topic))
    //            {
    //                ICollection<Func<string, Task>> callbacks = _topicCallbacksMappings[topic];
    //                Debug.Assert(callbacks.Remove(callback));

    //                if (callbacks.Count == 0)
    //                {
    //                    await _mqttClient.UnsubscribeAsync(topic);
    //                    _topicCallbacksMappings.Remove(topic);
    //                }
    //            }
    //            else
    //            {
    //                Debug.Assert(false);
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    public async ValueTask DisposeAsync()
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            if (!_disposed)
    //            {
    //                foreach (var topic in _topicCallbacksMappings.Keys.ToList())
    //                {
    //                    _mqttClient.UnsubscribeAsync(topic).GetAwaiter().GetResult();
    //                    _topicCallbacksMappings.Remove(topic);
    //                }

    //                _mqttClient.Dispose();
    //                _disposed = true;
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }

    //    private async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            if (args.Reason == MqttClientDisconnectReason.NormalDisconnection)
    //            {
    //                Console.WriteLine($"MQTT client disconnected from server");
    //            }
    //            else
    //            {
    //                if (_reconnectionAttepts++ < 3)
    //                {
    //                    Console.WriteLine($"MQTT client disconnected abnormal! Reason: {args.ReasonString}");
    //                    Console.WriteLine("MQTT client reconnecting to server...");
    //                    await _mqttClient.ReconnectAsync();
    //                }
    //                else
    //                {
    //                    _mqttClient.ConnectedAsync -= OnConnectedAsync;
    //                    _mqttClient.ConnectingAsync -= OnConnectingAsync;
    //                    _mqttClient.DisconnectedAsync -= OnDisconnectedAsync;
    //                    _mqttClient.ApplicationMessageReceivedAsync -= OnApplicationMessageReceivedAsync;

    //                    await UnsubscribeFromAllTopicsAsync();

    //                    Console.WriteLine($"MQTT client stopped reconnecting after {_reconnectionAttepts} attempts.");
    //                }
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    private async Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            string topic = args.ApplicationMessage.Topic;
    //            string message = args.ApplicationMessage.ConvertPayloadToString();

    //            Console.WriteLine($"MQTT client received test message for topic: {topic}");

    //            IEnumerable<Func<string, Task>> callbacks = _topicCallbacksMappings[topic];

    //            foreach (var callback in callbacks)
    //            {
    //                try
    //                {
    //                    await callback(message);
    //                }
    //                catch (Exception e)
    //                {
    //                    Console.WriteLine($"MQTT client error executing callback for topic {topic}: {e.Message}");
    //                }
    //            }
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    private async Task OnConnectingAsync(MqttClientConnectingEventArgs args)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Console.WriteLine("MQTT client Connecting to server...");
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    private async Task OnConnectedAsync(MqttClientConnectedEventArgs args)
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            _reconnectionAttepts = 0;
    //            Console.WriteLine($"MQTT client connected to server[{_serverIP}:{_serverPort}]");
    //            await SubscribeToAllTopicsAsync();

    //            OnConnected?.Invoke();
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    private async Task SubscribeToAllTopicsAsync()
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            Console.WriteLine("MQTT client subscribing to topics...");
    //            MqttClientSubscribeOptionsBuilder subscriptionOptionBuilder = new MqttFactory().CreateSubscribeOptionsBuilder();

    //            foreach (var topicCallbackMapping in _topicCallbacksMappings)
    //            {
    //                subscriptionOptionBuilder.WithTopicFilter(topicCallbackMapping.Key);
    //            }

    //            MqttClientSubscribeOptions subscriptionOptions = subscriptionOptionBuilder.Build();
    //            await _mqttClient.SubscribeAsync(subscriptionOptions);

    //            Console.WriteLine("MQTT client subscribed to topics");
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }
    //    private async Task UnsubscribeFromAllTopicsAsync()
    //    {
    //        await _semaphoreSlim.WaitAsync();

    //        try
    //        {
    //            Debug.Assert(!_disposed);

    //            Console.WriteLine("MQTT client unsubscribing from topics...");
    //            MqttClientUnsubscribeOptionsBuilder unsubscriptionOptionBuilder = new MqttFactory().CreateUnsubscribeOptionsBuilder();

    //            foreach (var topicCallbackMapping in _topicCallbacksMappings)
    //            {
    //                unsubscriptionOptionBuilder.WithTopicFilter(topicCallbackMapping.Key);
    //            }

    //            MqttClientUnsubscribeOptions subscriptionOptions = unsubscriptionOptionBuilder.Build();
    //            await _mqttClient.UnsubscribeAsync(subscriptionOptions);

    //            Console.WriteLine("MQTT client unsubscribed from topics");
    //        }
    //        finally
    //        {
    //            _semaphoreSlim.Release();
    //        }
    //    }


    //    private bool _disposed = false;
    //    private byte _reconnectionAttepts = 0;

    //    private readonly int _serverPort;
    //    private readonly IPAddress _serverIP;
    //    private readonly MqttClientOptions _mqttClientOptions;
    //    private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
    //    private readonly IMqttClient _mqttClient = new MqttFactory().CreateMqttClient();
    //    private readonly Dictionary<string, ICollection<Func<string, Task>>> _topicCallbacksMappings = new Dictionary<string, ICollection<Func<string, Task>>>();
    //}
}
