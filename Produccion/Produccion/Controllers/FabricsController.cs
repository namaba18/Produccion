using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;

namespace Produccion.Controllers
{
    public class FabricsController : Controller
    {
        private readonly DataContext _context;
        public FabricsController(DataContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            return _context.Fabrics != null ?
                        View(await _context.Fabrics.ToListAsync()) :
                        Problem("Entity set 'DataContext.Colors'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fabric fabric)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fabric);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fabric);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Fabrics == null)
            {
                return NotFound();
            }

            Fabric fabric = await _context.Fabrics.FindAsync(id);
            if (fabric == null)
            {
                return NotFound();
            }
            return View(fabric);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fabric fabric)
        {
            if (id != fabric.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fabric);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FabricExists(fabric.Id))
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
            return View(fabric);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Fabrics == null)
            {
                return NotFound();
            }

            var color = await _context.Fabrics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (color == null)
            {
                return NotFound();
            }

            return View(color);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Fabrics == null)
            {
                return Problem("Entity set 'DataContext.Fabrics'  is null.");
            }
            Fabric fabric = await _context.Fabrics.FindAsync(id);
            if (fabric != null)
            {
                _context.Fabrics.Remove(fabric);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FabricExists(int id)
        {
            return (_context.Fabrics?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
