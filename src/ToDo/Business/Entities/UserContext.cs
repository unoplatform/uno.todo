using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Business.Entities
{
	public record UserContext
	{
		public string? name { get; set; }
		public string? preferred_username { get; set; }
	}
}
