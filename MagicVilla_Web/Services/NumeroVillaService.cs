using MagicVilla_Utilidad;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class NumeroVillaService : BaseService, INumeroVIllaService
    {
        public readonly IHttpClientFactory _httpClient;
        private string _VillaUrl;

        public NumeroVillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClient = httpClient;
            _VillaUrl = configuration.GetValue<string>("ServiceUrls:API_URL");
        }

        public Task<T> Actualizar<T>(NumeroVillaUpdateDto dto, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.PUT,
                Datos = dto,
                URl = _VillaUrl + "api/v1/NumeroVilla/" + dto.VillaNo,
                Token = Token
            });
        }

        public Task<T> Crear<T>(NumeroVillaCreateDto dto, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.POST,
                Datos = dto,
                URl = _VillaUrl + "api/v1/NumeroVilla",
                Token = Token
            });
        }

        public Task<T> Obtener<T>(int id, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.GET,
                URl = _VillaUrl + "api/v1/NumeroVilla/" + id.ToString(),
                Token = Token
            });
        }

        public Task<T> ObtenerTodos<T>(string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.GET,
                URl = _VillaUrl + "api/v1/NumeroVilla",
                Token = Token
            });
        }

        public Task<T> Remover<T>(int id, string Token)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.DELETE,
                URl = _VillaUrl + "api/v1/NumeroVilla/" + id.ToString(),
                Token = Token
            });
        }
    }
}
