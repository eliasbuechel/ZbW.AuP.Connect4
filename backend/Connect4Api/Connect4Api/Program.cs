namespace Connect4Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost webAppHost = CreateHostBuilder(args).Build();

            new Thread(() =>
            {
                webAppHost.Services.GetRequiredService<MqttTopicBroker>().Start();
                webAppHost.Services.GetRequiredService<MqttTopicClient>().Connect();
                MqttTest(webAppHost.Services.GetRequiredService<MqttTopicClient>());
            }).Start();
            
            webAppHost.Run();

            webAppHost.Services.GetRequiredService<MqttTopicClient>().Disconnect();
            webAppHost.Services.GetRequiredService<MqttTopicBroker>().Stop();
        }
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {   
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:7136");
                });
        }
        private static void MqttTest(MqttTopicClient client)
        {
            string topic = "testTopic";

            client.SubscribeTo(topic, message =>
            {
                Console.WriteLine($"Doing something in the Programm with the recived messge '{message}' on the topic '{topic}'");
            });

            client.Publish(topic, "This is a testmessage!");
            client.Publish(topic, "This is a second testmessage!");
        }
    }
}