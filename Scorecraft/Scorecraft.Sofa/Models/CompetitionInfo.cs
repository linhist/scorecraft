namespace Scorecraft.Sofa.Models
{
    public class CompetitionInfo : SofaInfo
    {
        public string PrimaryColorHex { get; set; }

        public string SecondaryColorHex { get; set; }

        public string LogoPath { get; set; }

        public bool? HasStandingsGroups { get; set; }

        public bool? HasRounds { get; set; }

        public bool? HasPlayoffSeries { get; set; }

        public int? StartDateTimestamp { get; set; }

        public int? EndDateTimestamp { get; set; }

        public RegionInfo Region { get; set; }
    }
}