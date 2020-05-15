using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace course.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email,string password)
        {
            if (email.Equals("admin@gmail.com") && password.Equals("admin"))
            {
                return RedirectToAction("menu");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Menu()
        {
            return View();
        }
    }
}