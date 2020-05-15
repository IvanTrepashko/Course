using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace course.ViewModels
{
    public class ClothingViewModel
    {
        [Required]
        public int ClothingId { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public double Cost { get; set; }

        [Required]
        public string Material { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
