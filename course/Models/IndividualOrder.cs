using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace course.Models
{
    public class IndividualOrder
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

        [AllowNull]
        [Column(TypeName = "nvarchar(450)")]
        public string MasterName { get; set; }

        [AllowNull]
        [Column(TypeName = "nvarchar(450)")]
        public string PhoneNumber { get; set; }

        [Column(TypeName ="nvarchar(450)")]
        public string ClientName { get; set; }
        [Column(TypeName = "varchar(150)")]
        [Required]
        public string OrderDescription { get; set; }

        [Column(TypeName = "float")]
        public double Cost { get;set; }

        [Required]
        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset OrderingTime { get; set; }

        [AllowNull]
        [Column(TypeName = "datetimeoffset")]
        public DateTimeOffset SewingDate { get; set; }

        [Column(TypeName ="integer")]
        public int isCompleted { get; set; }
    }
}
