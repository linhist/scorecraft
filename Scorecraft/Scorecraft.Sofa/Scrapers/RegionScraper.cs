using System.Collections.Generic;
using Scorecraft.Data;
using Scorecraft.Sofa.Models;

namespace Scorecraft.Sofa.Scrapers
{
    //https://api.sofascore.com/api/v1/sport/football/categories
    //{"categories":[
    //    {"name":"Northern Ireland","slug":"northern-ireland","sport":{"name":"Football","slug":"football","id":1},"priority":0,"id":130,"flag":"northern-ireland","alpha2":"NX"},
    //    {"name":"Wales","slug":"wales","sport":{"name":"Football","slug":"football","id":1},"priority":0,"id":131,"flag":"wales","alpha2":"WA"},
    //    {"name":"Morocco","slug":"morocco","sport":{"name":"Football","slug":"football","id":1},"priority":0,"id":303,"flag":"morocco","alpha2":"MA"},
    //    {"name":"Armenia","slug":"armenia","sport":{"name":"Football","slug":"football","id":1},"priority":0,"id":296,"flag":"armenia","alpha2":"AM"},
    //    {"name":"Costa Rica","slug":"costa-rica","sport":{"name":"Football","slug":"football","id":1},"priority":0,"id":289,"flag":"costa-rica","alpha2":"CR"}
    //    ...
    //]}
    public class RegionScraper : SofaScraper<SofaRegion,RegionInfo>
    {
        private readonly string MdaPath = "https://www.sofascore.com/static/images/flags/{0}.png";

        public bool SaveFlag { get; set; }

        public RegionScraper(ISofaLogger logger, IRepository<SofaRegion> repos)
            : base(logger, repos)
        {
            SaveFlag = false;
            UrlPath = "https://api.sofascore.com/api/v1/sport/football/categories";
        }

        protected override IEnumerable<RegionInfo> BindData(string content)
        {
            return ParseJson<RegionInfo>(content, "$.categories");
        }

        protected override void SaveFile(string content)
        {
            Logger.SaveContent(content, "regions.json");
        }

        protected override void MergeInfo(SofaRegion entity, RegionInfo info)
        {
            base.MergeInfo(entity, info);
            entity.Slug = info.Slug;
            entity.Code = info.Alpha2;
            entity.Priority = info.Priority;

            if (!string.IsNullOrEmpty(info.Flag))
            {
                entity.Flag = info.Flag;
            }
        }

        public override void SaveMedia(RegionInfo info)
        {
            info.Flag = $"\\flags\\{info.Id}.png";
            Logger.SaveResource(string.Format(MdaPath, info.Name), info.Flag);
        }
    }
}
