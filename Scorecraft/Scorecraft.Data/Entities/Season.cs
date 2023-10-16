using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data.Entities
{
    [Table("scSeason")]
    public class Season : BaseEntity
    {
        public int? CompetitionId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? FinishDate { get; set;}

        public virtual Competition Competition { get; set; }
    }
}