using System.Collections.Generic;

namespace IsoFinder.Model
{
    public class IsoFinderFolderRequest : IsoFinderRequest
    {
        public IList<int> FolderIds { get; set; }
    }
}