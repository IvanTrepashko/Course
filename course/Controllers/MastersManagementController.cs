using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;
using course.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace course.Controllers
{
    public class MastersManagementController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public MastersManagementController(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: MastersManagement
        public async Task<IActionResult> Index()
        {
            return View(await _context.Masters.ToListAsync());
        }

        // GET: MastersManagement/Details/5
        public IActionResult Details(string id)
        {
            var currentMasterOrders = new List<OrderViewModel>();

            var master = _context.Masters.First(x => x.UserGuid.Equals(id));

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
                            MasterGuid = order.EmployeeGuid
                        };
            var query1 = from item in query
                         join client in _context.Clients on item.ClientGuid equals client.UserGuid into qrt
                         from sub in qrt.DefaultIfEmpty()
                         select new { Order = item, Client = sub.FullName };

            foreach (var item in query1)
            {
                var tmp = new OrderViewModel();

                if (item.Order.MasterGuid == id)
                {

                    tmp.Category = item.Order.Category;
                    tmp.Material = item.Order.Material;
                    tmp.Cost = item.Order.Cost;
                    tmp.OrderingTime = item.Order.OrderingTime;
                    tmp.ClientName = item.Client;
                    tmp.OrderId = item.Order.OrderId;
                    tmp.MasterName = master.FullName;
                    currentMasterOrders.Add(tmp);
                }
            }
            ViewBag.MasterName = master.FullName;
            return View(currentMasterOrders);

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MasterViewModel master)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = master.Email, UserName = master.Email };

                var result = await _userManager.CreateAsync(user, master.Password);
                await _userManager.AddToRoleAsync(user, "employee");

                _context.Masters.Add(new Master { FullName = master.FullName, Qualification = master.Qualification, TotalCosts = 0, TotalOrders = 0, UserGuid = user.Id });
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var master = await _context.Masters.FindAsync(id);
            if (master == null)
            {
                return NotFound();
            }
            master.TotalCosts += 100;
            return View(master);
        }

        // POST: MastersManagement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Master master)
        {
            var mstr = _context.Masters.Where(x => x.UserGuid.Equals(id)).ToList();
            mstr[0].FullName = master.FullName;
            mstr[0].Qualification = master.Qualification;
 
            try
            {
                _context.Update(mstr[0]);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MasterExists(master.UserGuid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var master = await _context.Masters
                .FirstOrDefaultAsync(m => m.UserGuid == id);
            if (master == null)
            {
                return NotFound();
            }

            return View(master);
        }

        // POST: MastersManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var master = await _context.Masters.FindAsync(id);
            _context.Masters.Remove(master);

            var orders = await _context.Orders.Where(x => x.EmployeeGuid.Equals(id)).ToListAsync();

            foreach (var item in orders)
            {
                item.EmployeeGuid = null;
                item.SewingDate = DateTimeOffset.MinValue;
            }

            await _context.SaveChangesAsync();
 
            var user = await _userManager.FindByIdAsync(master.UserGuid);
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool MasterExists(string id)
        {
            return _context.Masters.Any(e => e.UserGuid == id);
        }
    }
}
