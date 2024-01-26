using System.Net;

namespace MagicVilla_Web.Models
{
    public class APIResponse
    {
        public APIResponse()
        {
            ErrorMessages = new List<string>();
        }
        public HttpStatusCode StatusCode { get; set; }

        public bool IsExitoso { get; set; } = true;

        public List<String> ErrorMessages { get; set; }

        public object Resultado { get; set; }
    }
}
