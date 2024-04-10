using backend.communication;
using backendTests.helperClasses;
using System.Net;

namespace backendTests
{
    [TestFixture]
    public class MqttTopicClientTests
    {
        [Test]
        public void MqttTopicBroker_ComunicateOverToppics_NoErrors()
        {
            // arrange
            const int PORT = 1890;
            IPAddress ipAddress = IPAddress.Loopback;
            using MqttTopicBroker broker = new MqttTopicBroker(ipAddress, PORT);
            MqttTopicClient client = new MqttTopicClient(ipAddress, PORT);
            broker.Start().Wait();
            const string TOPIC = "test";
            const string MESSAGE = "hello";
            const int TIMEOUT_DURATION = 10;


            // act - connecting
            client.OnConnected += OnConnectedCallback;
            bool isConnected = client.Connect();
            Assert.That(isConnected, Is.True);
            Assert.That(() => client.IsConnected, Is.True.After(TIMEOUT_DURATION));
            Assert.That(connectingCallbackCounter, Is.EqualTo(1));
            client.OnConnected -= OnConnectedCallback;


            // act - topic communication
            client.SubscribeTo(TOPIC, CommunicationCallback);
            client.Publish(TOPIC, MESSAGE);
            Assert.That(() => communicationCallbackCounter, Is.EqualTo(1).After(TIMEOUT_DURATION));
            Assert.That(communicationCallbackMessage, Is.EqualTo(MESSAGE));

            // act - connection loss
            client.OnDisconnected += OnDisconnectedCallback;
            broker.Stop().Wait();
            Assert.That(() => client.IsConnected, Is.False.After(TIMEOUT_DURATION));
            Assert.That(disconnectingCallbackCounter, Is.EqualTo(1));
            client.OnDisconnected -= OnDisconnectedCallback;

            // act - reconnect
            broker.Start().Wait();
            client.Connect();
            Assert.That(() => client.IsConnected, Is.True.After(TIMEOUT_DURATION));

            // act - topic communication after reconnection
            client.Publish(TOPIC, MESSAGE);
            Assert.That(() => communicationCallbackCounter, Is.EqualTo(2).After(TIMEOUT_DURATION));

            // act - unsubscribe from topic
            client.UnsubscribeFrom(TOPIC, CommunicationCallback);
            client.Publish(TOPIC, MESSAGE);
            Assert.That(communicationCallbackCounter, Is.EqualTo(2));

            // act - disconnect
            client.OnDisconnected += OnDisconnectedCallback;
            client.Disconnect();
            Assert.That(() => client.IsConnected, Is.False.After(TIMEOUT_DURATION));
            Assert.That(disconnectingCallbackCounter, Is.EqualTo(2));
            client.OnDisconnected -= OnDisconnectedCallback;


            // cleenup
            broker.Stop().Wait();
        }

        private void OnConnectedCallback()
        {
            connectingCallbackCounter++;
        }
        private void OnDisconnectedCallback()
        {
            disconnectingCallbackCounter++;
        }
        private void CommunicationCallback(string message)
        {
            communicationCallbackCounter++;
            communicationCallbackMessage = message;
        }

        private int connectingCallbackCounter = 0;
        private int disconnectingCallbackCounter = 0;
        private int communicationCallbackCounter = 0;
        private string communicationCallbackMessage = string.Empty;
    }
}