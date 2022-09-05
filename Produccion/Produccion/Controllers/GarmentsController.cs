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
    public class GarmentsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public GarmentsController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Garments.ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            if (id == 0)
            {
                return View(new Garment());
            }
            else
            {
                Garment garment = await _context.Garments.FindAsync(id);
                if (garment == null)
                {
                    return NotFound();
                }

                return View(garment);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Garment garment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        _context.Add(garment);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado");
                    }
                    else
                    {
                        _context.Update(garment);
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
                    return View(garment);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(garment);
                }
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "Index", _context.Garments.ToList()) });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", garment) });

        }

        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {

            Garment garment = await _context.Garments
                .Include(g => g.ProductionOrders)
                .FirstOrDefaultAsync(m => m.Id == id);
            try
            {
                _context.Garments.Remove(garment);
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
