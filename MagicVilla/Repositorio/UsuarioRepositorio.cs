using MagicVilla.Datos;
using MagicVilla.Modelos;
using MagicVilla.Modelos.Dto;
using MagicVilla.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        private string _secretKey;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _secretKey = configuration.GetValue<string>("APISettings:Secret");

        }

        public bool IsUsuarioUnico(string userName)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (usuario == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
        {
            var usuario = _db.Usuarios.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower() &&
                                                            u.Password == loginRequestDto.Password);
            if (usuario == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }
            //si el usuario existe generamos el JW Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role,usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                Usuario = usuario,
            };

            return loginResponseDto;

        }

        public async Task<Usuario> Registrar(RegistroRequestDTO registroRequestDto)
        {
            Usuario usuario = new()
            {
                UserName = registroRequestDto.UserName,
                Password = registroRequestDto.Password,
                Nombres = registroRequestDto.Nombres,
                Rol = registroRequestDto.Rol,
            };
            await _db.Usuarios.AddAsync(usuario);
            await _db.SaveChangesAsync();

            usuario.Password = "";

            return usuario;
        }

    }
}
