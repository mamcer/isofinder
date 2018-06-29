using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoFinder.Core
{
    [Table("IsoFolder")]
    public class IsoFolder
    {
        public IsoFolder()
        {
            ChildFiles = new HashSet<IsoFile>();
            ChildFolders = new HashSet<IsoFolder>();
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Path { get; set; }

        public virtual IsoFolder Parent { get; set; }

        [Required]
        public int IsoVolumeId { get; set; }

        public virtual ICollection<IsoFolder> ChildFolders { get; set; }

        public virtual ICollection<IsoFile> ChildFiles { get; set; }

        public virtual ICollection<IsoRequest> IsoRequests { get; set; }
    }
}