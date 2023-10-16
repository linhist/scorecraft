using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data
{
    [Table("sfSeason")]
    public class SofaSeason : SofaEntity
    {
        public int? CompetitionId { get; set; }

        public string Years { get; set; }

        public virtual SofaCompetition Competition { get; set; }

        public virtual ICollection<SofaGroup> Groups { get; set; }

        public virtual ICollection<SofaRound> Rounds { get; set; }

    }
}
