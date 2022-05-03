namespace ToDo;

[Headers("Authorization: Bearer")]
public interface IUserProfilePictureEndpoint
{
	[Get("/photo/$value")]
	[Headers("Content-Type: multipart/form-data")]
	Task<StreamContent> GetAsync(CancellationToken ct);
}
