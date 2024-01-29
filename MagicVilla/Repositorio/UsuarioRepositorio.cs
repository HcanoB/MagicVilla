using AutoMapper;
using MagicVilla.Datos;
using MagicVilla.Modelos;
using MagicVilla.Modelos.Dto;
using MagicVilla.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<UsuarioAplicacion> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration, UserManager<UsuarioAplicacion> userManager
                                   , IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _secretKey = configuration.GetValue<string>("APISettings:Secret");
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;

        }

        public bool IsUsuarioUnico(string userName)
        {
            var usuario = _db.UsuariosAplicacion.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (usuario == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto)
        {
            var usuario = _db.UsuariosAplicacion.FirstOrDefault(
                                                            u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            //&& u.Password == loginRequestDto.Password);

            bool isValido = await _userManager.CheckPasswordAsync(usuario, loginRequestDto.Password);

            if (usuario == null || isValido == false)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    Usuario = null
                };
            }
            //si el usuario existe generamos el JW Token
            var roles = await _userManager.GetRolesAsync(usuario);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    //new Claim(ClaimTypes.Name,usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name,usuario.UserName.ToString()),
                    //new Claim(ClaimTypes.Role,usuario.Rol)
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDTO loginResponseDto = new()
            {
                Token = tokenHandler.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDto>(usuario),
                //Rol = roles.FirstOrDefault()
            };

            return loginResponseDto;

        }

        public async Task<UsuarioDto> Registrar(RegistroRequestDTO registroRequestDto)
        {
            UsuarioAplicacion usuario = new()
            {
                UserName = registroRequestDto.UserName,
                //Password = registroRequestDto.Password,
                Nombres = registroRequestDto.Nombres,
                //Rol = registroRequestDto.Rol,
                Email = registroRequestDto.UserName,
                NormalizedEmail = registroRequestDto.UserName.ToUpper()
            };
            //await _db.Usuarios.AddAsync(usuario);
            //await _db.Usuarios.AddAsync(usuario);
            //await _db.SaveChangesAsync();

            //usuario.Password = "";

            // return usuario;

            try
            {
                var resultado = await _userManager.CreateAsync(usuario, registroRequestDto.Password);
                if (resultado.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                    }
                    await _userManager.AddToRoleAsync(usuario, "admin");
                    var usuarioAp = _db.UsuariosAplicacion.FirstOrDefault(u => u.UserName == registroRequestDto.UserName);
                    return _mapper.Map<UsuarioDto>(usuarioAp);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return new UsuarioDto();
        }

    }
}
