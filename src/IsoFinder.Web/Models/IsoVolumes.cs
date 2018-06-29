using System.Collections.Generic;

namespace IsoFinder.Web.Models
{
    public class IsoVolumes
    {
        public List<IsoVolumeInfo> Items { get; set; }

        public IsoVolumes()
        {
            Items = new List<IsoVolumeInfo>();
        }
    }
}