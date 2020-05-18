using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace course.Models
{
    public class Client
    {
        [Column(TypeName = "varchar(30)")]
        [Required]
        public string BodyParameters { get; set; }

        [Key]
        [Column(TypeName = "nvarchar(450)")]
        [Required]
        public string UserGuid { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Required]
        public string FullName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Required]
        public string PhoneNumber { get; set; }
    }
}
