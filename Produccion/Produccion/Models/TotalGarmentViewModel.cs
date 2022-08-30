using Produccion.Data.Entities;

namespace Produccion.Models
{
    public class TotalGarmentViewModel
    {
        public Garment Garment { get; set; }

        public int Cantidad { get; set; }

        public Color Color { get; set; }
    }
}
