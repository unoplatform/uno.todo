using System.Net;

namespace ToDo;

public static class ResponseData<T>
{
    public static Response<T> Get(HttpStatusCode statusCode, T? data = default)
    {
        return new Response<T>()
        {
            Data = data,
            StatusCode = statusCode
        };
    }
}
