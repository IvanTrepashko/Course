using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace course.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Country { get; set; }

        [Required]
        [Column(TypeName = "varchar(40)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "float")]
        public double Cost { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "varchar(30)")]
        public string Material { get; set; }
    }
}
