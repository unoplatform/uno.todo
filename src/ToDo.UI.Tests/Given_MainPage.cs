using NUnit.Framework;
using Query = System.Func<Uno.UITest.IAppQuery, Uno.UITest.IAppQuery>;

namespace ToDo.UI.Tests
{
	public class Given_MainPage : TestBase
    {
        [Test]
        public void When_SmokeTest()
        {
			// Make sure the ViewModelButton has rendered
			App.WaitForElement(q => q.Marked("WelcomePage_Button_Wide"));

			// Query for the XamlButton and then tap it
			Query WelcomePage_GetStarted = q => q.All().Marked("WelcomePage_Button_Wide");
			App.WaitForElement(WelcomePage_GetStarted);
			App.Tap(WelcomePage_GetStarted);

			// Take a screenshot and add it to the test results
			TakeScreenshot("After tapped");

			Query important = q => q.All().Text("Important");
			App.WaitForElement(important);
			App.Tap(important);

			// Take a screenshot and add it to the test results
			TakeScreenshot("After important");

			Query payBills = q => q.All().Text("Pay bills");
			App.WaitForElement(payBills);
			App.Tap(payBills);

			Query dueTime = q => q.All().Text("Due Wed, 13 April");
			App.WaitForElement(dueTime);

			// Take a screenshot and add it to the test results
			TakeScreenshot("After pay bill");
		}
	}
}
