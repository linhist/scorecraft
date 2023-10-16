using System.Collections.Generic;
using Scorecraft.Data;
using Scorecraft.Sofa.Models;

namespace Scorecraft.Sofa.Scrapers
{
    //https://api.sofascore.com/api/v1/unique-tournament/7/season/41897/groups
    //{"groups":[
    //    {"tournamentId":1462,"groupName":"Group A"},{"tournamentId":1463,"groupName":"Group B"},
    //    {"tournamentId":1464,"groupName":"Group C"},{"tournamentId":1465,"groupName":"Group D"},
    //    {"tournamentId":1466,"groupName":"Group E"},{"tournamentId":1467,"groupName":"Group F"},
    //    {"tournamentId":1468,"groupName":"Group G"},{"tournamentId":1469,"groupName":"Group H"}
    //]}
    public class GroupScraper : SofaScraper<SofaGroup, GroupInfo>
    {
        public SeasonInfo Season { get; set; }

        public GroupScraper(ISofaLogger logger, IRepository<SofaGroup> repos)
            : base(logger, repos)
        {
            UrlPath = "https://api.sofascore.com/api/v1/unique-tournament/{0}/season/{1}/groups";
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
            Logger.SaveContent(content, $"competitions\\{Season.Competition.Id}\\{Season.Id}--groups.json");
        }

        protected override IEnumerable<GroupInfo> BindData(string content)
        {
            int idx = 0;
            IEnumerable<GroupInfo> groups = ParseJson<GroupInfo>(content, "$.groups");
            if (groups == null) return groups;

            foreach (GroupInfo grp in groups)
            {
                grp.Id = grp.TournamentId;
                grp.Name = grp.GroupName;
                grp.Season = Season;
                grp.Priority = idx++;
            }

            return groups;
        }

        protected override void MergeInfo(SofaGroup entity, GroupInfo info)
        {
            base.MergeInfo(entity, info);
            entity.SeasonId = Season.Id;
            entity.Priority = info.Priority;
        }
    }
}
