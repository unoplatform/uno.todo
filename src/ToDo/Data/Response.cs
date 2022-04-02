using System.Net;

namespace ToDo;

public class Response<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }
}
