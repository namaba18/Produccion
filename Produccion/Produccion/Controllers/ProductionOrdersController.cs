using Microsoft.AspNetCore.Mvc;
using Produccion.Data;

namespace Produccion.Controllers
{
    public class ProductionOrdersController : Controller
    {
        private readonly DataContext _context;

        public ProductionOrdersController(DataContext context)
        {
            _context = context;
        }


    }
}
