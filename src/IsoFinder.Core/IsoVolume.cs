using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsoFinder.Core
{
    [Table("IsoVolume")]
    public class IsoVolume
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(255)]
        public string VolumeLabel { get; set; }

        [Required]
        public int FileCount { get; set; }

        [Required]
        public decimal Size { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public virtual IsoFolder RootFolder { get; set; }
    }
}