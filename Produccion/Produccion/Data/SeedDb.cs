using Produccion.Data.Entities;
using Produccion.Enums;
using Produccion.Helpers;

namespace Produccion.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckColorsAsync();
            await CheckFabricsAsync();
            await CheckGarmentsAsync();
            await CheckRawMaterialsAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Natalia", "Mantilla", "manti@yopmail.com", UserType.Admin);

        }

        private async Task<User> CheckUserAsync(
            string document,
            string firstName,
            string lastName,
            string email,
            UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    Document = document,
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
            }

            return user;
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckRawMaterialsAsync()
        {
            if(!_context.RawMaterials.Any())
            {
                _context.RawMaterials.Add(new RawMaterial
                {
                    Nombre = "Franela",
                    Color = new Color { Nombre = "Amarillo"},
                    Fabric = new Fabric { Nombre = "Algodón" }
                });
                _context.RawMaterials.Add(new RawMaterial
                {
                    Nombre = "Lino",
                    Color = new Color { Nombre = "Blanco" },
                    Fabric = new Fabric { Nombre = "Nylon" }
                });
            }
            
            await _context.SaveChangesAsync();
        }

        private async Task CheckColorsAsync()
        {
            if (!_context.Colors.Any())
            {
                _context.Colors.Add(new Color { Nombre = "Azul" });
                _context.Colors.Add(new Color { Nombre = "Morado" });
                _context.Colors.Add(new Color { Nombre = "Naranja" });
                _context.Colors.Add(new Color { Nombre = "Negro" });
                _context.Colors.Add(new Color { Nombre = "Rojo" });
                _context.Colors.Add(new Color { Nombre = "Rosado" });
                _context.Colors.Add(new Color { Nombre = "Verde" });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckFabricsAsync()
        {
            if(!_context.Fabrics.Any())
            {
                _context.Fabrics.Add(new Fabric { Nombre = "Lana" });
                _context.Fabrics.Add(new Fabric { Nombre = "Poliester" });
            }
            await _context.SaveChangesAsync();
        }

        private async Task CheckGarmentsAsync()
        {
            if (!_context.Garments.Any())
            {
                _context.Garments.Add(new Garment { Nombre = "Camisa de hombre", ConsumoInvUnd = 1 });
                _context.Garments.Add(new Garment { Nombre = "Pantalón de hombre", ConsumoInvUnd = 2 });
                _context.Garments.Add(new Garment { Nombre = "Camiseta de hombre", ConsumoInvUnd = 1 });
                _context.Garments.Add(new Garment { Nombre = "Camisa de mujer" , ConsumoInvUnd = 1 });
                _context.Garments.Add(new Garment { Nombre = "Pantalón de mujer" , ConsumoInvUnd = 2 });
            }
            await _context.SaveChangesAsync();
        }

    }
}
