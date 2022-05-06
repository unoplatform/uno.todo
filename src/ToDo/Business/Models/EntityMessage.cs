namespace ToDo.Business.Models;

public record EntityMessage<T>(EntityChange Change, T Value)
{
}

public enum EntityChange
{
	Create,
	Update,
	Delete,
}
