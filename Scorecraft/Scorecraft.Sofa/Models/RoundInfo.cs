namespace Scorecraft.Sofa.Models
{
    public class RoundInfo : SofaInfo
    {
        public int? Round { get; set; }

        public string Prefix { get; set; }

        public int? Priority { get; set; }

        public bool? Current { get; set; }

        public SeasonInfo Season { get; set; }
    }
}