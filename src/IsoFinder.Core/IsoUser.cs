using System;
using System.ComponentModel.DataAnnotations;

namespace IsoFinder.Core
{
    public class IsoUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}