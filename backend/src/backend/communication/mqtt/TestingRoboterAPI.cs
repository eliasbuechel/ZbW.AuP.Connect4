using backend.game;

namespace backend.communication.mqtt
{
    internal class TestingRoboterAPI : IRoboterAPI
    {
        public event Action<IPlayer, Field>? OnStonePlaced;
        public event Action? OnBoardReset;

        public void PlaceStone(IPlayer player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
        }
        public void ResetConnect4Board()
        {
            OnBoardReset?.Invoke();
        }
    }
}
