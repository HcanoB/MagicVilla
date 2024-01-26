using static MagicVilla_Utilidad.DS;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public APITipo APITipo { get; set; } = APITipo.GET;

        public string URl { get; set; }

        public object Datos { get; set; }

        public string Token { get; set; }
    }
}
