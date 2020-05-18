using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using course.Models;
using Microsoft.AspNetCore.Identity;

namespace course.Controllers
{
    public class ClientsManagementController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<User> _userManager;

        public ClientsManagementController(ApplicationContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Clients.ToListAsync());
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.UserGuid == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);

            var orders = _context.Orders.Where(x => x.ClientGuid == client.UserGuid);

            foreach (var item in orders)
            {
                _context.Orders.Remove(item);
            }

            var indOrders = _context.IndividualOrders.Where(x => x.ClientGuid == client.UserGuid);

            foreach (var item in indOrders)
            {
                _context.IndividualOrders.Remove(item);
            }

            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return _context.Clients.Any(e => e.UserGuid == id);
        }
    }
}
