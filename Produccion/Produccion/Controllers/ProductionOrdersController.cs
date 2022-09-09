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
        public async Task<IActionResult> AddOrEdit(int id)
        {
            ProductionOrderViewModel model = new()
            {
                Unidades = 0,
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0),
                Garments = await _combosHelper.GetComboGarmentsAsync(),
            };            
            if (id == 0)
            {
                return View(model);
            }
            else
            {
                ProductionOrder order = await _context.ProductionOrders
                .Include(r => r.RawMaterial)
                .ThenInclude(i => i.Color)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Inventories)
                .Include(o => o.Garment)
                .FirstOrDefaultAsync(r => r.Id == id);

                model = new()
                {
                    Id = order.Id,
                    Colors = await _combosHelper.GetComboColorsAsync(),
                    ColorId = order.RawMaterial.Color.Id,
                    Fabrics = await _combosHelper.GetComboFabricsAsync(),
                    FabricId = order.RawMaterial.Fabric.Id,
                    RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(),
                    RawMaterialId = order.RawMaterial.Id,
                    Garments = await _combosHelper.GetComboGarmentsAsync(),
                    GarmentId = order.Garment.Id,
                    Unidades = order.Unidades
                };

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(int id, ProductionOrderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        List<Inventory> Inventory = await _context.Inventories
                                                        .Include(i => i.RawMaterial)
                                                            .ThenInclude(r => r.Color)
                                                        .Include(i => i.RawMaterial)
                                                            .ThenInclude(r => r.Fabric)
                                                        .Where(i => i.RawMaterial.Id == model.RawMaterialId)
                                                        .ToListAsync();
                        float existence = 0;
                        foreach (Inventory item in Inventory)
                        {
                            existence += item.Existencia;
                        }
                        Garment garment = await _context.Garments.FindAsync(model.GarmentId);
                        float cant = model.Unidades * garment.ConsumoInvUnd;
                        if (existence >= cant)
                        {
                            ProductionOrder productionOrder = new()
                            {
                                Unidades = model.Unidades,
                                RawMaterial = await _context.RawMaterials
                                                .Include(r => r.Color)
                                                .Include(r => r.Fabric)
                                                .Include(r => r.Inventories)
                                                .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId),
                                Garment = garment
                            };
                            bool num = false;
                            foreach (Inventory item in Inventory)
                            {
                                if (num == false)
                                {
                                    if (item.Existencia >= cant)
                                    {
                                        item.Existencia -= cant;
                                        num = true;
                                    }
                                    else
                                    {
                                        cant -= item.Existencia;
                                        item.Existencia = 0;
                                    }
                                }
                            }
                            _context.Add(productionOrder);
                            await _context.SaveChangesAsync();
                            _flashMessage.Info("Orden creada con exito");
                        }
                        else
                        {
                            model.Colors = await _combosHelper.GetComboColorsAsync();
                            model.Fabrics = await _combosHelper.GetComboFabricsAsync();
                            model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync();
                            model.Garments = await _combosHelper.GetComboGarmentsAsync();
                            _flashMessage.Danger("No hay inventario suficiente.");
                            return View(model);
                        }
                    }

                    else
                    {

                    }
                }
                catch (DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException.Message.Contains("duplicate"))
                    {
                        _flashMessage.Danger("Ya existe un registro con el mismo nombre.");
                    }
                    else
                    {
                        _flashMessage.Danger(dbUpdateException.InnerException.Message);
                    }
                    return View(model);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(model);
                }
                model.Colors = await _combosHelper.GetComboColorsAsync();
                model.Fabrics = await _combosHelper.GetComboFabricsAsync();
                model.RawMaterials = await _combosHelper.GetComboRawMaterialsAsync();
                return RedirectToAction(nameof(Index));

            }           
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
