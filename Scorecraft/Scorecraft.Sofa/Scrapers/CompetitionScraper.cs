using Scorecraft.Data;
using Scorecraft.Sofa.Models;
using System.Collections.Generic;

namespace Scorecraft.Sofa.Scrapers
{
    //https://api.sofascore.com/api/v1/category/289/unique-tournaments
    //{"groups":[
    //    {"uniqueTournaments":[
    //        {"name":"Primera Division, Apertura","slug":"primera-division-apertura","primaryColorHex":"#02663a","secondaryColorHex":"#91b498","category":{"name":"Costa Rica","slug":"costa-rica","sport":{"name":"Football","slug":"football","id":1},"id":289,"flag":"costa-rica","alpha2":"CR"},"userCount":4878,"id":11535,"displayInverseHomeAwayTeams":false},
    //        {"name":"Primera Division, Clausura","slug":"primera-division-clausura","primaryColorHex":"#02663a","secondaryColorHex":"#91b498","category":{"name":"Costa Rica","slug":"costa-rica","sport":{"name":"Football","slug":"football","id":1},"id":289,"flag":"costa-rica","alpha2":"CR"},"userCount":4921,"id":11542,"displayInverseHomeAwayTeams":false},
    //        {"name":"Liga de Ascenso, Apertura","slug":"liga-de-ascenso-apertura","primaryColorHex":"#931a24","secondaryColorHex":"#f07c3c","category":{"name":"Costa Rica","slug":"costa-rica","sport":{"name":"Football","slug":"football","id":1},"id":289,"flag":"costa-rica","alpha2":"CR"},"userCount":841,"id":11671,"displayInverseHomeAwayTeams":false},
    //        {"name":"Liga de Ascenso, Clausura","slug":"liga-de-ascenso-clausura","primaryColorHex":"#931a24","secondaryColorHex":"#f07c3c","category":{"name":"Costa Rica","slug":"costa-rica","sport":{"name":"Football","slug":"football","id":1},"id":289,"flag":"costa-rica","alpha2":"CR"},"userCount":988,"id":11672,"displayInverseHomeAwayTeams":false}
    //    ]}
    //]}
    public class CompetitionScraper : SofaScraper<SofaCompetition, CompetitionInfo>
    {
        private readonly string MdaPath = "https://api.sofascore.app/api/v1/unique-tournament/{0}/image";

        public RegionInfo Region { get; set; }

        public CompetitionScraper(ISofaLogger logger, IRepository<SofaCompetition> repos)
            : base(logger, repos)
        {
            UrlPath = "https://api.sofascore.com/api/v1/category/{0}/unique-tournaments";
        }

        protected override string GetContent(string url)
        {
            if (Region == null)
            {
                Logger.AddError("There is no RegionInfo specified.", $"{GetType().Name}.GetContent");
                return "";
            }
            
            return base.GetContent(string.Format(UrlPath, Region.Id));
        }

        protected override IEnumerable<CompetitionInfo> BindData(string content)
        {
            var comps = ParseJson<CompetitionInfo>(content, "$.groups[*].uniqueTournaments");
            foreach (CompetitionInfo com in comps)
            {
                com.Region = Region;
            }
            return comps;
        }

        protected override void SaveFile(string content)
        {
            Logger.SaveContent(content, $"{Region.Id}--competitons.json");
        }

        protected override void MergeInfo(SofaCompetition entity, CompetitionInfo info)
        {
            base.MergeInfo(entity, info);
            entity.Slug = info.Slug;
            entity.Colors = $"{info.PrimaryColorHex}/{info.SecondaryColorHex}";
            entity.RegionId = Region.Id;

            if (!string.IsNullOrEmpty(info.LogoPath))
            {
                entity.Logo = info.LogoPath;
            }
        }

        public override void SaveMedia(CompetitionInfo info)
        {
            info.LogoPath = $"\\logos\\{info.Id}.png";
            Logger.SaveResource(string.Format(MdaPath, info.Id), info.LogoPath);
        }
    }
}
