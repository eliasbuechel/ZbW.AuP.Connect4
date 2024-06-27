using backend.game.entities;

namespace backend.communication.dtos
{
    internal abstract class EntityDto
    {
        protected EntityDto(string id)
        {
            Id = id;
        }
        protected EntityDto(Entity entity)
        {
            Id = entity.Id;
        }

        public string Id { get; }
    }
}
