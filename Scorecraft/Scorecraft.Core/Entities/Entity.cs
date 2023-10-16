using System.ComponentModel.DataAnnotations;

namespace Scorecraft.Entities
{
    public class Entity<TKey> : IEntity<TKey>
    {
        [Key]
        public TKey Id { get; set; }

        public string Name { get; set; }
    }
}
