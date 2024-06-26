﻿using backend.infrastructure;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Diagnostics;
using System.Text;

namespace backend.communication.mqtt
{
    internal class MqttNetTopicClient : DisposingObject
    {
        public MqttNetTopicClient(string brokerUri, string username, string password)
        {
            _brokerUri = brokerUri;
            _username = username;
            _password = password;

            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(_brokerUri)
                .WithClientId(_clientId.ToString())
                .WithCredentials(_username, _password)
                .Build();

            _managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            _managedClient = new MqttFactory().CreateManagedMqttClient();
            _managedClient.UseConnectedHandler(OnConnectedToBorker);
            _managedClient.UseDisconnectedHandler(OnDisonnectedFromBroker);
            _managedClient.UseApplicationMessageReceivedHandler(OnMessageRecivedFromBroker);
        }
        internal MqttNetTopicClient(string brokerUri)
        {
            _brokerUri = brokerUri;
            _username = "";
            _password = "";

            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(_brokerUri)
                .WithClientId(_clientId.ToString())
                //.WithCredentials(_username, _password)
                .Build();

            _managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            _managedClient = new MqttFactory().CreateManagedMqttClient();
            _managedClient.UseConnectedHandler(OnConnectedToBorker);
            _managedClient.UseDisconnectedHandler(OnDisonnectedFromBroker);
            _managedClient.UseApplicationMessageReceivedHandler(OnMessageRecivedFromBroker);
        }

        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public bool IsConnected => _connected;

        public async Task ConnectAsync()
        {
            await _managedClient.StartAsync(_managedOptions);
        }
        public async Task DisconnectAsync()
        {
            await _managedClient.StopAsync();
        }

        public async Task PublishAsync(string topic, string message)
        {
            if (!IsConnected)
                return;

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await _managedClient.PublishAsync(applicationMessage);

            Log(LogLevel.Debug, $"Send on [{topic}]:'{message}'");
        }
        public async Task SubscribeToAsync(string topic, Func<string, Task> callback)
        {
            if (_topicCallbackMappings.TryAdd(topic, new List<Func<string, Task>>()))
            {
                await _managedClient.SubscribeAsync(topic);
                _topicValueMappings.Add(topic, null);
            }

            _topicCallbackMappings[topic].Add(callback);
        }
        public async Task UnsubscribeFromAsync(string topic, Func<string, Task> callback)
        {
            if (!_topicCallbackMappings.TryGetValue(topic, out ICollection<Func<string, Task>>? callbacks))
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
                await _managedClient.UnsubscribeAsync([topic]);
                _topicCallbackMappings.Remove(topic);
                _topicValueMappings.Remove(topic);
            }
        }
            
        private async Task OnConnectedToBorker(MqttClientConnectedEventArgs e)
        {
            if (string.IsNullOrEmpty(_username))
                Log(LogLevel.Information, $"Connected to broker at '{_brokerUri}'.");
            else
                Log(LogLevel.Information, $"Connected as '{_username}' to broker at '{_brokerUri}'.");

            _connected = true;
            OnConnected?.Invoke();
            await Task.CompletedTask;
        }
        private async Task OnDisonnectedFromBroker(MqttClientDisconnectedEventArgs e)
        {
            if (e.ClientWasConnected)
                Log(LogLevel.Information, "Disconnected from broker.", e.Exception);
            else
                Log(LogLevel.Warning, "Not able to connect to broker.", e.Exception);

            _connected = false;
            OnDisconnected?.Invoke();
            await Task.CompletedTask;
        }
        private async Task OnMessageRecivedFromBroker(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            Log(LogLevel.Debug, $"Recive on [{topic}]:'{message}'");

            if (_topicValueMappings[topic] == null)
            {
                _topicValueMappings[topic] = message;
                return;
            }

            _topicValueMappings[topic] = message;


            IEnumerable<Func<string, Task>> callbacks = _topicCallbackMappings[topic];

            foreach (var callback in callbacks)
            {
                try
                {
                    await callback(message);
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, $"Error while executing callback for topic {topic}: {ex.Message}");
                }
            }
        }

        private static void Log(LogLevel logLevel, string message)
        {
            Logger.Log(logLevel, LogContext.MQTT_CLIENT, message);
        }
        private static void Log(LogLevel logLevel, string message, Exception e)
        {
            Logger.Log(logLevel, LogContext.MQTT_CLIENT, message, e);
        }

        protected override void OnDispose()
        {
            _managedClient.Dispose();
        }

        private bool _connected;
        private readonly string _brokerUri;
        private readonly string _username;
        private readonly string _password;
        private readonly Guid _clientId = Guid.NewGuid();
        private readonly IManagedMqttClient _managedClient;
        private readonly ManagedMqttClientOptions _managedOptions;
        private readonly Dictionary<string, ICollection<Func<string, Task>>> _topicCallbackMappings = [];
        private readonly Dictionary<string, string?> _topicValueMappings = [];
    }
}
