using MagicVilla.Modelos;
using MagicVilla.Modelos.Dto;

namespace MagicVilla.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        bool IsUsuarioUnico(string userName);

        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDto);

        //Task<Usuario> Registrar(RegistroRequestDTO registroRequestDto
        //
        Task<UsuarioDto> Registrar(RegistroRequestDTO registroRequestDto);
    }
}
