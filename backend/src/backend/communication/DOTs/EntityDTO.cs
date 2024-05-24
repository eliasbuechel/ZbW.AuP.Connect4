using backend.game.entities;

namespace backend.communication.DOTs
{
    internal abstract class EntityDTO
    {
        public EntityDTO(string id)
        {
            Id = id;
        }
        public EntityDTO(Entity entity)
        {
            Id = entity.Id;
        }

        public string Id { get; }
    }
}
