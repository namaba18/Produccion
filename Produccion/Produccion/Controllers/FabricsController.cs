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
    public class FabricsController : Controller
    {
        private readonly DataContext _context;
        private readonly IFlashMessage _flashMessage;

        public FabricsController(DataContext context, IFlashMessage flashMessage)
        {
            _context = context;
            _flashMessage = flashMessage;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Fabrics.ToListAsync());
        }

        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(int id)
        {
            if (id == 0)
            {
                return View(new Fabric());
            }
            else
            {
                Fabric fabric = await _context.Fabrics.FindAsync(id);
                if (fabric == null)
                {
                    return NotFound();
                }

                return View(fabric);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(int id, Fabric fabric)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        _context.Add(fabric);
                        await _context.SaveChangesAsync();
                        _flashMessage.Info("Registro creado");
                    }
                    else
                    {
                        _context.Update(fabric);
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
                    return View(fabric);
                }
                catch (Exception exception)
                {
                    _flashMessage.Danger(exception.Message);
                    return View(fabric);
                }
                return Json(new { isValid = true, html = ModalHelper.RenderRazorViewToString(this, "Index", _context.Fabrics.ToList()) });
            }

            return Json(new { isValid = false, html = ModalHelper.RenderRazorViewToString(this, "AddOrEdit", fabric) });

        }
        [Authorize(Roles = "Admin")]
        [NoDirectAccess]
        public async Task<IActionResult> Delete(int? id)
        {

            Fabric fabric = await _context.Fabrics
                .FirstOrDefaultAsync(m => m.Id == id);
            try
            {
                _context.Fabrics.Remove(fabric);
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
