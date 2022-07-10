using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Produccion.Models
{
    public class InventoryViewModel
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }

        [Display(Name = "Cantidad")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Cantidad { get; set; }

        public int RawMaterialId { get; set; }

        [Display(Name = "Materia Prima")]
        public IEnumerable<SelectListItem>? RawMaterials { get; set; }
    }
}
