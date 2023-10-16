using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Scorecraft.Data;

namespace Scorecraft.Sofa
{
    public class SofaScraper<TEnt, TInf> : ISofaScraper<TEnt, TInf>
        where TEnt : SofaEntity, new()
        where TInf : SofaInfo
    {
        protected string FilePath { get; set; }

        protected string UrlPath { get; set; }

        protected ISofaLogger Logger { get; }

        public bool HasMedia { get; set; }

        public IEnumerable<TInf> Data { get; protected set; }

        public IRepository<TEnt> Repos { get; }

        public SofaScraper(ISofaLogger logger, IRepository<TEnt> repos)
        {
            HasMedia = false;
            Logger = logger;
            Data = new List<TInf>();
            Repos = repos;
        }

        protected T ParseOnly<T>(string content, string jpath = "")
        {
            JObject json = JObject.Parse(content);
            if (json == null) return default;

            var token = json.SelectToken(jpath ?? "$");
            return token.ToObject<T>();
        }

        protected IEnumerable<T> ParseJson<T>(string content, string jpath = "")
        {
            JObject json = JObject.Parse(content);
            if (json == null) return default;

            return json.SelectToken(jpath ?? "$").Children().Select(j => j.ToObject<T>()).ToList();
        }

        protected virtual string GetContent(string url)
        {
            string content = "";
            if (string.IsNullOrEmpty(url))
            {
                Logger.AddError("Url path is empty", $"{GetType().Name}.GetContent");
            }

            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = new CookieContainer();
                req.Accept = "text/html,application/xhtml+xml,application/xml,application/json";
                req.KeepAlive = true;
                req.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/115.0";

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                {
                    using (var reader = new StreamReader(res.GetResponseStream()))
                    {
                        content = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddError(ex.Message, $"{GetType().Name}.GetContent");
            }

            return content?.Trim();
        }

        protected virtual IEnumerable<TInf> BindData(string content)
        {
            return ParseJson<TInf>(content);
        }

        protected virtual void SaveFile(string content)
        {
            Logger.SaveContent(content, "");
        }

        protected virtual List<TEnt> GetEntities()
        {
            var lstIds = Data.Select(d => d.Id).ToList();
            return Repos.Search(e => lstIds.Contains(e.Id)).ToList();
        }

        protected virtual bool IsMatch(TEnt entity, TInf info)
        {
            return (entity?.Id == info?.Id);
        }

        protected virtual void MergeInfo(TEnt entity, TInf info)
        {
            entity.Name = info.Name;
            entity.Last = DateTime.Now;
        }

        protected TEnt MatchEntity(List<TEnt> entities, TInf info)
        {
            TEnt ent;
            for (int i = 0; i < entities.Count; i++)
            {
                if (IsMatch(entities[i], info))
                {
                    ent = entities[i];
                    MergeInfo(ent, info);

                    entities.RemoveAt(i);
                    return ent;
                }
            }

            ent = new TEnt
            {
                Id = info.Id ?? 0,
            };
            MergeInfo(ent, info);
            return ent;
        }

        public virtual void SaveMedia(TInf info)
        { }

        public bool LoadData()
        {
            Data = null;

            try
            {
                string content = GetContent(UrlPath);
                if (string.IsNullOrEmpty(content)) return false;

                Data = BindData(content);
                SaveFile(content);

                if (HasMedia && Logger.AllowSave)
                {
                    foreach (var info in Data)
                    {
                        SaveMedia(info);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", $"{GetType().Name}.LoadData");
            }

            return false;
        }

        public bool SyncData()
        {
            try
            {
                List<TEnt> ents = GetEntities();
                var lst = Data.Select(d => MatchEntity(ents, d));
                Repos.UpdateAll(lst);
                Repos.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}", $"{GetType().Name}.SyncData");
            }

            return false;
        }
    }
}
