using System.Collections.Generic;
using System.Web.Http;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Api.Controllers
{
    public class SeasonController : ApiController
    {
        private readonly ISofaScraper<SofaSeason, SeasonInfo> Scraper;

        public SeasonController(ISofaScraper<SofaSeason, SeasonInfo> scraper)
        {
            Scraper = scraper;
        }

        [HttpGet]
        [Route("api/season/load/{competition}")]
        public IEnumerable<SeasonInfo> Load(int competition)
        {
            ((SeasonScraper)Scraper).Competition = new CompetitionInfo()
            {
                Id = competition,
                Slug = "competition",
                Region = new RegionInfo()
                {
                    Id = 0,
                    Slug = "category"
                }
            };

            return Scraper.LoadData() ? Scraper.Data : null;
        }

        [HttpGet]
        [Route("api/season/sync/{competition}")]
        public IEnumerable<SeasonInfo> Sync(int competition)
        {
            ((SeasonScraper)Scraper).Competition = new CompetitionInfo()
            {
                Id = competition,
                Slug = "competition",
                Region = new RegionInfo()
                {
                    Id = 0,
                    Slug = "category"
                }
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
