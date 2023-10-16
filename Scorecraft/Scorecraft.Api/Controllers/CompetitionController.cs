using System.Collections.Generic;
using System.Web.Http;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Api.Controllers
{
    public class CompetitionController : ApiController
    {
        private readonly ISofaScraper<SofaCompetition, CompetitionInfo> Scraper;

        public CompetitionController(ISofaScraper<SofaCompetition, CompetitionInfo> scraper)
        {
            Scraper = scraper;
        }

        [HttpGet]
        [Route("api/competition/load/{region}")]
        public IEnumerable<CompetitionInfo> Load(int region)
        {
            //Scraper.HasMedia = true;
            ((CompetitionScraper) Scraper).Region = new RegionInfo()
            {
                Id = region
            };

            return Scraper.LoadData() ? Scraper.Data : null;
        }

        [HttpGet]
        [Route("api/competition/sync/{region}")]
        public IEnumerable<CompetitionInfo> Sync(int region)
        {
            Scraper.HasMedia = true;
            ((CompetitionScraper)Scraper).Region = new RegionInfo()
            {
                Id = region
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
