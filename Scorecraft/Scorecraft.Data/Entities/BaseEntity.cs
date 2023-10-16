using Scorecraft.Entities;

namespace Scorecraft.Data.Entities
{
    public class BaseEntity : Entity<int>
    {
        public string NameVN { get; set; }

        public bool? Active { get; set; }
    }
}