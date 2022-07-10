using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Produccion.Models
{
    public class GarmentViewModel
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }
        [Display(Name = "Unidades")]
        [Required]
        public int Unidades { get; set; }
        public int RawMaterialId { get; set; }

        public IEnumerable<SelectListItem>? RawMaterials { get; set; }

        public int GarmentId { get; set; }
        public IEnumerable<SelectListItem>? Garments { get; set; }
    }
}
