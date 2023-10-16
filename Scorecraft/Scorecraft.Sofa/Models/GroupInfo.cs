namespace Scorecraft.Sofa.Models
{
    public class GroupInfo : SofaInfo
    {
        public string GroupName { get; set; }

        public int? TournamentId { get; set; }

        public int? Priority { get; set; }

        public SeasonInfo Season { get; set; }
    }
}