using System.Collections.Generic;

namespace IsoFinder.Model
{
    public class SearchResultInfo<T> where T : class
    {
        public int Total { get; set; }

        public List<T> Results { get; set; }

        public SearchResultInfo()
        {
            Results = new List<T>();
        }
    }
}