﻿using System.ComponentModel.DataAnnotations;

namespace Produccion.Data.Entities
{
    public class Color
    {
        public int Id { get; set; }

        [Display (Name="Color")]
        [MaxLength(50)]
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        public string Nombre { get; set; }
        public ICollection<RawMaterial>? RawMaterials { get; set; }
    }
}
