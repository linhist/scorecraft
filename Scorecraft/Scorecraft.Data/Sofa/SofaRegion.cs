using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data
{
    [Table("sfRegion")]
    public class SofaRegion : SofaEntity
    {
        public string Slug { get; set; }

        public string Code { get; set; }

        public string Flag { get; set; }

        public int? Priority { get; set; }

        public virtual ICollection<SofaCompetition> Competitions { get; set; }
    }
}
