﻿namespace ToDo;

[Headers("Content-Type: image/jpg")]
public interface IUserProfilePictureEndpoint
{
	[Get("/photo/$value")]
	[Headers("Authorization: Bearer")]
	Task<byte[]> Get(CancellationToken ct);
}
