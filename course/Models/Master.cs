using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace course.Models
{
    public class Master
    {
        [Key]
        [Required]
        [Column(TypeName ="nvarchar(450)")]
        public string UserGuid { get; set; }

        [Column(TypeName = "varchar(30)")]
        [Required]
        public string Qualification { get; set; }

        [Column(TypeName = "integer")]
        [Required]
        public int TotalOrders { get; set; }

        [Column(TypeName = "float")]
        [Required]
        public double TotalCosts { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Required]
        public string FullName { get; set; }
    }
}
