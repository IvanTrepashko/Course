using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;
using System.Web;
using System.IO;
using course.ViewModels;

namespace course.Controllers
{
    public class ClothingsController : Controller
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

        public ClothingsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Clothings
        public async Task<IActionResult> Index(SortState sortState=SortState.CategoryAsc)
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

        // GET: Clothings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothes
                .FirstOrDefaultAsync(m => m.ClothingId == id);
            if (clothing == null)
            {
                return NotFound();
            }

            int count = _context.Orders.Where(x => x.ClothingId == id).Count();

            ViewBag.Count = count;
            ViewBag.Sum = count * clothing.Cost;

            return View(clothing);
        }

        // GET: Clothings/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClothingViewModel clothing)
        {
            Clothing clothing1 = new Clothing { Category = clothing.Category, Cost = clothing.Cost, Material = clothing.Material };

            if (ModelState.IsValid)
            {
                byte[] image = null;
                using (var binaryReader = new BinaryReader(clothing.Image.OpenReadStream()))
                {
                    image = binaryReader.ReadBytes((int)clothing.Image.Length);
                }
                clothing1.Image = image;

                _context.Clothes.Add(clothing1);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }
            return View(clothing);
        }

        // GET: Clothings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothes.FindAsync(id);
            if (clothing == null)
            {
                return NotFound();
            }

            var clothingViewModel = new ClothingViewModel();
            clothingViewModel.Category = clothing.Category;
            clothingViewModel.Cost = clothing.Cost;
            clothingViewModel.Material = clothing.Material;
            clothingViewModel.ClothingId = clothing.ClothingId;

            return View(clothingViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("ClothingId,Category,Cost,Material,Image")] ClothingViewModel clothing)
        {
            if (id != clothing.ClothingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    byte[] image = null;
                    using (var binaryReader = new BinaryReader(clothing.Image.OpenReadStream()))
                    {
                        image = binaryReader.ReadBytes((int)clothing.Image.Length);
                    }

                    var updClothing = new Clothing() { Category = clothing.Category, Cost = clothing.Cost, Image = image, Material = clothing.Material, ClothingId = clothing.ClothingId };
                    
                    _context.Update(updClothing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClothingExists(clothing.ClothingId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("index");
            }
            return View(clothing);
        }

        // GET: Clothings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clothing = await _context.Clothes
                .FirstOrDefaultAsync(m => m.ClothingId == id);
            if (clothing == null)
            {
                return NotFound();
            }

            return View(clothing);
        }

        // POST: Clothings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clothing = await _context.Clothes.FindAsync(id);
            _context.Clothes.Remove(clothing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClothingExists(int id)
        {
            return _context.Clothes.Any(e => e.ClothingId == id);
        }
    }
}
