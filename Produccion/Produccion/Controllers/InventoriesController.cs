using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;

namespace Produccion.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public InventoriesController(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Inventories
                .Include(i => i.RawMaterial)
                .ToListAsync());
        }

        public async Task<IActionResult> AddInventory()
        {
            InventoryViewModel model = new()
            {
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(),
                Cantidad = 0,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddInventory(InventoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                Inventory inventory = new()
                {
                    RawMaterial = await _context.RawMaterials.FindAsync(model.RawMaterialId),
                    Cantidad = model.Cantidad,
                };
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync();
            return View(model);
        }
    }
}
