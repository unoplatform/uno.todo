namespace ToDo.Business.Models;

public record EntityMessage<T>(EntityChange Change, T Value)
{
}

public enum EntityChange
{
	Created,
	Updated,
	Deleted,
}
