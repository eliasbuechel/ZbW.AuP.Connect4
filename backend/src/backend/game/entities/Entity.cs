using backend.Data.entities;

namespace backend.game.entities
{
    internal abstract class Entity
    {
        public Entity()
        {
            Id = Guid.NewGuid().ToString();
        }
        public Entity(string id)
        {
            Id = id;
        }
        public Entity(Entity entity)
        {
            Id = entity.Id;
        }
        public Entity(DbEntity entity)
        {
            Id = entity.Id;
        }

        public string Id { get; }
    }
}
