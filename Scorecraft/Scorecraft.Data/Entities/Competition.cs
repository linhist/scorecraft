using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data.Entities
{
    [Table("scCompetition")]
    public class Competition : BaseEntity
    {
        public int? RegionId { get; set; }

        public string Code { get; set; }

        public string Colors { get; set; }

        public string Logo { get; set; }

        public int? Priority { get; set; }

        public virtual Region Region { get; set; }

        public virtual ICollection<Season> Seasons { get; set; }
    }
}