using backend.game;
using backend.game.entities;

namespace backend.communication.mqtt
{
    internal class TestingRoboterAPI : IRoboterAPI
    {
        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;
        public event Action<int>? OnManualMove;

        public void PlaceStone(Player player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
        }
        public void ResetConnect4Board()
        {
            OnBoardReset?.Invoke();
        }
    }
}
