using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoFinder.Core
{
    [Table("IsoRequest")]
    public class IsoRequest
    {
        public IsoRequest()
        {
            Files = new List<IsoFile>();
            Folders = new List<IsoFolder>();
        }

        public int Id { get; set; }

        [Required]
        public DateTime Created { get; set; }
        
        public DateTime Completed { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; }

        public string Query { get; set; }

        [Required]
        public IsoRequestStatus Status { get; set; }

        public virtual ICollection<IsoFile> Files { get; set; }

        public virtual ICollection<IsoFolder> Folders { get; set; }

        [Required]
        public virtual IsoUser User { get; set; }
    }
}