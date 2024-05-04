using backend.game;

namespace backend.communication.mqtt
{
    internal class RoboterMQTTMock : IDisposable
    {
        public RoboterMQTTMock()
        {
            _topicClient.ConnectAsync().Wait();

            _topicClient.SubscribeToAsync("column", async (message) =>
            {
                int column;
                try
                {
                    column = Convert.ToInt32(message);
                }
                catch
                {
                    return;
                }

                if (column == -1)
                    return;

                Console.WriteLine("ROBOTER: Stone placed!");
                await _topicClient.PublishAsync("column", "-1");
            }).Wait();

            _topicClient.SubscribeToAsync("reset", async (message) =>
            {
                if (message != true.ToString())
                    return;

                Console.WriteLine("ROBOTER: Board reset!");
                await _topicClient.PublishAsync("reset", false.ToString());
            }).Wait();

        }

        public void Dispose()
        {
            _topicClient?.Dispose();
        }

        private readonly MQTTNetTopicClient _topicClient = new MQTTNetTopicClient("ws://mqtt.r4d4.work", "ardu", "Pw12ArduR4D4!");
    }
}
