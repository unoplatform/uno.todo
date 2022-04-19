using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions.Serialization;

namespace ToDo.Tests.Services;

internal class BaseEndpointTests<T> where T : notnull
{
	protected readonly T service;

	protected BaseEndpointTests()
	{
		var host = Host.CreateDefaultBuilder()
			.UseSerialization()
			.ConfigureAppConfiguration(builder =>
			{
				var appsettingsPrefix = new Dictionary<string, string>
						{
							{ "ITaskEndpoint:Url", "https://graph.microsoft.com/beta/me" },
							{ "ITaskEndpoint:UseNativeHandler","true" }
						};
				builder.AddInMemoryCollection(appsettingsPrefix);

			})
			.ConfigureServices((context, services) =>
			{
				services.AddEndpoints(context, (sp, settings) => settings.AuthorizationHeaderValueGetter = GetAccessToken);
			})
			.Build();

		service = host.Services.GetRequiredService<T>();
	}
	private Task<string> GetAccessToken()
	{
		return Task.FromResult("**AccessToken**");
	}
}

internal class TaskEndpointTests : BaseEndpointTests<ITaskEndpoint>
{

	[SetUp]
	public void Setup() { }

	[Test]
	public async System.Threading.Tasks.Task Create_TodoTask_ShouldReturn_NewTask()
	{
		//Arrange
		var listId = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF-AAA=";
		var newTask = new TaskData { Title = "new task created from unit test" };
		//Act
		var result = await service.CreateAsync(listId, newTask, CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<TaskData>(result);
	}

	[Test]
	public async System.Threading.Tasks.Task Get_TodoTask_ShouldReturnTask()
	{
		//Arrange
		var listId = "AAMkAGM0ZTZiY2IwLTliZWEtNDM5Zi1iMDBlLTUxZDQxNWNmY2IxNgAuAAAAAAAC8Egk03A8QrAy_y5u1QQAAQD-PT2STVFATpxIXsYfLHGvAADbMaF-AAA=";
		var taskId = "AAMkAGFlMTMyOTVlLTg4MTYtNGNkYi05Y2I1LWIxNjQ3MjQzZGUwZgBGAAAAAABxiwJ7rbfvTL0IfGDSJ4lUBwAstIhkSEopRrR__AvQNI34AACzQA1BAAAstIhkSEopRrR__AvQNI34AACzQBkEAAA=";

		//Act
		var result = await service.GetAsync(listId, taskId, CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<TaskData>(result);
	}
}
