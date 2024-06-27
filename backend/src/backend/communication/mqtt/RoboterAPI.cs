using backend.game;
using backend.game.entities;
using backend.infrastructure;

namespace backend.communication.mqtt
{
    internal abstract class RoboterApi : DisposingObject
    {
        public event Action<Player, Field>? OnStonePlaced;
        public event Action? OnBoardReset;
        public event Action<int>? OnManualMove;
        public event Action<Player, Field>? OnPlacingStone;

        public void PlaceStone(Player player, Field field)
        {
            OnPlacingStone?.Invoke(player, field);
            PlaceStoneOnApi(player, field);
        }
        public abstract void ResetConnect4Board();

        protected abstract void PlaceStoneOnApi(Player player, Field field);
        protected void StonePlaced(Player player, Field field)
        {
            OnStonePlaced?.Invoke(player, field);
        }
        protected void BoardReset()
        {
            OnBoardReset?.Invoke();
        }
        protected void ManualMove(int column)
        {
            OnManualMove?.Invoke(column);
        }
    }
}