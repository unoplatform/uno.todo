using Android.App;
using Android.Views;
using Android.Content;
using Microsoft.Identity.Client;

namespace ToDo.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
	{
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			AuthenticationContinuationHelper
				.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
		}
	}
}

