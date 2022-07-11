using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Produccion.Data.Entities
{
    public class RawMaterial
    {
        public int Id { get; set; }

        [Display(Name = "Materia Prima")]
        [MaxLength(50)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Nombre { get; set; }

        [Display(Name = "Tela")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [JsonIgnore]
        public Fabric Fabric { get; set; }
        [Display(Name = "Color")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [JsonIgnore]
        public Color Color { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public ICollection<ProductionOrder> ProductionOrders { get; set; }
        

    }
}
