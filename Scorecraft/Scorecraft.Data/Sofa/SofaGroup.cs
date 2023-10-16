using System.ComponentModel.DataAnnotations.Schema;

namespace Scorecraft.Data
{
    [Table("sfGroup")]
    public class SofaGroup : SofaEntity
    {
        public int? SeasonId { get; set; }

        public int? Priority { get; set; }

        public virtual SofaSeason Season { get; set; }
    }
}
