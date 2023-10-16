using System.Collections.Generic;
using System.Web.Http;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Api.Controllers
{
    public class RoundController : ApiController
    {
        private readonly ISofaScraper<SofaRound, RoundInfo> Scraper;

        public RoundController(ISofaScraper<SofaRound, RoundInfo> scraper)
        {
            Scraper = scraper;
        }

        [HttpGet]
        [Route("api/round/load/{competition}/{season}")]
        public IEnumerable<RoundInfo> Load(int season, int competition)
        {
            ((RoundScraper)Scraper).Season = new SeasonInfo()
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
        [Route("api/round/sync/{competition}/{season}")]
        public IEnumerable<RoundInfo> Sync(int season, int competition)
        {
            ((RoundScraper)Scraper).Season = new SeasonInfo()
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