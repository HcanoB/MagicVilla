using System.Net;

namespace MagicVilla.Modelos
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsExitoso { get; set; } = true;

        public List<String> ErrorMessages { get; set; }

        public object Resultado { get; set; }
    }
}
