using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoFinder.Core
{
    [Table("IsoFile")]
    public class IsoFile
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Extension { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public DateTime Modified { get; set; }

        [Required]
        public decimal Size { get; set; }

        [Required]
        public virtual IsoFolder Parent { get; set; }

        [Required]
        public int IsoVolumeId { get; set; }

        public virtual ICollection<IsoRequest> IsoRequests { get; set; }
    }
}