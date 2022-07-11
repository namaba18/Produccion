using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;

namespace Produccion.Controllers
{
    public class ProductionOrdersController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public ProductionOrdersController(DataContext context, ICombosHelper combosHelper)
        {
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductionOrders
                .Include(p => p.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Include(p => p.RawMaterial)
                .ThenInclude(r => r.Color)
                .Include(p => p.Garment)
                .ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ProductionOrderViewModel model = new()
            {
                Unidades = 0,
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0),
                Garments = await _combosHelper.GetComboGarmentsAsync(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductionOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                ProductionOrder productionOrder = new()
                {
                    Unidades = model.Unidades,
                    RawMaterial = await _context.RawMaterials
                    .Include(r => r.Color)
                    .Include(r => r.Fabric)
                    .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId),
                    Garment = await _context.Garments.FindAsync(model.GarmentId)
                };
                Inventory inventory = await _context.Inventories.FirstOrDefaultAsync(i => i.RawMaterial.Id == productionOrder.RawMaterial.Id);
                if (productionOrder.Unidades*productionOrder.Garment.ConsumoInvUnd > inventory.Cantidad)
                {
                    ModelState.AddModelError(string.Empty, "No hay inventario suficiente.");
                    model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0);
                    model.Garments = await _combosHelper.GetComboGarmentsAsync();
                    return View(model);
                }
                inventory.Cantidad = inventory.Cantidad - (productionOrder.Unidades * productionOrder.Garment.ConsumoInvUnd); 
                _context.Add(productionOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.RawMaterials= await _combosHelper.GetComboRawMaterialsAsync(0);
            model.Garments = await _combosHelper.GetComboGarmentsAsync();
            return View(model);
        }

    }
}
