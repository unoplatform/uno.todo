using NUnit.Framework;
using System.Net.Http;
using System.Threading;

namespace ToDo.Tests.Services;

internal class UserProfilePictureEndpointTests : BaseEndpointTests<IUserProfilePictureEndpoint>
{
	[SetUp]
	public void Setup() { }

	[Test]
	public async System.Threading.Tasks.Task Get_UserProfilePicture_ShouldReturn_BytesArray()
	{
		//Arrange

		//Act
		var result = await service.GetAsync(CancellationToken.None);

		//Assert
		Assert.IsInstanceOf<HttpContent>(result);
	}

}
