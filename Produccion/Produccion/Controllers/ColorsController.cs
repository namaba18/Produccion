using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Vereyon.Web;
using static Produccion.Helpers.ModalHelper;

namespace Produccion.Controllers
{
    public class ColorsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public ColorsController(DataContext context, IFlashMessage flashMessage) 
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Colors
                .ToListAsync());  
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            if(id == 0)
            {
                return View(new Color());
            }
            else
            {
                Color color = await _context.Colors.FindAsync(id);
                if(color == null)
                {
                    return NotFound();
                }

                return View(color);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Color color)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if(id==0)
                    {
                        _context.Add(color);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado");
                    }
                    else
                    {
                        _context.Update(color);
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
                    return View(color);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(color);
                }
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "Index", _context.Colors.ToList()) });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", color) });

        }
                
        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {
                        
            Color color = await _context.Colors
                .Include(c => c.RawMaterials)
                    .ThenInclude(r => r.Inventories)
                .Include(c => c.RawMaterials)
                    .ThenInclude(r => r.ProductionOrders)
                .FirstOrDefaultAsync(m => m.Id == id);
            try 
            {
                _context.Colors.Remove(color);
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
