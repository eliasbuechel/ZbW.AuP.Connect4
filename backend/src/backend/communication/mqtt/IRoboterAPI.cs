using backend.game;

namespace backend.communication.mqtt
{
    internal interface IRoboterAPI
    {
        event Action<IPlayer, Field>? OnStonePlaced;
        event Action? OnBoardReset;
        void PlaceStone(IPlayer player, Field field);
        void ResetConnect4Board();
    }
}
