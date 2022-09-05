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
    public class RawMaterialsController : Controller
    {
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;
        private readonly IFlashMessage _flashMessage;

        public RawMaterialsController(DataContext context, ICombosHelper combosHelper, IFlashMessage flashMessage)
        {
            _context = context;
            _combosHelper = combosHelper;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.RawMaterials
                .Include(r => r.Color)
                .Include(r => r.Fabric)
                .ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            RawMaterialViewModel model = new()
            {
                Colors = await _combosHelper.GetComboColorsAsync(),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
            };
            if (id == 0)
            {
                return View(model);
            }
            else
            {
                RawMaterial rawMaterial = await _context.RawMaterials
                .Include(r => r.Color)
                .Include(r => r.Fabric)
                .FirstOrDefaultAsync(r => r.Id == id);

                model = new()
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

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, RawMaterialViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        RawMaterial rawMaterial = new()
                        {
                            Nombre = model.Nombre,
                            Color = await _context.Colors.FindAsync(model.ColorId),
                            Fabric = await _context.Fabrics.FindAsync(model.FabricId),
                        };
                        _context.Add(rawMaterial);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado");
                    }
                    else
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
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "Index", _context.RawMaterials.ToList()) });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", model) });

        }
        
        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
            RawMaterial rawMaterial = await _context.RawMaterials
                .Include(r => r.Inventories)
                .Include(r => r.ProductionOrders)
                .FirstOrDefaultAsync(r => r.Id == id);
            try
            {
                _context.RawMaterials.Remove(rawMaterial);
                await _context.SaveChangesAsync();
                _flashMessage.Info("Registro borrado.");

            }
            catch
            {
                _flashMessage.Danger("No se puede borrar el registro porque tiene datos relacionados.");

            }
            return RedirectToAction(nameof(Index));
        }  

    }
}
