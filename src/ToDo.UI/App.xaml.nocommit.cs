namespace ToDo;

public partial class App 
{
	private Task<string> GetAccessToken()
	{
		// TODO: This needs to be connected to the authentication process to return the current Access Token
		// In the meantime do NOT commit an actual access token into the repo
		// To get a temporary access token for development, go to https://developer.microsoft.com/en-us/graph/graph-explorer
		// Sign in and select "get To Do task lists" from the sample queries
		// Run the query, and then select the Access token tab. Paste the access token here for development ONLY
		// The access token will expire periodically, so if you start to get errors, you may need to update the access token
		return Task.FromResult("Put Access Token Here - DO NOT commit changes to this file");
	}
}
