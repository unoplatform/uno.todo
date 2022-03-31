
namespace ToDo.Models
{
    public record MicrosoftGraphSettings
    {
        public MicrosoftGraphSettings(string endPoint, string accessToken)
        {
            Endpoint = endPoint;
            AccessToken = accessToken;
        }
        public string Endpoint { get; private set; }
        public string AccessToken { get; private set; }
    }
}
