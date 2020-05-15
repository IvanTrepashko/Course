using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;
using course.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.Security.Policy;

namespace course.Controllers
{
    public class OrdersManagementController : Controller
    {
        public enum SortState
        {
            ClientAsc,
            ClientDesc,
            MaterialAsc,
            MaterialDesc,
            CategoryAsc,
            CategoryDesc,
            CostAsc,
            CostDesc,
            OrderAsc,
            OrderDesc,
            SewingDesc,
            SewingAsc,
            MasterAsc,
            MasterDesc
        }

        private readonly ApplicationContext _context;

        public OrdersManagementController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Orders
        public IActionResult Index(SortState sortState=SortState.ClientAsc)
        {
            var orders = new List<OrderViewModel>();
            var query = from order in _context.Orders
                        join clothing in _context.Clothes on order.ClothingId equals clothing.ClothingId
                        join client in _context.Clients on order.ClientGuid equals client.UserGuid
                        select new
                        {
                            OrderId = order.OrderId,
                            ClientName=client.FullName,
                            Category = clothing.Category,
                            Material = clothing.Material,
                            Cost = clothing.Cost,
                            OrderingTime = order.OrderingTime,
                            SewingTime = order.SewingDate,
                            MasterId = order.EmployeeGuid,
                            isCompleted=order.isCompleted
                        };
            var query2 = from order in query
                         join master in _context.Masters on order.MasterId equals master.UserGuid into tt
                         from sub in tt.DefaultIfEmpty()
                         select new { Order = order, Master = sub.FullName };
            foreach (var item in query2)
            {
                var tmp = new OrderViewModel();
                tmp.Category = item.Order.Category;
                tmp.Material = item.Order.Material;
                tmp.Cost = item.Order.Cost;
                tmp.OrderingTime = item.Order.OrderingTime;
                tmp.SewingTime = item.Order.SewingTime;
                tmp.ClientName = item.Order.ClientName;
                tmp.OrderId = item.Order.OrderId;
                tmp.MasterName = item.Master;
                tmp.isCompleted = item.Order.isCompleted;

                orders.Add(tmp);
            }


            ViewData["MaterialSort"] = sortState == SortState.MaterialAsc ? SortState.MaterialDesc : SortState.MaterialAsc;
            ViewData["CategorySort"] = sortState == SortState.CategoryAsc ? SortState.CategoryDesc : SortState.CategoryAsc;
            ViewData["CostSort"] = sortState == SortState.CostAsc ? SortState.CostDesc : SortState.CostAsc;
            ViewData["ClientSort"] = sortState == SortState.ClientAsc ? SortState.ClientDesc : SortState.ClientAsc;
            ViewData["OrderSort"] = sortState == SortState.OrderAsc ? SortState.OrderDesc : SortState.OrderAsc;
            ViewData["SewingSort"] = sortState == SortState.SewingAsc ? SortState.SewingDesc : SortState.SewingAsc;
            ViewData["MasterSort"] = sortState == SortState.MasterAsc ? SortState.MasterDesc : SortState.MasterAsc;

            orders = sortState switch
            {
                SortState.MaterialDesc => orders.OrderByDescending(x => x.Material).ToList(),
                SortState.MaterialAsc => orders.OrderBy(x => x.Material).ToList(),
                SortState.CategoryAsc => orders.OrderBy(x => x.Category).ToList(),
                SortState.CategoryDesc => orders.OrderByDescending(x => x.Category).ToList(),
                SortState.CostAsc => orders.OrderBy(x => x.Cost).ToList(),
                SortState.CostDesc => orders.OrderByDescending(x => x.Cost).ToList(),
                SortState.OrderDesc => orders.OrderByDescending(x => x.OrderingTime).ToList(),
                SortState.OrderAsc => orders.OrderBy(x => x.OrderingTime).ToList(),
                SortState.SewingAsc => orders.OrderBy(x => x.SewingTime).ToList(),
                SortState.SewingDesc => orders.OrderByDescending(x => x.SewingTime).ToList(),
                SortState.ClientAsc => orders.OrderBy(x => x.ClientName).ToList(),
                SortState.ClientDesc => orders.OrderByDescending(x => x.ClientName).ToList(),
                SortState.MasterDesc => orders.OrderByDescending(x => x.MasterName).ToList(),
                SortState.MasterAsc => orders.OrderBy(x => x.MasterName).ToList(),
                _ => throw new NotImplementedException()
            };

            return View(orders);
        }
      
        // GET: Orders/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var orders = new List<OrderViewModel>();

            var query = from order in _context.Orders
                        join clothing in _context.Clothes on order.ClothingId equals clothing.ClothingId
                        join client in _context.Clients on order.ClientGuid equals client.UserGuid
                        select new
                        {
                            OrderId = order.OrderId,
                            ClientName = client.FullName,
                            Category = clothing.Category,
                            Material = clothing.Material,
                            Cost = clothing.Cost,
                            OrderingTime = order.OrderingTime,
                            SewingTime = order.SewingDate,
                            MasterId = order.EmployeeGuid
                        };

            var query2 = from order in query
                         join master in _context.Masters on order.MasterId equals master.UserGuid into tt
                         from sub in tt.DefaultIfEmpty()
                         select new { Order = order, Master = sub.FullName };

            var order1 = query2.First(x => x.Order.OrderId == id);

            var tmp = new OrderViewModel();
            tmp.Category = order1.Order.Category;
            tmp.Material = order1.Order.Material;
            tmp.Cost = order1.Order.Cost;
            tmp.OrderingTime = order1.Order.OrderingTime;
            tmp.SewingTime = order1.Order.SewingTime;
            tmp.ClientName = order1.Order.ClientName;
            tmp.OrderId = order1.Order.OrderId;
            tmp.MasterName = order1.Master;

            if (order1 == null)
            {
                return NotFound();
            }

            return View(tmp);
        }

        public async Task<IActionResult> Individual()
        {
            var individualOrders = await _context.IndividualOrders.ToListAsync();
            return View(individualOrders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult DeleteInd(int? id)
        {
            var order = _context.IndividualOrders.First(x => x.OrderId == id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteInd(int id)
        {
            var order = await _context.IndividualOrders.FindAsync(id);
            _context.IndividualOrders.Remove(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("individual");
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
