using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Text;

namespace backend.communication.mqtt
{
    internal class TestClient
    {
        public async Task Run()
        {
            string brokerUri = "ws://mqtt.r4d4.work"; // WebSocket URI of the MQTT broker
            string clientId = Guid.NewGuid().ToString();

            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(brokerUri) // Use WebSocket connection
                .WithClientId(clientId)
                .Build();

            var managedOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(options)
                .Build();

            managedClient = new MqttFactory().CreateManagedMqttClient();
            await managedClient.StartAsync(managedOptions);

            managedClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected to MQTT broker via WebSocket.");

                // Subscribe to a topic when connected
                await managedClient.SubscribeAsync("test/topic");
            });

            managedClient.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine($"Received message on topic: {e.ApplicationMessage.Topic}");
                Console.WriteLine($"Payload: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            });

            await Task.Delay(1000);
            OnTimerElapsed();

            await Task.Delay(1000);
            OnTimerElapsed();

            await Task.Delay(1000);
            OnTimerElapsed();

        }

        private async void OnTimerElapsed()
        {
            // Publish a test message every 5 seconds
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("test/topic")
                .WithPayload("Hello from C#")
                .WithExactlyOnceQoS()
                .WithRetainFlag()
                .Build();

            await managedClient.PublishAsync(message);

            Console.WriteLine("Message published: Hello from C#");
        }

        private IManagedMqttClient managedClient;
        //private System.Timers.Timer messageTimer;
    }
}
