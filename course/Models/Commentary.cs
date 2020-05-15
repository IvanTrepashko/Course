using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace course.Models
{
    public class Commentary
    {
        [Key]
        [Required]
        [Column(TypeName ="integer")]
        public int CommentId { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(250)")]
        public string Comment { get; set; }

    }
}
