using MQTTnet;
using MQTTnet.Server;
using System.Diagnostics;
using System.Net;

namespace Connect4Api
{
    public class MqttTopicBroker : IDisposable
    {
        public MqttTopicBroker(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;

            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(_ipAddress)
                .WithDefaultEndpointPort(_port)
                .WithDefaultEndpoint()
                .Build();

            _mqttServer = new MqttFactory().CreateMqttServer(mqttServerOptions);
        }
        ~MqttTopicBroker()
        {
            if (_disposed)
                return;

            Debug.Assert(false);
            Dispose();
        }

        public IPAddress IPAddress => _ipAddress;
        public int Port => _port;

        public void Start()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            if (_mqttServer.IsStarted)
            {
                Debug.Assert(false);
                return;
            }

            Log($"Starting at {_ipAddress}:{_port}...");
            _mqttServer.StartAsync();
            Log($"Started. Now listening on {_ipAddress}:{_port}");
        }
        public void Stop()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            if (!_mqttServer.IsStarted)
            {
                Debug.Assert(false);
                return;
            }

            Log("Stopping...");
            _mqttServer.StopAsync();
            Log("Stopped");
        }
        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            if (_mqttServer.IsStarted)
            {
                Debug.Assert(false);
                _mqttServer.StopAsync();
            }

            Log("Disposing...");
            _mqttServer.Dispose();
            _disposed = true;
            Log("Disposed");
        }

        private void Log(string message)
        {
            Console.WriteLine($"MQTT-SERVER: {message}");
        }

        private bool _disposed = false;
        private readonly MqttServer _mqttServer;
        private readonly IPAddress _ipAddress;
        private readonly int _port;
    }
}
