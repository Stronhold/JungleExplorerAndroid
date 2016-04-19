using System;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Content;


namespace JungleExplorer
{
	public class CustomDialog:Android.Support.V4.App.DialogFragment
	{
		Button Button_Dismiss;
		EditText editText_altitude;
		EditText editText_latitude;

		public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			// Android 3.x+ still wants to show title: disable
			Dialog.Window.RequestFeature(WindowFeatures.NoTitle);

			// Create our view
			var view = inflater.Inflate(Resource.Layout.DefaultLocationDialog, container, true);

			editText_altitude = view.FindViewById<EditText> (Resource.Id.edit_Altitude);
			editText_latitude = view.FindViewById<EditText> (Resource.Id.edit_Latitude);
			// Handle dismiss button click
			Button_Dismiss = view.FindViewById<Button>(Resource.Id.Button_Dismiss);
			Button_Dismiss.Click += Button_Dismiss_Click;

			return view;
		}

		public override void OnResume()
		{
			// Auto size the dialog based on it's contents
			Dialog.Window.SetLayout(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);

			// Make sure there is no background behind our view
			Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));

			// Disable standard dialog styling/frame/theme: our custom view should create full UI
			SetStyle(Android.Support.V4.App.DialogFragment.StyleNoFrame, Android.Resource.Style.Theme);

			base.OnResume();
		}

		private void Button_Dismiss_Click (object sender, EventArgs e)
		{
			var prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity); 
			ISharedPreferencesEditor editor = prefs.Edit();
			if (editText_altitude.Text == string.Empty) {
				editText_altitude.Text = 0 + "";
			}
			if (editText_latitude.Text == string.Empty) {
				editText_latitude.Text = 0 + "";
			}
			var altitude = Convert.ToInt32(editText_altitude.Text);
			var latitude = Convert.ToInt32(editText_latitude.Text);
			editor.PutInt ("latitude", latitude);
			editor.PutInt ("altitude", altitude);
			editor.Commit ();
			Dismiss();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			// Unwire event
			if (disposing)
				Button_Dismiss.Click -= Button_Dismiss_Click;
		}
	}
}

