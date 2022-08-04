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
            List<Inventory> Inventory =            
                await _context.Inventories
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Color)
                .OrderBy(i => i.Id)
                .ToListAsync();
            List<InventoryIndexViewModel> InventoryPartial = new();
            foreach (Inventory inventory in Inventory)
            {
                if(InventoryPartial.Count == 0)
                {                    
                    InventoryPartial.Add(new(){
                        RawMaterial = inventory.RawMaterial,
                        ExistenciaTotal = inventory.Existencia
                    });
                }
                else
                {
                    if (InventoryPartial.Any(i => i.RawMaterial.Id == inventory.RawMaterial.Id))
                    {
                        foreach (InventoryIndexViewModel rawMaterial in InventoryPartial)
                        {
                            if (rawMaterial.RawMaterial.Id == inventory.RawMaterial.Id)
                            {
                                rawMaterial.ExistenciaTotal += inventory.Existencia;
                            }

                        }
                    }
                    else
                    {
                        InventoryPartial.Add(new()
                        {
                            RawMaterial = inventory.RawMaterial,
                            ExistenciaTotal = inventory.Existencia
                        });
                    }                    
                }
            }
            
            return View(InventoryPartial);
        }
        public async Task<IActionResult> Details(int id)
        {
            List<Inventory> Inventory =
                await _context.Inventories
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Color)
                .OrderBy(i => i.Id)
                .ToListAsync();
            List<Inventory> InventoryDetails = new();
            foreach (var inventory in Inventory)
            {
                if(inventory.RawMaterial.Id == id)
                {
                    InventoryDetails.Add(inventory);
                }
            }
            return View(InventoryDetails);
        }

        public async Task<IActionResult> AddInventory()
        {
            InventoryViewModel model = new()
            {
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0),
                Cantidad = 0
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
                    RawMaterial = await _context.RawMaterials
                    .Include(r => r.Color)
                    .Include(r => r.Fabric)
                    .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId),
                    Cantidad = model.Cantidad,
                    Existencia = model.Cantidad
                };
                _context.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0);
            return View(model);
        }

        public JsonResult GetRawMaterialColor(int colorId)
        {
            Color color = _context.Colors
                .Include(c => c.RawMaterials)
                .FirstOrDefault(c => c.Id == colorId);
            
            if (color == null)
            {
                return null;
            }

            return Json(color.RawMaterials.OrderBy(d => d.Nombre));
        }

        

    }
}
