using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;

namespace Produccion.Controllers
{
    public class RawMaterialsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public RawMaterialsController(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.RawMaterials
                .Include(r => r.Color)
                .Include(r => r.Fabric)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            RawMaterialViewModel model = new()
            {
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RawMaterialViewModel model)
        {
            if (ModelState.IsValid)
            {
                RawMaterial rawMaterial = new()
                {
                    Nombre = model.Nombre,
                    Color = await _context.Colors.FindAsync(model.ColorId),
                    Fabric = await _context.Fabrics.FindAsync(model.FabricId),
                };
                _context.Add(rawMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.Colors = await _combosHelper.GetComboColorsAsync();
            model.Fabrics = await _combosHelper.GetComboFabricsAsync();
            return View(model);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RawMaterials == null)
            {
                return NotFound();
            }
            RawMaterial rawMaterial = await _context.RawMaterials
                .Include(r => r.Color)
                .Include(r => r.Fabric)
                .FirstOrDefaultAsync(r => r.Id == id);

            RawMaterialViewModel model = new()
            {
                Id = rawMaterial.Id,
                Nombre = rawMaterial.Nombre,
                Colors = await _combosHelper.GetComboColorsAsync(),
                ColorId = rawMaterial.Color.Id,
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
                FabricId = rawMaterial.Fabric.Id
            };
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RawMaterialViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    RawMaterial rawMaterial = new()
                    {
                        Id = model.Id,
                        Nombre = model.Nombre,
                        Color = await _context.Colors.FindAsync(model.ColorId),
                        Fabric = await _context.Fabrics.FindAsync(model.FabricId),
                    };
                    _context.Update(rawMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RawMaterialExists(model.Id))
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
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
