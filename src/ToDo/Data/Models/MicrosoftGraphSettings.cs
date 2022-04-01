namespace ToDo.Data.Models;

public record MicrosoftGraphSettings
{
    public string? Endpoint { get; set; }
    public string? AccessToken { get; set; }
}
