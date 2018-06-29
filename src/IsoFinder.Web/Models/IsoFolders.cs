using System.Collections.Generic;

namespace IsoFinder.Web.Models
{
    public class IsoFolders
    {
        public int IsoVolumeId { get; set; }

        public string IsoVolumeName { get; set; }

        public int IsoFolderId { get; set; }

        public string IsoFolderName { get; set; }

        public IEnumerable<IsoFolderInfo> FolderItems { get; set; }

        public IEnumerable<IsoFileInfo> FileItems { get; set; }
    }
}