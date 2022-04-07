using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Business.Entities
{
	public class UserContext
	{
		public string? UserIdentifier { get; set; }
		public string? DisplayName { get; set; }
		public string? Name { get; set; }
		public string? Surname { get; set; }
		public string? Country { get; set; }
		public string? EmailAddress { get; set; }
		public bool IsLoggedOn { get; set; }
		public string? AccessToken { get; set; }
	}
}
