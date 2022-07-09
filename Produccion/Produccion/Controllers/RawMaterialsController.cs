using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;

namespace Produccion.Controllers
{
    public class RawMaterialsController : Controller
    {
        private readonly DataContext _context;

        public RawMaterialsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.RawMaterials != null ?
                        View(await _context.RawMaterials.ToListAsync()) :
                        Problem("Entity set 'DataContext.Colors'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterial rawMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rawMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rawMaterial);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RawMaterials == null)
            {
                return NotFound();
            }

            RawMaterial rawMaterial = await _context.RawMaterials
                .Include(r => r.Colors)
                .Include(r => r.Fabrics)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (rawMaterial == null)
            {
                return NotFound();
            }
            return View(rawMaterial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RawMaterial rawMaterial)
        {
            if (id != rawMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rawMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RawMaterialExists(rawMaterial.Id))
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
            return View(rawMaterial);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RawMaterials == null)
            {
                return NotFound();
            }

            RawMaterial rawMaterial = await _context.RawMaterials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rawMaterial == null)
            {
                return NotFound();
            }

            return View(rawMaterial);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RawMaterials == null)
            {
                return Problem("Entity set 'DataContext.Colors'  is null.");
            }
            RawMaterial rawMaterial = await _context.RawMaterials.FindAsync(id);
            if (rawMaterial != null)
            {
                _context.RawMaterials.Remove(rawMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RawMaterialExists(int id)
        {
            return (_context.RawMaterials?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
