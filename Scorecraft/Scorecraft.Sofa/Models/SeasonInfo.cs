namespace Scorecraft.Sofa.Models
{
    public class SeasonInfo : SofaInfo
    {
        public string Year { get; set; }

        public CompetitionInfo Competition { get; set; }
    }
}