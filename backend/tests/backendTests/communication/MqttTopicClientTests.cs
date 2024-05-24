using backend.communication.mqtt;

namespace backendTests.communication
{
    [TestFixture]
    public class MqttTopicClientTests
    {
        //[Test]
        public void MqttTopicBroker_ComunicateOverToppics_NoErrors()
        {
            // arrange
            MQTTNetTopicClient client = new MQTTNetTopicClient("ws://mqtt.r4d4.work", "ardu", "Pw12ArduR4D4!");
            //MQTTnetTopicClient client = new MQTTnetTopicClient("ws://mqtt.r4d4.work");
            const string TOPIC = "test/gabor";
            const string MESSAGE = "My own test message!";
            const int TIMEOUT_DURATION = 1000;

            // act - connecting
            client.OnConnected += OnConnectedCallback;
            client.ConnectAsync().Wait();
            Assert.That(() => client.IsConnected, Is.True.After(TIMEOUT_DURATION));
            Assert.That(connectingCallbackCounter, Is.EqualTo(1));
            client.OnConnected -= OnConnectedCallback;


            // act - topic communication
            client.SubscribeToAsync(TOPIC, CommunicationCallback).Wait();

            client.PublishAsync(TOPIC, MESSAGE).Wait();
            Assert.That(() => communicationCallbackCounter, Is.EqualTo(1).After(TIMEOUT_DURATION));
            Assert.That(communicationCallbackMessage, Is.EqualTo(MESSAGE));

            client.PublishAsync(TOPIC, MESSAGE).Wait();
            Assert.That(() => communicationCallbackCounter, Is.EqualTo(2).After(TIMEOUT_DURATION));
            Assert.That(communicationCallbackMessage, Is.EqualTo(MESSAGE));

            client.UnsubscribeFromAsync(TOPIC, CommunicationCallback).Wait();

            client.PublishAsync(TOPIC, MESSAGE).Wait();
            Assert.That(() => communicationCallbackCounter, Is.EqualTo(2).After(TIMEOUT_DURATION));


            client.DisconnectAsync().Wait();
            client.Dispose();
        }


        private void OnConnectedCallback()
        {
            connectingCallbackCounter++;
        }
        private async Task CommunicationCallback(string message)
        {
            communicationCallbackCounter++;
            communicationCallbackMessage = message;
            await Task.CompletedTask;
        }

        private int connectingCallbackCounter = 0;
        private int communicationCallbackCounter = 0;
        private string communicationCallbackMessage = string.Empty;
    }
}