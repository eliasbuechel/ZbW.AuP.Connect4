using backend.game.entities;

namespace backend.communication.dtos
{
    internal abstract class EntityDto
    {
        public EntityDto(string id)
        {
            Id = id;
        }
        public EntityDto(Entity entity)
        {
            Id = entity.Id;
        }

        public string Id { get; }
    }
}
