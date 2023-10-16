using System.Collections.Generic;
using System.Web.Http;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Api.Controllers
{
    public class GroupController : ApiController
    {
        private readonly ISofaScraper<SofaGroup, GroupInfo> Scraper;

        public GroupController(ISofaScraper<SofaGroup, GroupInfo> scraper)
        {
            Scraper = scraper;
        }

        [HttpGet]
        [Route("api/group/load/{competition}/{season}")]
        public IEnumerable<GroupInfo> Load(int season, int competition)
        {
            ((GroupScraper)Scraper).Season = new SeasonInfo()
            {
                Id = season,
                Competition = new CompetitionInfo()
                {
                    Id = competition,
                },
            };

            return Scraper.LoadData() ? Scraper.Data : null;
        }

        [HttpGet]
        [Route("api/group/sync/{competition}/{season}")]
        public IEnumerable<GroupInfo> Sync(int season, int competition)
        {
            ((GroupScraper)Scraper).Season = new SeasonInfo()
            {
                Id = season,
                Competition = new CompetitionInfo()
                {
                    Id = competition,
                },
            };

            if (Scraper.LoadData())
            {
                Scraper.SyncData();
                return Scraper.Data;
            }
            return null;
        }
    }
}