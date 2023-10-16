using System;
using System.Collections.Generic;
using System.Linq;
using Scorecraft.Data;
using Scorecraft.Sofa.Models;

namespace Scorecraft.Sofa.Scrapers
{
    //https://api.sofascore.com/api/v1/unique-tournament/7/season/41897/rounds
    //{"currentRound":{"round":29,"name":"Final","slug":"final"},
    //"rounds":[
    //    {"round":28,"name":"Semifinal","slug":"semifinal","prefix":"Preliminary"},{"round":29,"name":"Final","slug":"final","prefix":"Preliminary"},
    //    {"round":1,"name":"Qualification round 1","slug":"qualification-round-1"},{"round":2,"name":"Qualification round 2","slug":"qualification-round-2"},
    //    {"round":3,"name":"Qualification round 3","slug":"qualification-round-3"},{"round":4,"name":"Qualification round 4","slug":"qualification-round-4"},
    //    {"round":1},{"round":2},{"round":3},{"round":4},{"round":5},{"round":6},{"round":5,"name":"Round of 16","slug":"round-of-16"},
    //    {"round":27,"name":"Quarterfinal","slug":"quarterfinal"},{"round":28,"name":"Semifinal","slug":"semifinal"},
    //    {"round":29,"name":"Final","slug":"final"}
    //]}
    public class RoundScraper : SofaScraper<SofaRound, RoundInfo>
    {
        public SeasonInfo Season { get; set; }

        public RoundScraper(ISofaLogger logger, IRepository<SofaRound> repos)
            : base(logger, repos)
        {
            UrlPath = "https://api.sofascore.com/api/v1/unique-tournament/{0}/season/{1}/rounds";
        }

        protected override string GetContent(string url)
        {
            if (Season == null || Season.Competition == null)
            {
                Logger.AddError("There is no SeasonInfo specified.", $"{GetType().Name}.GetContent");
                return "";
            }

            return base.GetContent(string.Format(url, Season.Competition.Id, Season.Id));
        }

        protected override void SaveFile(string content)
        {
            Logger.SaveContent(content, $"competitions\\{Season.Competition.Id}\\{Season.Id}--rounds.json");
        }

        protected override IEnumerable<RoundInfo> BindData(string content)
        {
            int idx = 0;
            RoundInfo cur = null;
            try
            {
                cur = ParseOnly<RoundInfo>(content, "$.currentRound");
            }
            catch { }

            var rounds = ParseJson<RoundInfo>(content, "$.rounds");
            if (rounds == null) return rounds;

            foreach (RoundInfo rnd in rounds)
            {
                rnd.Season = Season;
                rnd.Priority = idx++;
                rnd.Current = (cur != null && rnd.Round == cur.Round && rnd.Name == cur.Name && rnd.Slug == cur.Slug);
                if (rnd.Round != null && string.IsNullOrEmpty(rnd.Name) && string.IsNullOrEmpty(rnd.Slug))
                {
                    rnd.Name = $"Week {rnd.Round}";
                    rnd.Slug = rnd.Round.ToString();
                }
            }

            return rounds;
        }

        protected override List<SofaRound> GetEntities()
        {
            return Repos.Search(e => e.SeasonId == Season.Id).ToList();
        }

        protected override bool IsMatch(SofaRound entity, RoundInfo info)
        {
            return entity?.Name == info.Name && entity?.RoundNo == info?.Round && entity?.Prefix == info?.Prefix;
        }

        protected override void MergeInfo(SofaRound entity, RoundInfo info)
        {
            entity.SeasonId = Season.Id;
            entity.Name = info.Name;
            entity.Slug = info.Slug;
            entity.Prefix = info.Prefix;
            entity.RoundNo = info.Round;
            entity.Priority = info.Priority;
            entity.Current = info.Current;
            entity.Last = DateTime.Now;
        }
    }
}
