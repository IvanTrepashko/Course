using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using course.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace course.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly SignInManager<User> _signInManager;

        public HomeController(ApplicationContext context, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            await _signInManager.SignOutAsync();

            var commentaries = await _context.Commentaries.ToListAsync();
            Commentary comment=new Commentary();

            Random rnd = new Random();
            if (commentaries.Count != 0)
            {
                comment = commentaries[rnd.Next(0, commentaries.Count)];
            }
            return View(comment);
        }
    }
}