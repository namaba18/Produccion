using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;
using Produccion.Data.Entities;
using Produccion.Helpers;
using Produccion.Models;
using System.Diagnostics;

namespace Produccion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        private readonly ICombosHelper _combosHelper;

        public HomeController(ILogger<HomeController> logger, DataContext context, ICombosHelper combosHelper)
        {
            _logger = logger;
            _context = context;
            _combosHelper = combosHelper;
        }

        public async Task<IActionResult> Index()
        {
            HomeViewModel model = new()
            {
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(0),
                Fabrics = await _combosHelper.GetComboFabricsAsync(),
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Inventory(HomeViewModel model)
        {
            Inventory inventory = await _context.Inventories
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Color)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .FirstOrDefaultAsync(i => i.RawMaterial.Id == model.RawMaterialId);

            return View(inventory);
        }

        public async Task<IActionResult> Garment(HomeViewModel model)
        {
            Fabric fabric = await _context.Fabrics
                .Include(f => f.RawMaterials)
                .ThenInclude(r => r.ProductionOrders)
                .ThenInclude(p => p.Garment)
                .FirstOrDefaultAsync(f => f.Id == model.FabricId);


            return View();
        }
    }
}