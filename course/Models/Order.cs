using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace course.Models
{
    public class Order
    {

        [Key]
        [Column(TypeName = "integer")]
        [Required]
        public int OrderId { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [Required]
        public string ClientGuid { get; set; }

        [AllowNull]
        [Column(TypeName = "nvarchar(450)")]
        public string EmployeeGuid { get; set; }

        [Column(TypeName = "integer")]
        [Required]
        public int ClothingId { get; set; }

        [Required]
        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset OrderingTime { get; set; }

        [AllowNull]
        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset SewingDate { get; set; }

        [Required]
        [Column(TypeName = "integer")]
        public int isCompleted { get; set; } = 0;
    }
}
