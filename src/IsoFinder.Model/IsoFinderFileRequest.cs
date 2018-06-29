using System.Collections.Generic;

namespace IsoFinder.Model
{
    public class IsoFinderFileRequest : IsoFinderRequest
    {
        public IList<int> FileIds { get; set; }
    }
}