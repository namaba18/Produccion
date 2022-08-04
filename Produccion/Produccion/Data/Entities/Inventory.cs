using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class Inventory
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }

        [Display(Name = "Cantidad")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Cantidad { get; set; }

        [Display(Name = "Existencia")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float Existencia { get; set; }

        [Display(Name = "Materia Prima")]
        [Required]
        public RawMaterial RawMaterial { get; set; }

    }
}
