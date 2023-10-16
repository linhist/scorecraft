using System;
using System.Collections.Generic;
using Scorecraft.Data;
using Scorecraft.Sofa.Models;
using Scorecraft.Utilities;

namespace Scorecraft.Sofa.Scrapers
{
    //view-source:https://www.sofascore.com/tournament/football/europe/uefa-champions-league/7#41897
    //--> HTML --> <script id="__NEXT_DATA__" type="application/json">...
    //{"props":{
    //    "pageProps":{
    //        "uniqueTournament":{
    //            "name":"UEFA Champions League","slug":"uefa-champions-league","primaryColorHex":"#062b5c","secondaryColorHex":"#086aab",
    //            "logo":{"md5":"20da94def999f8815e9c4d4b46f92c01","id":1417929},"darkLogo":{"md5":"58dc0a8e3de711eaca3280f92949f83c","id":1417945},
    //            ...
    //        },
    //        "seasons":[
    //            {"name":"UEFA Champions League 23/24","year":"23/24","editor":false,"id":52162},
    //            {"name":"UEFA Champions League 22/23","year":"22/23","editor":false,"seasonCoverageInfo":{},"id":41897},
    //            {"name":"UEFA Champions League 21/22","year":"21/22","editor":false,"id":36886},
    //            ...
    //        ],
    // ...
    public class SeasonScraper : SofaScraper<SofaSeason, SeasonInfo>
    {
        private readonly IRepository<SofaCompetition> _comRepos;

        public CompetitionInfo Competition { get; set; }

        public SeasonScraper(ISofaLogger logger, IRepository<SofaSeason> repos, IRepository<SofaCompetition> comRepos)
            : base(logger, repos)
        {
            _comRepos = comRepos;
            UrlPath = "https://www.sofascore.com/tournament/football/{0}/{1}/{2}";
        }

        protected override string GetContent(string url)
        {
            if (Competition == null || Competition.Region == null)
            {
                Logger.AddError("There is no CompetitionInfo specified.", $"{GetType().Name}.GetContent");
                return "";
            }
            
            return base.GetContent(string.Format(UrlPath, Competition.Region.Slug, Competition.Slug, Competition.Id));
        }

        protected override void SaveFile(string content)
        {
            Logger.SaveContent(content, $"competitions\\{Competition.Id}\\seasons.json");
        }

        protected override IEnumerable<SeasonInfo> BindData(string content)
        {
            string nxtData = "<script id=\"__NEXT_DATA__\" type=\"application/json\">";
            int idx = content.IndexOf(nxtData) + nxtData.Length;
            content = content.Substring(idx, content.IndexOf("</script>", idx) - idx).Trim();

            var comp = ParseOnly<CompetitionInfo>(content, "$.props.pageProps.uniqueTournament");
            if (comp == null || Competition.Id != comp.Id) return null;

            Competition.HasStandingsGroups = comp.HasStandingsGroups;
            Competition.HasRounds = comp.HasRounds;
            Competition.HasPlayoffSeries = comp.HasPlayoffSeries;
            Competition.StartDateTimestamp = comp.StartDateTimestamp;
            Competition.EndDateTimestamp = comp.EndDateTimestamp;

            var seasons = ParseJson<SeasonInfo>(content, "$.props.pageProps.seasons");
            foreach (var ses in seasons)
            {
                ses.Competition = Competition;
            }
            return seasons;
        }

        protected override void MergeInfo(SofaSeason entity, SeasonInfo info)
        {
            base.MergeInfo(entity, info);
            entity.Years = info.Year;
            entity.CompetitionId = Competition.Id;
        }

        public new bool SyncData()
        {
            var comp = _comRepos.Select(Competition.Id);
            if (comp == null)
            {
                comp = new SofaCompetition
                {
                    Id = Competition.Id ?? 0,
                    Name = Competition.Name,
                    Slug = Competition.Slug,
                    Logo = Competition.LogoPath,
                    Colors = $"{Competition.PrimaryColorHex}/{Competition.SecondaryColorHex}",
                    RegionId = Competition.Region.Id,
                    HasGroup = Competition.HasStandingsGroups,
                    HasRound = Competition.HasRounds,
                    HasPlayoff = Competition.HasPlayoffSeries,
                    StartDate = TypeHelper.FromJsDateNum(Competition.StartDateTimestamp),
                    EndDate = TypeHelper.FromJsDateNum(Competition.EndDateTimestamp),
                    Last = DateTime.Now,
                };
                _comRepos.Create(comp);
            }
            else
            {
                comp.HasGroup = Competition.HasStandingsGroups;
                comp.HasRound = Competition.HasRounds;
                comp.HasPlayoff = Competition.HasPlayoffSeries;
                comp.StartDate = TypeHelper.FromJsDateNum(Competition.StartDateTimestamp);
                comp.EndDate = TypeHelper.FromJsDateNum(Competition.EndDateTimestamp);
                comp.Last = DateTime.Now;
                _comRepos.Update(comp);
            }

            return base.SyncData();
        }
    }
}