using System.Net;

namespace ToDo.Data.Models;

public class ResponseService<T>
{
    public HttpStatusCode StatusCode { get; set; }
    public T? Data { get; set; }
}
