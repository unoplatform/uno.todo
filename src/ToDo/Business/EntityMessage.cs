namespace ToDo.Business;

public record EntityMessage<T>(EntityChange Change, T Value)
{
}

public enum EntityChange
{
	Create,
	Update,
	Delete,
}
