using backend.game;
using backend.game.entities;

namespace backend.communication.mqtt
{
    internal interface IRoboterAPI
    {
        event Action<IPlayer, Field>? OnStonePlaced;
        event Action? OnBoardReset;
        event Action<int>? OnManualMove;
        void PlaceStone(IPlayer player, Field field);
        void ResetConnect4Board();
    }
}
