using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data.Entities
{
    [Table("scRegion")]
    public class Region : BaseEntity
    {
        public string Code { get; set; }

        public string Flag { get; set; }

        public int? Priority { get; set; }

        public virtual ICollection<Competition> Competitions { get; set; }
    }
}
