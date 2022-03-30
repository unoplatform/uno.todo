using System.Net;

namespace ToDo.Models
{
    public class ResponseService<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public T? Data { get; set; }
    }
}
