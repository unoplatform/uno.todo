using NUnit.Framework;
using ToDo;

namespace ToDo.Tests;

public class AppInfoTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void AppInfoCreation()
	{
		var appInfo = new AppInfo { Title = "Test" };

		Assert.AreEqual("Test", appInfo.Title);
	}
}
