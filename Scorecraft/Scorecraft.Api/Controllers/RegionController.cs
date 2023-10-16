using System.Collections.Generic;
using System.Web.Http;
using Scorecraft.Data;
using Scorecraft.Sofa;
using Scorecraft.Sofa.Models;

namespace Scorecraft.Api.Controllers
{
    public class RegionController : ApiController
    {
        private readonly ISofaScraper<SofaRegion, RegionInfo> Scraper;

        public RegionController(ISofaScraper<SofaRegion, RegionInfo> scraper)
        {
            Scraper = scraper;
        }

        [HttpGet]
        [Route("api/region/load")]
        public IEnumerable<RegionInfo> Load()
        {
            //Scraper.HasMedia = true;
            return Scraper.LoadData() ? Scraper.Data : null;
        }

        [HttpGet]
        [Route("api/region/sync")]
        public IEnumerable<RegionInfo> Sync()
        {
            Scraper.HasMedia = true;
            if (Scraper.LoadData())
            {
                Scraper.SyncData();
                return Scraper.Data;
            }
            return null;
        }
    }
}
