using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Produccion.Data;

namespace Produccion.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboColorsAsync()
        {
            List<SelectListItem> list = await _context.Colors.Select(x => new SelectListItem
            {
                Text = x.Nombre,
                Value = x.Id.ToString(),
            })
                .OrderBy(x => x.Text)
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un color...]",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboFabricsAsync()
        {
            List<SelectListItem> list = await _context.Fabrics.Select(x => new SelectListItem
            {
                Text = x.Nombre,
                Value = x.Id.ToString()
            })
                .OrderBy(x => x.Text)
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un tipo de tela...]",
                Value = "0"
            });

            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetComboRawMaterialsAsync(int colorId)
        {
            {
                List<SelectListItem> list = await _context.RawMaterials
                    .Where(r => r.Color.Id == colorId)
                    .Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.Id.ToString(),
                    })
                    .OrderBy(c => c.Text)
                    .ToListAsync();

                list.Insert(0, new SelectListItem { Text = "[Seleccione una Materia Prima...]", Value = "0" });

                return list;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetComboRawMaterialsAsync()
        {
            {
                List<SelectListItem> list = await _context.RawMaterials
                    .Select(c => new SelectListItem
                    {
                        Text = c.Nombre,
                        Value = c.Id.ToString(),
                    })
                    .OrderBy(c => c.Text)
                    .ToListAsync();

                list.Insert(0, new SelectListItem { Text = "[Seleccione una Materia Prima...]", Value = "0" });

                return list;
            }
        }
        public async Task<IEnumerable<SelectListItem>> GetComboGarmentsAsync()
        {
            List<SelectListItem> list = await _context.Garments.Select(x => new SelectListItem
            {
                Text = x.Nombre,
                Value = x.Id.ToString(),
            })
                .OrderBy(x => x.Text)
                .ToListAsync();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una prenda...]",
                Value = "0"
            });

            return list;
        }
    }
}
