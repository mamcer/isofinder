using System.Collections.Generic;

namespace ISOFinder.Domain.MainModule
{
    public class SearchResults
    {
        public int Count { get; set; }

        public IEnumerable<FileDto> Files { get; set; }
    }
}