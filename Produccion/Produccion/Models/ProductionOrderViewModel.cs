using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Produccion.Models
{
    public class ProductionOrderViewModel
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }

        [Display(Name = "Unidades")]
        public int Unidades { get; set; }
        [Display(Name = "Prenda")]
        public int GarmentId { get; set; }

        public IEnumerable<SelectListItem>? Garments { get; set; }
        [Display(Name = "Materia Prima")]
        public int RawMaterialId { get; set; }
        public IEnumerable<SelectListItem>? RawMaterials { get; set; }
        [Display(Name = "Color")]
        public int ColorId { get; set; }
        public IEnumerable<SelectListItem>? Colors { get; set; }
        [Display(Name = "Tipo de tela")]
        public int FabricId { get; set; }
        public IEnumerable<SelectListItem?> Fabrics { get; set; }
    }
}
