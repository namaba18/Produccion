using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class Garment
    {
        public int Id { get; set; }

        [Display (Name ="Prenda") ]
        [Required]
        public string Nombre { get; set; }

        [Display(Name = "Consumo de inventario")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public float ConsumoInvUnd { get; set; }

        public ICollection<ProductionOrder>? ProductionOrders { get; set; }
    }
}
