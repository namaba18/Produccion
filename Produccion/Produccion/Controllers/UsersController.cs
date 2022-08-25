using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Common;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;

namespace Produccion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(DataContext context, IUserHelper userHelper, IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Id = Guid.Empty.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.AddUserAsync(model);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este correo ya está siendo usado.");
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Users", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(
                    $"{model.FirstName} {model.LastName}",
                    model.Username,
                    "Produccion - Confirmación de Email",
                    $"<h1>Producción - Confirmación de Email</h1>" +
                        $"Para habilitar el usuario por favor hacer click en el siguiente link:, " +
                        $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar el usuario han sido enviadas al correo.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);

            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }


    }
}
