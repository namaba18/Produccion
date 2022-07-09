using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class RawMaterial
    {
        public int Id { get; set; }

        [Display(Name = "Materia Prima")]
        [MaxLength(50)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Nombre { get; set; }

        public IEnumerable<Fabric> Fabrics { get; set; }
        public IEnumerable<Color> Colors { get; set; }
        

    }
}
