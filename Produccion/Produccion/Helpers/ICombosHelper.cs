using Microsoft.AspNetCore.Mvc.Rendering;

namespace Produccion.Helpers
{
    public interface ICombosHelper
    {
        Task<IEnumerable<SelectListItem>> GetComboColorsAsync();
        Task<IEnumerable<SelectListItem>> GetComboFabricsAsync();
        Task<IEnumerable<SelectListItem>> GetComboRawMaterialsAsync(int colorId);
        Task<IEnumerable<SelectListItem>> GetComboGarmentsAsync();

    }
}
