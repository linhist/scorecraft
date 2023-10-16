using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data
{
    [Table("sfCompetition")]
    public class SofaCompetition : SofaEntity
    {
        public int? RegionId { get; set; }

        public string Slug { get; set; }

        public string Logo { get; set; }

        public string Colors { get; set; }

        public bool? HasGroup { get; set; }

        public bool? HasRound { get; set; }

        public bool? HasPlayoff { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual SofaRegion Region { get; set; }

        public virtual ICollection<SofaSeason> Seasons { get; set; }
    }
}