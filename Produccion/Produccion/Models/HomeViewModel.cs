using Microsoft.AspNetCore.Mvc.Rendering;
using Produccion.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Produccion.Models
{
    public class HomeViewModel
    {
        [Display (Name ="Materia Prima")]
        public int? RawMaterialId { get; set; }
        public IEnumerable<SelectListItem>? RawMaterials { get; set; }

        [Display(Name = "Tipo de tela")]
        public int? FabricId { get; set; }
        public IEnumerable<SelectListItem>? Fabrics { get; set; }
    }
}
