using System.Net;

namespace Tienda.Modelo
{
    public class APIResponse{
        public HttpStatusCode statusCode { get; set; }
        public bool isExitoso { get; set; }
        public List<string> ErrorMessages { get; set; }
        public Object Resultado { get; set; }

        public APIResponse(){
            isExitoso = true;
        }
    }
}
