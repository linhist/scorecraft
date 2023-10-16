using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data
{
    [Table("sfRound")]
    public class SofaRound : SofaEntity
    {
        public int? SeasonId { get; set; }

        public string Slug { get; set; }

        public string Prefix { get; set; }

        public int? RoundNo { get; set; }

        public int? Priority { get; set; }

        public bool? Current { get; set; }
    }
}