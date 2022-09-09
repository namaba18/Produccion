using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;
using Vereyon.Web;
using static Produccion.Helpers.ModalHelper;

namespace Produccion.Controllers
{
    public class InventoriesController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IFlashMessage _flashMessage;

        public InventoriesController(DataContext context, ICombosHelper combosHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _flashMessage = flashMessage;
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

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            InventoryViewModel model = new()
            {
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0),
                Cantidad = 0
            };
            if (id == 0)
            {
                return View(model);
            }
            else
            {
                Inventory inventory = await _context.Inventories
                .Include(r => r.RawMaterial)
                .ThenInclude(i => i.Color)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .FirstOrDefaultAsync(r => r.Id == id);

                model = new()
                {
                    Id = inventory.Id,
                    Colors = await _combosHelper.GetComboColorsAsync(),
                    ColorId = inventory.RawMaterial.Color.Id,
                    Fabrics = await _combosHelper.GetComboFabricsAsync(),
                    FabricId = inventory.RawMaterial.Fabric.Id,
                    RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(),
                    RawMaterialId = inventory.RawMaterial.Id,
                    Cantidad = inventory.Cantidad
                };

                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, InventoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
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
                        _flashMessage.Info("Registro creado");
                    }
                    else
                    {
                        Inventory inventory = new()
                        {
                            Id = model.Id,
                            RawMaterial = await _context.RawMaterials
                                            .Include(r => r.Color)
                                            .Include(r => r.Fabric)
                                            .FirstOrDefaultAsync(r => r.Id == model.RawMaterialId),
                            Cantidad = model.Cantidad,                            
                        };
                        _context.Update(inventory);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro actualizado");
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

        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            Inventory inventory = await _context.Inventories
                .Include(r => r.RawMaterial)
                .FirstOrDefaultAsync(r => r.Id == id);
            try
            {
                _context.Inventories.Remove(inventory);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");

            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el registro porque tiene datos relacionados.");

            }
            return RedirectToAction(nameof(Index));
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
