using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
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
		return Task.FromResult("EwCAA8l6BAAUkj1NuJYtTVha+Mogk+HEiPbQo04AAdGkhAQudgm+W+pflNtzFj4iZLRkT5NCz+3bFSxfYW5zLozXrP1IlbNipaKvjZgSm7yLqdItrMJOLsPdsL1OTp/AeudJHgL5RAecHTLCZOyv2jfsIR/zwzLG+FfNUVMjGmIZq1+hc2ZX8WE2FWSZam1+uLlyGt0ABs0H5tZtN2UU9F4u5jfpyZFlDiWYzulZ1B+tBpZwIzIvBqmOYMxzF9UBdRucAuLgAuwCsvQ1+0Jy8u/pIXcnuxP3fGHZZsHq1IpMxdvJeKALpVj11pGbt//nssaKVzZS86S5QYpCagCrOpBpEu7HUDaJNa9I5ZKpzCYtQMihu/n2l5EiHtKqhB4DZgAACD1AHfdzkKuAUALAKOuUce7xw6r0R6SpjcRpk6Vl1HXm6k7LY0KU3yk2AUTtg0TwSaPwC2qmccYTZ3rAolMy/sJQxyEjxybJfVvOC4uLCcXTSFXNsUCVSx6wVPP59hNfK9KacUWUIwZxAlQtUYNvRnvJBrbu9QthpDycmpkDNGVPKLOw+BrWwAJKxyl6XsBia/V5uJutltKu2L3wRJh9Mp3RuE/8fhmFKz/KgpQM2yRLS94+775lxwkfKm1E5V2S1ZzHfKpZbgQMKgEjWaxXzfaPDH7N+ipwFCC2vFyprGvNdtQfbJSPoiucQhbiM7jLb/WVhh1wwJmtFwptVA8mZaXQI891uaGgEnfu5IAMxh/a7KFKqkjcGCSdK4d92mh7L0MqcuO3+MLujw4bAKdSMneRqv0Y9rHg6k2/Zkuj/DQMrMBek4iEPHAmY8ht0t5Qb3CA8EzBadKJMeuKcoGB01HwSXWefDN0Xq0gfzOwhzdGd7vy7Niki/bjlu/899NYSd7ewZg4bxANB1FkZOim/Dm4XoD5J15qQNVuZZ/SyOG0moOYlV+PW4CF4SfZlycwYFjyc1QuejUi2/WOozXpb4NXBg7PqNjcqzi6zjhet0g928iGEmjucTlWHO9gi3M67JYU5dsGpcgJOlc9WdvZMaukVdXFmgN8yu1G/EjGCaaw7yggy7wcV23/tqRx7+De+ultf+hXJKcG0Q2nlwIRWAHf3CYlXj/9qdx4WpSeaNvNHVcuVSYcn4k6dsEFstugVl875bI0iqFvGbU4ez28hmj5w+jn8jy0WRW5mAI=");
	}
}
