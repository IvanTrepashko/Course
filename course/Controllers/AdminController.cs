using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using course.Models;
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

        [HttpGet]
        public IActionResult Method()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Method(Method method )
        {
            StaticMethod.MonthCount = method.MonthCount;
            StaticMethod.OptionsCount = method.OptionsCount;
            StaticMethod.Efficiency = method.Efficiency;

            return RedirectToAction("Method1");
        }

        [HttpGet]
        public IActionResult Method1()
        {
            Method method = new Method();

            method.Efficiency = StaticMethod.Efficiency;
            method.MonthCount = StaticMethod.MonthCount;
            method.OptionsCount = StaticMethod.OptionsCount;

            return View(method);
        }

        [HttpPost]
        public IActionResult Method1(Method method)
        {
            StaticMethod.Data = method.Data;

            StaticMethod.Compute();

            return RedirectToAction("Method2");
        }
        [HttpGet]
        public IActionResult Method2()
        {
            Method method = new Method();
            method.Result = StaticMethod.Result;

            return View(method);
        }
    }
}