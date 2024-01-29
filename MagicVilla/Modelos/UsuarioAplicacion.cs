using Microsoft.AspNetCore.Identity;

namespace MagicVilla.Modelos
{
    public class UsuarioAplicacion :IdentityUser
    {
        public string Nombres { get; set; }
    }
}
