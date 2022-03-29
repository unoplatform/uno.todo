namespace ToDo.Models;

public record ChangePasswordObj
{
    public string? currentPassword { get; set; } 
    public string? newPassword { get; set; }
}
