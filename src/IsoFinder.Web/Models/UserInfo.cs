using System.Collections.Generic;

namespace IsoFinder.Web.Models
{
    public class UserInfo
    {
        public UserInfo()
        {
            CartFileItems = new List<IsoFileInfo>();
            CartFolderItems = new List<IsoFolderInfo>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
    
        public List<IsoFolderInfo> CartFolderItems { get; set; }

        public List<IsoFileInfo> CartFileItems { get; set; }
    }
}