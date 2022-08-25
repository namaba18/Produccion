using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Models;

namespace Produccion.Controllers
{
    public class GarmentsController : Controller
    {
        private readonly DataContext _context;
        public GarmentsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Garments.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Garment garment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(garment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(garment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Garments == null)
            {
                return NotFound();
            }

            Garment garment = await _context.Garments.FindAsync(id);
            if (garment == null)
            {
                return NotFound();
            }

            return View(garment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Garment garment)
        {
            if (id != garment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                                                       
                    _context.Update(garment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GarmentExists(garment.Id))
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
            return View(garment);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Garments == null)
            {
                return NotFound();
            }

            Garment garment = await _context.Garments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (garment == null)
            {
                return NotFound();
            }

            return View(garment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Garments == null)
            {
                return Problem("Entity set 'DataContext.Fabrics'  is null.");
            }
            Garment garment = await _context.Garments.FindAsync(id);
            if (garment != null)
            {
                _context.Garments.Remove(garment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool GarmentExists(int id)
        {
            return (_context.Garments?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
    
}
