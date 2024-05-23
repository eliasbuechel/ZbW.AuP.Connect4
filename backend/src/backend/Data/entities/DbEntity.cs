using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using backend.game.entities;

namespace backend.Data.entities
{
    internal abstract class DbEntity
    {
        public DbEntity() { }
        public DbEntity(string id)
        {
            Id = id;
        }
        public DbEntity(Entity entity)
        {
            Id = entity.Id;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; } = string.Empty;
    }
}
