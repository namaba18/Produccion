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
             IEnumerable<Inventory> Inventory = await _context.Inventories
                .Include(i => i.RawMaterial)
                    .ThenInclude(r => r.Color)
                .Include(i => i.RawMaterial)
                    .ThenInclude(r => r.Fabric)
                .Where(i => i.RawMaterial.Id == model.RawMaterialId)
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

            return View(InventoryPartial);
        }
        

        public async Task<IActionResult> Garment(HomeViewModel model)
        {            
            Fabric fabric = await _context.Fabrics
                .Include(f => f.RawMaterials)
                    .ThenInclude(r => r.ProductionOrders)
                    .ThenInclude(p => p.Garment)
                .Include(f => f.RawMaterials)
                    .ThenInclude(r => r.Color)
                .FirstOrDefaultAsync(f => f.Id == model.FabricId);

            IEnumerable<RawMaterial> rawMaterials = await _context.RawMaterials
                .Include(r => r.ProductionOrders)
                    .ThenInclude(p => p.Garment)
                .Include(r => r.Color)
                .Where(r => r.Fabric == fabric)
                .ToListAsync();

            List<ProductionOrder> Orders = new();
            foreach (var item in rawMaterials)
            {
                IEnumerable<ProductionOrder> productionOrders = await _context.ProductionOrders
                    .Include(p => p.Garment)
                    .Include(p => p.RawMaterial)
                        .ThenInclude(r => r.Fabric)
                    .Include(p => p.RawMaterial)
                        .ThenInclude(r => r.Color)
                    .Where(p => p.RawMaterial == item)
                    .ToListAsync();
                foreach(ProductionOrder item2 in productionOrders)
                {
                     Orders.Add(item2);                    
                }
            }

            List<TotalGarmentViewModel> garments = new();
            foreach (var item in Orders)
            {
                if( garments.Count == 0)
                {
                    garments.Add(new TotalGarmentViewModel {
                        Garment = item.Garment,
                        Cantidad = item.Unidades,
                        Color = item.RawMaterial.Color
                    });
                   
                }
                else 
                {
                    if (garments.Any(g => g.Garment.Id == item.Garment.Id && g.Color == item.RawMaterial.Color))
                    {
                        garments.Find(g => g.Garment.Id == item.Garment.Id && g.Color == item.RawMaterial.Color).Cantidad += item.Unidades;
                    }
                    else { 
                        garments.Add(new TotalGarmentViewModel
                        {
                            Garment = item.Garment,
                            Cantidad = item.Unidades,
                            Color = item.RawMaterial.Color
                        });
                    }
                }                  
            }

            

            return View(garments);
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }

    }
}