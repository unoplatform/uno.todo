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
		var listId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgAuAAADIptfVB-VcUaFb7L0jgOsSQEAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAA";
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
		var listId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgAuAAADIptfVB-VcUaFb7L0jgOsSQEAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAA";
		var taskId = "AQMkADAwATNiZmYAZC00YjBmLWQzOTItMDACLTAwCgBGAAADIptfVB-VcUaFb7L0jgOsSQcAcYiQalobw0a8Voz8RAJUmAAAAjxiAAAAcYiQalobw0a8Voz8RAJUmAAAAo0oAAAA";

		//Act
		var result = await service.GetAsync(listId, taskId, CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<TaskData>(result);
	}
}
