using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using course.Models;
using course.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace course.Controllers
{

    public class ClientsController : Controller
    {
        public enum SortState
        {
            MaterialAsc,
            MaterialDesc,
            CategoryAsc,
            CategoryDesc,
            CostAsc,
            CostDesc
        }

        private readonly ApplicationContext _context;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public ClientsController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            await _signInManager.SignOutAsync();
            return View();
        }

        public IActionResult Menu()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, PhoneNumber=model.PhoneNumber };

                var result = await _userManager.CreateAsync(user, model.Password);

                await _userManager.AddToRoleAsync(user, "client");

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);

                    _context.Clients.Add(new Client { UserGuid = user.Id, BodyParameters=model.BodyParameters, FullName=model.FullName, PhoneNumber=model.PhoneNumber});

                    await _context.SaveChangesAsync();

                    return RedirectToAction("menu");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, "Такой email уже существует");
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
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
                    
                    if(!role[0].Equals("client"))
                    {
                        ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                    }
                    else if 
                    (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                        {
                        return RedirectToAction("menu");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильный логин и (или) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Create(SortState sortState = SortState.MaterialAsc)
        {
            IQueryable<Clothing> clothings = _context.Clothes;

            ViewData["MaterialSort"] = sortState == SortState.MaterialAsc ? SortState.MaterialDesc : SortState.MaterialAsc;
            ViewData["CategorySort"] = sortState == SortState.CategoryAsc ? SortState.CategoryDesc : SortState.CategoryAsc;
            ViewData["CostSort"] = sortState == SortState.CostAsc ? SortState.CostDesc : SortState.CostAsc;

            clothings = sortState switch
            {
                SortState.MaterialDesc => clothings.OrderByDescending(x => x.Material),
                SortState.MaterialAsc => clothings.OrderBy(x => x.Material),
                SortState.CategoryAsc => clothings.OrderBy(x => x.Category),
                SortState.CategoryDesc => clothings.OrderByDescending(x => x.Category),
                SortState.CostAsc => clothings.OrderBy(x => x.Cost),
                SortState.CostDesc => clothings.OrderByDescending(x => x.Cost),
                _ => throw new NotImplementedException()
            };
            return View(await clothings.AsNoTracking().ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Add(int? id)
        {
            List<Clothing> clothings = await _context.Clothes.Where(x => x.ClothingId == id).ToListAsync();
            var clothing = clothings[0];
            
            _context.Orders.Add(new Order { ClientGuid = User.FindFirst(ClaimTypes.NameIdentifier).Value, ClothingId = clothing.ClothingId, OrderingTime = DateTimeOffset.Now, EmployeeGuid=null,SewingDate=DateTimeOffset.MinValue,isCompleted=0 });
            await _context.SaveChangesAsync();
            return RedirectToAction("create");
        }

        [HttpGet]
        public IActionResult Information()
        {
            double money=0;

            var currentUserOrders = new List<OrderViewModel>();
            var query = from order in _context.Orders
                        join clothing in _context.Clothes on order.ClothingId equals clothing.ClothingId
                        select new
                        {
                            ClientGuid = order.ClientGuid,
                            Category = clothing.Category,
                            Material = clothing.Material,
                            Cost = clothing.Cost,
                            OrderingTime = order.OrderingTime,
                            SewingTime = order.SewingDate,
                            MasterGuid=order.EmployeeGuid,
                            Image = clothing.Image,
                            isCompleted=order.isCompleted
                        };
            var query1 = from item in query
                         join master in _context.Masters on item.MasterGuid equals master.UserGuid into qrt
                         from sub in qrt.DefaultIfEmpty()
                         select new { Order = item, Master = sub.FullName };
            
            foreach (var item in query1)
            {
                var tmp = new OrderViewModel();

                if (item.Order.ClientGuid == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {

                    tmp.Category = item.Order.Category;
                    tmp.Material = item.Order.Material;
                    tmp.Cost = item.Order.Cost;
                    tmp.OrderingTime = item.Order.OrderingTime;
                    tmp.SewingTime = item.Order.SewingTime;
                    tmp.MasterName = item.Master;
                    tmp.Image = item.Order.Image;
                    tmp.isCompleted = item.Order.isCompleted;

                    money += tmp.Cost;
                    currentUserOrders.Add(tmp);
                }
            }

            return View(currentUserOrders);
        }

        [HttpGet]
        public IActionResult Comment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Comment(string comment)
        {
            _context.Commentaries.Add(new Commentary { Comment = comment });
            await _context.SaveChangesAsync();
            return View("menu");
        }

        [HttpGet]
        public IActionResult Individual()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Individual(string order)
        {
            var client = _context.Clients.First(x => x.UserGuid == this.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var indOrder = new IndividualOrder() { ClientGuid = this.User.FindFirst(ClaimTypes.NameIdentifier).Value, EmployeeGuid = null, OrderDescription = order, OrderingTime = DateTimeOffset.Now,ClientName=client.FullName, isCompleted=0, PhoneNumber = client.PhoneNumber};
            _context.IndividualOrders.Add(indOrder);
            await _context.SaveChangesAsync();

            return View("menu");
        }

        [HttpGet]
        public async Task<IActionResult> IndividualInfo()
        {
            var currentClientIndOrders = await _context.IndividualOrders.Where(x => x.ClientGuid == this.User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            return View(currentClientIndOrders); 
        }
    }
}