using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace course.Models
{
    public class Clothing
    {
        [Key]
        [Column(TypeName = "integer")]
        [Required]
        public int ClothingId { get; set; }

        [Column(TypeName = "varchar(30)")]
        [Required]
        public string Category { get; set; }

        [Column(TypeName = "float")]
        [Required]
        public double Cost { get; set; }

        [Column(TypeName = "varchar(30)")]
        [Required]
        public string Material { get; set; }
        
        [Required]
        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
    }
}
