using System.Net;

namespace ToDo.Data.Models;

public static class GetResponseService<T>
{
    public static ResponseService<T> GetResponse(HttpStatusCode statusCode, T? data = default)
    {
        return new ResponseService<T>()
        {
            Data = data,
            StatusCode = statusCode
        };
    }
}
