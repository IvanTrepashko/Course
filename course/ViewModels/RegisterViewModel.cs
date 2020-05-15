using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace course.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="email введен неверно")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Параметры тела (рост - грудь - талия)")]
        public string BodyParameters { get; set; }

        [Required]
        [Display(Name = "Ваше имя")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Ваш номер телефона (xxx-xx-xx)")]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}
