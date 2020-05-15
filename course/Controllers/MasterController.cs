using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using course.Models;
using course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace course.Controllers
{
    public class MasterController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _context;

        public MasterController(ApplicationContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {

            await _signInManager.SignOutAsync();
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var role = await _userManager.GetRolesAsync(user);

                    if (!role[0].Equals("employee"))
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                    else if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("menu","master");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        public IActionResult Menu()
        {
            return View();
        }

        public async Task<IActionResult> AddOrder()
        {
            var emptyOrders = new List<OrderViewModel>();

            var query = from order in _context.Orders
                        join clothing in _context.Clothes on order.ClothingId equals clothing.ClothingId
                        select new
                        {
                            ClientGuid= order.ClientGuid,
                            OrderId=order.OrderId,
                            Category = clothing.Category,
                            Material = clothing.Material,
                            Cost = clothing.Cost,
                            OrderingTime = order.OrderingTime,
                            MasterGuid = order.EmployeeGuid
                        };
            var query1 = from item in query
                         join client in _context.Clients on item.ClientGuid equals client.UserGuid into qrt
                         from sub in qrt.DefaultIfEmpty()
                         select new { Order = item, Client = sub.FullName, PhoneNumber=sub.PhoneNumber };

            foreach (var item in query1)
            {
                var tmp = new OrderViewModel();

                if (item.Order.MasterGuid == null)
                {

                    tmp.Category = item.Order.Category;
                    tmp.Material = item.Order.Material;
                    tmp.Cost = item.Order.Cost;
                    tmp.OrderingTime = item.Order.OrderingTime;
                    tmp.ClientName = item.Client;
                    tmp.OrderId = item.Order.OrderId;
                    tmp.PhoneNumber = item.PhoneNumber;

                    emptyOrders.Add(tmp);
                }
            }
            return View(emptyOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Add(int? id)
        {
            var order = _context.Orders.First(x => x.OrderId == id);

            var master = _context.Masters.First(x => x.UserGuid == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var clothes = _context.Clothes.First(x => x.ClothingId == order.ClothingId);
            
            master.TotalOrders++;
            master.TotalCosts += clothes.Cost;

            order.EmployeeGuid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Random rnd = new Random();
            
            _context.Orders.Update(order);
            _context.Masters.Update(master);

            await _context.SaveChangesAsync();

            return RedirectToAction("addorder");
        }

        [HttpGet]
        public IActionResult Information()
        {
            var currentMasterOrders = new List<OrderViewModel>();

            var query = from order in _context.Orders
                        join clothing in _context.Clothes on order.ClothingId equals clothing.ClothingId
                        select new
                        {
                            ClientGuid = order.ClientGuid,
                            OrderId = order.OrderId,
                            Category = clothing.Category,
                            Material = clothing.Material,
                            Cost = clothing.Cost,
                            OrderingTime = order.OrderingTime,
                            MasterGuid = order.EmployeeGuid,
                            isCompleted = order.isCompleted
                        };
            var query1 = from item in query
                         join client in _context.Clients on item.ClientGuid equals client.UserGuid into qrt
                         from sub in qrt.DefaultIfEmpty()
                         select new { Order = item, Client = sub.FullName,PhoneNumber=sub.PhoneNumber };

            foreach (var item in query1)
            {
                var tmp = new OrderViewModel();

                if (item.Order.MasterGuid == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {

                    tmp.Category = item.Order.Category;
                    tmp.Material = item.Order.Material;
                    tmp.Cost = item.Order.Cost;
                    tmp.OrderingTime = item.Order.OrderingTime;
                    tmp.ClientName = item.Client;
                    tmp.OrderId = item.Order.OrderId;
                    tmp.isCompleted = item.Order.isCompleted;
                    tmp.PhoneNumber = item.PhoneNumber;

                    currentMasterOrders.Add(tmp);
                }
            }
            return View(currentMasterOrders);
        }

        [HttpGet]
        public async Task<IActionResult> AddIndividual()
        {
            var emptyOrders = await _context.IndividualOrders.Where(x => x.EmployeeGuid == null).ToListAsync();
            return View(emptyOrders);
        }

        [HttpGet]
        public IActionResult AddInd (int id)
        {
            var indOrder = _context.IndividualOrders.First(x => x.OrderId == id);

            return View(indOrder);
        }

        [HttpPost]
        public IActionResult AddInd(IndividualOrder order)
        {
            var master = _context.Masters.First(x => x.UserGuid == this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var indOrder = _context.IndividualOrders.First(x => x.OrderDescription.Equals(order.OrderDescription));


            var rnd = new Random();
            indOrder.MasterName = master.FullName;
            indOrder.EmployeeGuid = master.UserGuid;

            indOrder.Cost = order.Cost;
            _context.IndividualOrders.Update(indOrder);
            _context.SaveChanges();

            return RedirectToAction("menu");
        }

        [HttpGet]
        public async Task<IActionResult> IndividualInfo()
        {
            var currentMasterIndOrders = await _context.IndividualOrders.Where(x => x.EmployeeGuid == this.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();

            return View(currentMasterIndOrders);
        }

        [HttpGet]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var order = _context.Orders.First(x => x.OrderId == id);
            order.isCompleted = 1;
            order.SewingDate = DateTimeOffset.Now;
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("information");
        }

        [HttpGet]
        public async Task<IActionResult> MarkCompletedInd(int id)
        {
            var order = _context.IndividualOrders.First(x => x.OrderId == id);
            order.isCompleted = 1;
            order.SewingDate = DateTimeOffset.Now;

            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("individualinfo");
        }
    }
}