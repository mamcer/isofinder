using System;

namespace IsoFinder.Model
{
    public class IsoFinderRequestStatus
    {
        public string Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Completed { get; set; }

        public string DownloadLink { get; set; }
    }
}
