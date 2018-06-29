using System;
using System.Collections.Generic;

namespace IsoFinder.Core
{
    public class PagedResult<TEntity>
    {
        public PagedResult(IEnumerable<TEntity> items, int totalCount)
        {
            Items = items ?? throw new ArgumentNullException("items");
            TotalCount = totalCount;
        }

        public IEnumerable<TEntity> Items { get; }

        public int TotalCount { get; }
    }
}