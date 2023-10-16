using System.Collections.Generic;
using System.Linq;

namespace Scorecraft.Entities
{
    public class Pagination<T> : List<T>
        where T : class
    {
        public int PageIndex { get; }

        public int PageSize { get; }

        public int PageCount { get; }

        public int TotalCount { get; }

        public Pagination(int pgIndex = 0, int pgSize = 10, int itmCount = 0)
            : base()
        {
            PageIndex = pgIndex < 1 ? 0 : pgIndex;
            TotalCount = itmCount < 1 ? 0 : itmCount;
            PageSize = pgSize < 1 ? TotalCount : pgSize;
            PageCount = PageSize < 1 ? 0 : TotalCount / PageSize + (TotalCount % PageSize > 0 ? 1 : 0);
        }

        public Pagination(IEnumerable<T> items, int pgIndex = 0, int itmCount = 0)
            : base(items)
        {
            PageIndex = pgIndex < 1 ? 0 : pgIndex;
            PageSize = items.Count();
            PageCount = pgIndex;
            TotalCount = itmCount < 1 ? (pgIndex + 1) * PageSize : itmCount;
        }
    }
}
