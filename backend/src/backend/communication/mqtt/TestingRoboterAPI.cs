using backend.game;
using backend.game.entities;

namespace backend.communication.mqtt
{
    internal class TestingRoboterAPI : RoboterAPI
    {
        public override void ResetConnect4Board()
        {
            BoardReset();
        }

        protected override void PlaceStoneOnApi(Player player, Field field)
        {
            StonePlaced(player, field);
        }
    }
}
