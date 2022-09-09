using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class ProductionOrder
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }

        [Display(Name = "Unidades")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int Unidades { get; set; }

        [Display(Name = "Prenda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public Garment Garment { get; set; }

        [Display(Name = "Materia Prima")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public RawMaterial RawMaterial { get; set; }

    }
}
