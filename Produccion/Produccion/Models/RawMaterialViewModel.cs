using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Produccion.Models
{
    public class RawMaterialViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Nombre { get; set; }
        [Display(Name = "Color")]
        public int ColorId { get; set; }
        public IEnumerable<SelectListItem>? Colors { get; set; }
        [Display(Name = "Tipo de Tela")]
        public int FabricId { get; set; }
        public IEnumerable<SelectListItem>? Fabrics { get; set; }

    }
}
