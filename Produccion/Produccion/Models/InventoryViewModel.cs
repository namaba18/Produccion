using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

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

        [Display(Name = "Color")]
        public int ColorId { get; set; }
        public IEnumerable<SelectListItem>? Colors { get; set; }

        [Display(Name = "Tipo tela")]
        public int FabricId { get; set; }
        public IEnumerable<SelectListItem>? Fabrics { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debes de seleccionar un color y un tipo de tela.")]
        [Display(Name = "Materia Prima")]
        public int RawMaterialId { get; set; }        
        public IEnumerable<SelectListItem>? RawMaterials { get; set; }
    }
}
