using System.Collections.Generic;
using System.Data.Entity;
using Scorecraft.Data;
using Scorecraft.Sofa.Models;
using Scorecraft.Sofa.Scrapers;

namespace Scorecraft.Sofa
{
    public class SofaService
    {
        //https://api.sofascore.com/api/v1/tournament/1463/season/41897/events
        //{"events":[
        //    {"tournament":{
        //            "name":"UEFA Champions League, Group B","slug":"uefa-champions-league-group-b",
        //            "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //            "uniqueTournament":{
        //                "name":"UEFA Champions League","slug":"uefa-champions-league","primaryColorHex":"#062b5c","secondaryColorHex":"#086aab",
        //                "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //                "userCount":1094225,"id":7,"hasEventPlayerStatistics":true,"crowdsourcingEnabled":false,"hasPerformanceGraphFeature":false,"displayInverseHomeAwayTeams":false
        //            },
        //            "priority":555,"id":1463
        //        },
        //        "roundInfo":{"round":1},
        //        "customId":"Lgbsckb","status":{"code":100,"description":"Ended","type":"finished"},"winnerCode":1,
        //        "homeTeam":{"name":"Atl\u00e9tico Madrid","slug":"atletico-madrid","shortName":"Atl. Madrid","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":497356,"nameCode":"ATM","disabled":false,"national":false,"type":0,"id":2836,"subTeams":[],"teamColors":{"primary":"#ffffff","secondary":"#c40000","text":"#c40000"}},
        //        "awayTeam":{"name":"FC Porto","slug":"fc-porto","shortName":"Porto","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":265674,"nameCode":"FCP","disabled":false,"national":false,"type":0,"id":3002,"subTeams":[],"teamColors":{"primary":"#194f93","secondary":"#ffffff","text":"#ffffff"}},
        //        "homeScore":{"current":2,"display":2,"period1":0,"period2":2,"normaltime":2},"awayScore":{"current":1,"display":1,"period1":0,"period2":1,"normaltime":1},
        //        "time":{"injuryTime1":1,"injuryTime2":9,"currentPeriodStartTimestamp":1662584506},
        //        "changes":{"changes":["status.code","status.description","status.type","homeScore.period2","homeScore.normaltime","awayScore.period2","awayScore.normaltime","time.currentPeriodStart"],"changeTimestamp":1662584510},
        //        "hasGlobalHighlights":false,"hasXg":true,"hasEventPlayerStatistics":true,"hasEventPlayerHeatMap":true,"detailId":1,"crowdsourcingDataDisplayEnabled":false,
        //        "id":10640472,"awayRedCards":1,"startTimestamp":1662577200,"slug":"fc-porto-atletico-madrid","finalResultOnly":false
        //    },
        //    ...
        //],
        //"hasNextPage":false}
        public static readonly string SCORE_PATH = "https://api.sofascore.com/api/v1/tournament/{0}/season/{1}/events"; //{0}: LEAGUE_ID (UniqueID), {1}: SEASON_ID

        //https://api.sofascore.com/api/v1/unique-tournament/7/season/41897/standings/total
        //{"standings":[
        //    {"tournament":{
        //            "name":"UEFA Champions League, Group A","slug":"uefa-champions-league-group-a",
        //            "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //            "uniqueTournament":{
        //                "name":"UEFA Champions League","slug":"uefa-champions-league","primaryColorHex":"#062b5c","secondaryColorHex":"#086aab",
        //                "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //                "userCount":1094225,"hasPerformanceGraphFeature":false,"id":7,"displayInverseHomeAwayTeams":false},"priority":556,"id":1462
        //            },
        //            "type":"total","name":"Group A","descriptions":[],
        //            "tieBreakingRule":{"text":"At the end of the group phase, in the event that two (or more) teams have an equal number of points the following rules break the tie: 1. Head-to-head 2. Goal difference 3. Goals scored During the group phase, the following tie-breaking procedures are used: 1. Goal difference 2. Goals scored","id":49
        //        },
        //        "rows":[
        //            {"team":{"name":"Napoli","slug":"napoli","shortName":"Napoli","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":484887,"nameCode":"NAP","disabled":false,"national":false,"type":0,"id":2714,"teamColors":{"primary":"#008cea","secondary":"#ffffff","text":"#ffffff"}},"descriptions":[],"promotion":{"text":"Playoffs","id":6},"position":1,"matches":6,"wins":5,"scoresFor":20,"scoresAgainst":6,"id":758533,"losses":1,"draws":0,"points":15},
        //            {"team":{"name":"Liverpool","slug":"liverpool","shortName":"Liverpool","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":1071772,"nameCode":"LIV","disabled":false,"national":false,"type":0,"id":44,"teamColors":{"primary":"#cc0000","secondary":"#ffffff","text":"#ffffff"}},"descriptions":[],"promotion":{"text":"Playoffs","id":6},"position":2,"matches":6,"wins":5,"scoresFor":17,"scoresAgainst":6,"id":758532,"losses":1,"draws":0,"points":15},
        //            {"team":{"name":"Ajax","slug":"ajax","shortName":"Ajax","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":307128,"nameCode":"AJA","disabled":false,"national":false,"type":0,"id":2953,"teamColors":{"primary":"#bb002b","secondary":"#ffffff","text":"#ffffff"}},"descriptions":[],"promotion":{"text":"UEFA Europa League","id":808},"position":3,"matches":6,"wins":2,"scoresFor":11,"scoresAgainst":16,"id":758530,"losses":4,"draws":0,"points":6},
        //            {"team":{"name":"Rangers","slug":"rangers","shortName":"Rangers","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":136343,"nameCode":"RFC","disabled":false,"national":false,"type":0,"id":2351,"teamColors":{"primary":"#0033a0","secondary":"#ff0000","text":"#ff0000"}},"descriptions":[],"position":4,"matches":6,"wins":0,"scoresFor":2,"scoresAgainst":22,"id":758531,"losses":6,"draws":0,"points":0}
        //        ],
        //        "id":92473,"updatedAtTimestamp":1667339482
        //    },
        //    ...
        //]}
        public static readonly string RANK_IN_GROUP_PATH = "https://api.sofascore.com/api/v1/unique-tournament/{0}/season/{1}/standings/total"; //{0}: LEAGUE_ID, {1}: SEASON_ID

        //https://api.sofascore.com/api/v1/unique-tournament/7/season/41897/team-events/total
        //{"tournamentTeamEvents":{
        //    "1462":{
        //        "44":[
        //            {"tournament":{
        //                    "name":"UEFA Champions League, Group A","slug":"uefa-champions-league-group-a",
        //                    "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //                    "uniqueTournament":{
        //                        "name":"UEFA Champions League","slug":"uefa-champions-league","primaryColorHex":"#062b5c","secondaryColorHex":"#086aab",
        //                        "category":{"name":"Europe","slug":"europe","sport":{"name":"Football","slug":"football","id":1},"id":1465,"flag":"europe"},
        //                        "userCount":1094225,"id":7,"displayInverseHomeAwayTeams":false
        //                    },
        //                    "priority":556,"id":1462
        //                },
        //                "customId":"Usoeb","status":{"code":100,"description":"Ended","type":"finished"},"winnerCode":1,
        //                "homeTeam":{"name":"Liverpool","slug":"liverpool","shortName":"Liverpool","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":1071772,"nameCode":"LIV","disabled":false,"national":false,"type":0,"id":44,"teamColors":{"primary":"#cc0000","secondary":"#ffffff","text":"#ffffff"}},
        //                "awayTeam":{"name":"Napoli","slug":"napoli","shortName":"Napoli","gender":"M","sport":{"name":"Football","slug":"football","id":1},"userCount":484887,"nameCode":"NAP","disabled":false,"national":false,"type":0,"id":2714,"teamColors":{"primary":"#008cea","secondary":"#ffffff","text":"#ffffff"}},
        //                "homeScore":{"current":2,"display":2,"period1":0,"period2":2,"normaltime":2},"awayScore":{"current":0,"display":0,"period1":0,"period2":0,"normaltime":0},
        //                "hasXg":true,"id":10640564,"startTimestamp":1667332800,"slug":"liverpool-napoli","finalResultOnly":false
        //            },
        //            ...
        //        ]
        //    }
        //}}
        public static readonly string MATCH_IN_GROUP_PATH = "https://api.sofascore.com/api/v1/unique-tournament/{0}/season/{1}/team-events/total"; //{0}: LEAGUE_ID, {1}: SEASON_ID

        //https://api.sofascore.com/api/v1/unique-tournament/7/season/52162/events/round/29/slug/final/prefix/Preliminary
        //https://api.sofascore.com/api/v1/unique-tournament/7/season/52162/events/round/1/slug/qualification-round-1
        //https://api.sofascore.com/api/v1/unique-tournament/7/season/52162/events/round/1

        public static readonly string MATCH_PATH = "";

        private SofaLogger Logger { get; }

        protected virtual DbContext CreateContext() => new SCDbContext();

        public SofaService()
        {
            Logger = new SofaLogger();
        }

        public IEnumerable<RegionInfo> SyncRegions()
        {
            using (DbContext context = CreateContext())
            {
                var repos = new Repository<SofaRegion>(context);
                var scraper = new RegionScraper(Logger, repos);
                if (!scraper.LoadData()) return null;

                scraper.SyncData();
                return scraper.Data;
            }
        }

        public IEnumerable<CompetitionInfo> SyncCompetitions(RegionInfo region)
        {
            using (DbContext context = CreateContext())
            {
                var repos = new Repository<SofaCompetition>(context);
                var scraper = new CompetitionScraper(Logger, repos)
                {
                    Region = region
                };
                if (!scraper.LoadData()) return null;

                scraper.SyncData();
                return scraper.Data;
            }
        }

        public IEnumerable<SeasonInfo> SyncSeasons(CompetitionInfo competition)
        {
            using (DbContext context = CreateContext())
            {
                var compRepo = new Repository<SofaCompetition>(context);
                var repos = new Repository<SofaSeason>(context);
                var scraper = new SeasonScraper(Logger, repos, compRepo)
                {
                    Competition = competition
                };
                if (!scraper.LoadData()) return null;

                scraper.SyncData();
                return scraper.Data;
            }
        }

        public IEnumerable<RoundInfo> SyncRounds(SeasonInfo season)
        {
            using (DbContext context = CreateContext())
            {
                var repos = new Repository<SofaRound>(context);
                var scraper = new RoundScraper(Logger, repos)
                {
                    Season = season
                };
                if (!scraper.LoadData()) return null;

                scraper.SyncData();
                return scraper.Data;
            }
        }

        public IEnumerable<GroupInfo> SyncGroups(SeasonInfo season)
        {
            using (DbContext context = CreateContext())
            {
                var repos = new Repository<SofaGroup>(context);
                var scraper = new GroupScraper(Logger, repos)
                {
                    Season = season
                };
                if (!scraper.LoadData()) return null;

                scraper.SyncData();
                return scraper.Data;
            }
        }
    }
}