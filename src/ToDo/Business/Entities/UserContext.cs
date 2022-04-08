using System;
using System.Collections.Generic;
using System.Text;

namespace ToDo.Business.Entities
{
	public class UserContext
	{
		public string? aud { get; set; }
		public string? iss { get; set; }
		public int iat { get; set; }
		public int nbf { get; set; }
		public int exp { get; set; }
		public string? name { get; set; }
		public string? oid { get; set; }
		public string? preferred_username { get; set; }
		public string? rh { get; set; }
		public string? sub { get; set; }
		public string? tid { get; set; }
		public string? uti { get; set; }
		public string? ver { get; set; }
	}
}
