using System.Collections.Generic;
using Scorecraft.Data;

namespace Scorecraft.Sofa
{
    public interface ISofaScraper<TEnt, TInf>
        where TEnt : SofaEntity
        where TInf : SofaInfo
    {
        bool HasMedia { get; set; }

        IEnumerable<TInf> Data { get; }

        IRepository<TEnt> Repos { get; }

        bool LoadData();

        bool SyncData();

        void SaveMedia(TInf info);
    }
}