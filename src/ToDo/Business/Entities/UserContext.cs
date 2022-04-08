
namespace ToDo.Business.Entities
{
	public record UserContext
	{
		public string? name { get; set; }
		public string? preferred_username { get; set; }
	}
}
