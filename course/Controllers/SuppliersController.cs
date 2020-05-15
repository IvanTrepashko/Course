using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using course.Models;

namespace course.Controllers
{
    public class SuppliersController : Controller
    {
        public enum SortState
        {
            MaterialAsc,
            MaterialDesc,
            NameAsc,
            NameDesc,
            CostAsc,
            CostDesc,
            CountryAsc,
            CountryDesc,
            PhoneAsc,
            PhoneDesc,
        }

        private readonly ApplicationContext _context;

        public SuppliersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: Suppliers
        public async Task<IActionResult> Index(SortState sortState = SortState.CountryAsc)
        {
            IQueryable<Supplier> clothings = _context.Suppliers;

            ViewData["MaterialSort"] = sortState == SortState.MaterialAsc ? SortState.MaterialDesc : SortState.MaterialAsc;
            ViewData["NameSort"] = sortState == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            ViewData["CostSort"] = sortState == SortState.CostAsc ? SortState.CostDesc : SortState.CostAsc;
            ViewData["CountrySort"] = sortState == SortState.CountryAsc ? SortState.CountryDesc : SortState.CountryAsc;
            ViewData["PhoneSort"] = sortState == SortState.PhoneAsc ? SortState.PhoneDesc : SortState.PhoneAsc;

            clothings = sortState switch
            {
                SortState.MaterialDesc => clothings.OrderByDescending(x => x.Material),
                SortState.MaterialAsc => clothings.OrderBy(x => x.Material),
                SortState.CountryAsc => clothings.OrderBy(x => x.Country),
                SortState.CountryDesc => clothings.OrderByDescending(x => x.Country),
                SortState.NameDesc => clothings.OrderByDescending(x => x.Name),
                SortState.NameAsc => clothings.OrderBy(x => x.Name),
                SortState.CostAsc => clothings.OrderBy(x => x.Cost),
                SortState.CostDesc => clothings.OrderByDescending(x => x.Cost),
                SortState.PhoneDesc => clothings.OrderByDescending(x => x.PhoneNumber),
                SortState.PhoneAsc => clothings.OrderBy(x => x.PhoneNumber),
                _ => throw new NotImplementedException()
            };
            return View(await clothings.AsNoTracking().ToListAsync());
        }

        // GET: Suppliers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Suppliers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SupplierId,Country,Name,Cost,PhoneNumber,Material")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplierId,Country,Name,Cost,PhoneNumber,Material")] Supplier supplier)
        {
            if (id != supplier.SupplierId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.SupplierId))
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
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.SupplierId == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.SupplierId == id);
        }
    }
}
