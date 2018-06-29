using System.Collections.Generic;

namespace IsoFinder.Web.Models
{
    public class SearchResultInfo
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }

        public List<SearchInfo> Results { get; set; }
    }
}