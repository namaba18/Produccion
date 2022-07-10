using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class ProductionOrder
    {
        [Display(Name = "Consecutivo")]
        public int Id { get; set; }

        [Display(Name = "Unidades")]
        public int Unidades { get; set; }

        [Display(Name = "Prenda")]
        public Garment Garment { get; set; }

        [Display(Name = "Materia Prima")]
        public RawMaterial RawMaterial { get; set; }
    }
}
