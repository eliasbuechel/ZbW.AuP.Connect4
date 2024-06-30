using backend.game;
using backend.game.entities;
using backend.game.players;
using backend.infrastructure;
using backend.utilities;
using System.Diagnostics;
using System.Numerics;

namespace backend.communication.mqtt
{
    internal class MqttRoboterApi : RoboterApi
    {
        public MqttRoboterApi(MqttNetTopicClient mqttTopicClient)
        {
            _mqttTopicClient = mqttTopicClient;
            Connect().Wait();
        }

        protected override async void ResetConnect4BoardOnApi()
        {
            if (!_mqttTopicClient.IsConnected)
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, "Not able to reset roboter because mqtt-client is not connect.");
                BoardReset();
                return;
            }

            if (_resettingBoard)
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, "Request overlapping while resetting board.");

            _resettingBoard = true;
            await _mqttTopicClient.PublishAsync(TOPIC_RESET, true.ToString());
        }
        protected override async void PlaceStoneOnApi(Player player, Field field)
        {
            if (!_mqttTopicClient.IsConnected)
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Not able to place stone on roboter because mqtt-client is not connect. Player: '{player.Username}' Column: '{field.Column}'");
                StonePlaced(player, field);
                return;
            }

            if (_placingPlayer != null || _placingField != null)
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Request overlapping while placing stone. Player: '{player.Username}' Column: '{field.Column}'");

            _placingPlayer = player;
            _placingField = field;
            await _mqttTopicClient.PublishAsync(TOPIC_COLUMN, field.Column.ToString());
        }
        protected override async void OnDispose()
        {
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_COLUMN, TopicColumnChanged);
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_RESET, TopicResetChanged);
            await _mqttTopicClient.UnsubscribeFromAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
            await _mqttTopicClient.DisconnectAsync();
        }

        private async Task Connect()
        {
            await _mqttTopicClient.SubscribeToAsync(TOPIC_COLUMN, TopicColumnChanged);
            await _mqttTopicClient.SubscribeToAsync(TOPIC_RESET, TopicResetChanged);
            await _mqttTopicClient.SubscribeToAsync(TOPIC_MANUAL_COLUMN, TopicManualColumnChanged);
            await _mqttTopicClient.ConnectAsync();
        }
        private Task TopicColumnChanged(string value)
        {
            int column;
            try
            {
                column = Convert.ToInt32(value);
            }
            catch
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Not able to parse message on topic '{TOPIC_COLUMN}'. Message: '{value}'");
                return Task.CompletedTask;
            }

            if (column == -1)
            {
                if (_placingField == null || _placingPlayer == null)
                    return Task.CompletedTask;

                Field placingField = _placingField;
                Player placingPlayer = _placingPlayer;

                _placingField = null;
                _placingPlayer = null;

                StonePlaced(placingPlayer, placingField);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
        private Task TopicResetChanged(string value)
        {
            bool resetValue;
            try
            {
                resetValue = Convert.ToBoolean(value);
            }
            catch
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Not able to parse message on topic '{TOPIC_RESET}'. Message: '{value}'");
                return Task.CompletedTask;
            }

            if (resetValue)
                return Task.CompletedTask;

            if (!_resettingBoard)
            {
                Debug.Assert(true);
                return Task.CompletedTask;
            }

            BoardReset();
            _resettingBoard = false;
            return Task.CompletedTask;
        }
        private async Task TopicManualColumnChanged(string value)
        {
            int column;
            try
            {
                column = Convert.ToInt32(value);
            }
            catch
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Reciving message on topic '{TOPIC_MANUAL_COLUMN}'. Not able to parse message '{value}'");
                return;
            }

            if (column == -1)
                return;

            if (column < 0 || column >= Board.Columns)
            {
                Logger.Log(LogLevel.Error, LogContext.MQTT_CLIENT, $"Reciving message on topic '{TOPIC_MANUAL_COLUMN}'. Message is not in the valid range. Message: '{value}'");
                return;
            }

            await _mqttTopicClient.PublishAsync(TOPIC_MANUAL_COLUMN, "-1");
            try
            {
                ManualMove(column);
            }
            catch (InvalidPlayerRequestException e)
            {
                Logger.Log(LogLevel.Warning, LogContext.MQTT_CLIENT, "Not able to process manual move from roboter.", e);
            }
        }

        private const string TOPIC_COLUMN = "column";
        private const string TOPIC_RESET = "reset";
        private const string TOPIC_MANUAL_COLUMN = "manualColumn";

        private bool _resettingBoard;
        private Field? _placingField;
        private Player? _placingPlayer;
        private readonly MqttNetTopicClient _mqttTopicClient;
    }
}