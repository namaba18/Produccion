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
                RawMaterials = await _combosHelper.GetComboRawMaterialsAsync(),
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
             IEnumerable<Inventory> inventory = await _context.Inventories
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Color)
                .Include(i => i.RawMaterial)
                .ThenInclude(r => r.Fabric)
                .Where(i => i.RawMaterial.Id == model.RawMaterialId)
                .ToListAsync();

            return View(inventory);
        }

        public async Task<IActionResult> Garment(HomeViewModel model)
        {            
            Fabric fabric = await _context.Fabrics
                .Include(f => f.RawMaterials)
                    .ThenInclude(r => r.ProductionOrders)
                .Include(f => f.RawMaterials)
                    .ThenInclude(r => r.Color)
                .FirstOrDefaultAsync(f => f.Id == model.FabricId);

            IEnumerable<RawMaterial> rawMaterials = await _context.RawMaterials
                .Include(r => r.ProductionOrders)
                .Where(r => r.Fabric == fabric)
                .ToListAsync();

            List<ProductionOrder> Orders = new();
            foreach (var item in rawMaterials)
            {
                IEnumerable<ProductionOrder> productionOrders = await _context.ProductionOrders
                    .Include(p => p.Garment)
                    .Include(p => p.RawMaterial)
                    .ThenInclude(r => r.Color)
                    .Where(p => p.RawMaterial == item)
                    .ToListAsync();
                foreach(ProductionOrder item2 in productionOrders)
                {
                     Orders.Add(item2);                    
                }
            }
            List<Garment> garments = new();
            foreach (var item in Orders)
            {
                if( garments.Count == 0)
                {
                    garments.Add(item.Garment);
                }
                else
                {
                    if (!garments.Any(g => g.Id == item.Garment.Id))
                    {
                        garments.Add(item.Garment);
                    }
                }               
                
            }

            return View(garments);
        }
    }
}