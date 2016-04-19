using System;
using Android.Preferences;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Net;
using Location.Droid;
using Android.Content;


namespace JungleExplorer
{
	public class FragmentSettings : Android.Support.V4.App.Fragment
	{
		TextView textViewSetLocation;

		Switch stopService;
		Switch stopNotifications;

		public FragmentSettings ()
		{
		}

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.FragmentSettings, container, false);
			textViewSetLocation = view.FindViewById<TextView> (Resource.Id.textViewLocation);

			stopService = view.FindViewById<Switch> (Resource.Id.switchGPS);
			stopNotifications = view.FindViewById<Switch> (Resource.Id.SwitchNotifications);

			textViewSetLocation.Click += TextViewSyncAnimals_Click;

			stopService.CheckedChange += checked_switch;
			stopNotifications.CheckedChange += checked_notifications;
			ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity); 
			var myValue= prefs.GetBoolean("service", false);
			stopService.Checked = myValue;
			myValue = prefs.GetBoolean ("notifications", true);
			stopNotifications.Checked = myValue;
			return view;
		}

		void TextViewSyncAnimals_Click (object sender, EventArgs e)
		{
			((MainActivity)(Activity)).ShowLocationDialog ();
		}

		void TextViewSyncScientist_Click (object sender, EventArgs e)
		{
			if (((MainActivity)Activity).HasConnectionToInternet ()) {
			} else {
				Toast.MakeText (this.Context, "There is no connection to Internet, try again later", ToastLength.Long).Show();
			}
		}

		void TextViewSyncRelations_Click (object sender, EventArgs e)
		{
			if (((MainActivity)Activity).HasConnectionToInternet ()) {
			} else {
				Toast.MakeText (this.Context, "There is no connection to Internet, try again later", ToastLength.Long).Show();
			}
		}


		void checked_switch (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (e != null) {
				if (e.IsChecked) {
					App.Current.StartLocationService ();
				} else {
					App.Current.StopLocationService ();
				}
				var prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity); 
				ISharedPreferencesEditor editor = prefs.Edit();
				editor.PutBoolean ("service", e.IsChecked);
				editor.Commit ();

			}
		}

		void checked_notifications (object sender, CompoundButton.CheckedChangeEventArgs e)
		{
			if (e != null) {
				var prefs = PreferenceManager.GetDefaultSharedPreferences(this.Activity); 
				ISharedPreferencesEditor editor = prefs.Edit();
				editor.PutBoolean ("notifications", e.IsChecked);
				editor.Commit ();

			}
		}
	}
}

