
namespace ToDo.Business.Entities
{
	public record UserContext
	{
		public string? Name { get; init; }

		public string? Email { get; init; }

		public string? AccessToken { get; init; }
	}
}
