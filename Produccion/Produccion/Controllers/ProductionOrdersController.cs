using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;
using Vereyon.Web;

namespace Produccion.Controllers
{
    public class ProductionOrdersController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IFlashMessage _flashMessage;

        public ProductionOrdersController(DataContext context, ICombosHelper combosHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.ProductionOrders
                .Include(p => p.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Include(p => p.RawMaterial)
                .ThenInclude(r => r.Color)
                .Include(p => p.Garment)
                .OrderBy(p => p.Id)
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
                    .Include(r => r.Inventories)
                    .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId),
                    Garment = await _context.Garments.FindAsync(model.GarmentId)
                };
                

                List<Inventory> Inventory = await _context.Inventories
                    .Include(i=>i.RawMaterial)
                    .ToListAsync();
                List<InventoryIndexViewModel> InventoryPartial = new();
                foreach (Inventory inventory in Inventory)
                {
                    if (InventoryPartial.Count == 0)
                    {
                        InventoryPartial.Add(new()
                        {
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
                float cant = productionOrder.Unidades * productionOrder.Garment.ConsumoInvUnd;
                foreach (var item in InventoryPartial)
                {
                    if(item.RawMaterial.Id == productionOrder.RawMaterial.Id)
                    {
                        if(item.ExistenciaTotal >= cant)
                        {
                            bool num = false;
                            foreach(Inventory item2 in Inventory)
                            {
                                if (item2.RawMaterial.Id == productionOrder.RawMaterial.Id && num == false)
                                {
                                    if(item2.Existencia >= cant)
                                    {
                                        item2.Existencia -= cant;
                                        num = true;
                                    }
                                    else
                                    {
                                        cant -= item2.Existencia;
                                        item2.Existencia = 0;
                                    }
                                    _context.Add(productionOrder);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {                            
                            model.Colors = await _combosHelper.GetComboColorsAsync();
                            model.Fabrics = await _combosHelper.GetComboFabricsAsync();
                            model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0);
                            model.Garments = await _combosHelper.GetComboGarmentsAsync();
                            _flashMessage.Danger("No hay inventario suficiente.");
                            return View(model);
                        }
                    }
                    
                }               
                
                return RedirectToAction(nameof(Index));
            }
            model.Colors = await _combosHelper.GetComboColorsAsync();
            model.Fabrics = await _combosHelper.GetComboFabricsAsync();
            model.RawMaterials= await _combosHelper.GetComboRawMaterialsAsync(0);
            model.Garments = await _combosHelper.GetComboGarmentsAsync();
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
