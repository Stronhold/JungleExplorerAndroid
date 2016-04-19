using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.OS;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

using System.Collections.Generic;
using SupportFragment = Android.Support.V4.App.Fragment;
using Model;
using Android.Locations;
using System.Threading.Tasks;
using System.Linq;
using Location.Droid;
using Android.Util;
using Location.Droid.Services;
using Android.Support.V4.View;
using Android.Widget;
using Android.Preferences;
using JungleExplorer.Service;
using Android.Net;



namespace JungleExplorer
{
	[Activity (Label = "JungleExplorer", MainLauncher = true, Icon = "@drawable/icon", Theme="@style/MyTheme")]
	public class MainActivity : Android.Support.V7.App.AppCompatActivity
	{
		private SupportToolbar mToolbar;
		private MyActionBarDrawerToggle mDrawerToggle;
		private Android.Support.V4.Widget.DrawerLayout mDrawerLayout;
		private Android.Widget.ListView mLeftDrawer;

		public Android.Locations.Location location { get; set;}

		private Android.Widget.ArrayAdapter mLeftAdapter;

		private List<string> mLeftDataSet;

		private Android.Widget.FrameLayout mFragmentContainer;
		private SupportFragment mCurrentFragment;
		private FragmentListAnimals mFragmentListAnimals;
		private FragmentListScientists mFragmentListScience;
		private FragmentAddScientist mFragmentAddScience;
		private FragmentSettings mFragmentSetting;
		private FragmentAddAnimal fragmentAddAnimal;
		private Stack<SupportFragment> mStackFragments;
		IMenuItem menuItem;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
		//	MenuItem item = menu.findItem(R.id.addAction);
			// Set our view from the "main" layout resource
			this.SetContentView (Resource.Layout.Main);

			mToolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);

			//mToolbar.FindViewById<IMenuItem>(Resource.Id.action_sa
			mDrawerLayout = FindViewById<Android.Support.V4.Widget.DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<Android.Widget.ListView>(Resource.Id.left_drawer);
			mFragmentContainer = FindViewById<Android.Widget.FrameLayout>(Resource.Id.fragmentContainer);
			mFragmentListAnimals = new FragmentListAnimals();
			/*mFragmentListScience = new FragmentListScientists();
			mFragmentAddScience = new FragmentAddScientist();
			fragmentAddAnimal = new FragmentAddAnimal ();*/

			mStackFragments = new Stack<SupportFragment>();

		    var trans = SupportFragmentManager.BeginTransaction();
		/*	trans.Add(Resource.Id.fragmentContainer, mFragmentAddScience, "Fragment3");
			trans.Hide(mFragmentAddScience);

			trans.Add(Resource.Id.fragmentContainer, mFragmentListScience, "Fragment2");
			trans.Hide(mFragmentListScience);
			trans.Add(Resource.Id.fragmentContainer, mFragmentSetting, "FragmentSetting");
			trans.Hide (mFragmentSetting);


			trans.Add (Resource.Id.fragmentContainer, fragmentAddAnimal, "FragmentAnimal");*/
			mFragmentListScience = new FragmentListScientists ();

			trans.Add(Resource.Id.fragmentContainer, mFragmentListAnimals, "FragmentListAnimals");
			trans.Commit();

			mCurrentFragment = mFragmentListAnimals;

			mLeftDrawer.Tag = 0;

			SetSupportActionBar(mToolbar);
		
			mLeftDataSet = new List<string>();
			mLeftDataSet.Add ("Animals");
			mLeftDataSet.Add ("Scientists");
			mLeftAdapter = new Android.Widget.ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;


			mDrawerToggle = new MyActionBarDrawerToggle(
				this,							//Host Activity
				mDrawerLayout					//DrawerLayout
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowTitleEnabled(true);
			mDrawerToggle.SyncState();

			//This is the first the time the activity is ran
			SupportActionBar.SetTitle(Resource.String.animals);

			mLeftDrawer.ItemClick += MLeftDrawer_ItemClick;

			InitBackgroundLocationManager ();

 			bundle = Intent.Extras;
			if (bundle != null) {
				var id = bundle.GetInt ("id");
				if (id != null && id != 0) {
					ShowFragment (ViewEnumeration.VIEW_DETAIL_ANIMAL, DataManager.Instance.GetAnimal (id));
				} 

			}

		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnPause()
		{
			base.OnPause();
		}

		void MLeftDrawer_ItemClick (object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			if (e != null) {
				if (e.Position == 0) {
					SupportActionBar.SetTitle(Resource.String.animals);
					mDrawerLayout.CloseDrawer (mLeftDrawer);
					ShowFragment (ViewEnumeration.VIEW_ANIMAL);
				} else if (e.Position == 1) {
					SupportActionBar.SetTitle(Resource.String.scientists);
					mDrawerLayout.CloseDrawer (mLeftDrawer);
					ShowFragment (ViewEnumeration.VIEW_SCIENTIST);
				}
			}
		}
			
			
		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{

			case Android.Resource.Id.Home:
				//The hamburger icon was clicked which means the drawer toggle will handle the event
				//all we need to do is ensure the right drawer is closed so the don't overlap
				mDrawerToggle.OnOptionsItemSelected(item);
				return true;
			case Resource.Id.action_save:
				EnableButtonSave (false);
				if (mCurrentFragment is FragmentAddAnimal) {
					((FragmentAddAnimal)mCurrentFragment).SaveAnimal (location);
					mFragmentListAnimals.RecalculateList ();
					ShowFragment (ViewEnumeration.VIEW_ANIMAL);
				} else if (mCurrentFragment is FragmentAddScientist) {
					((FragmentAddScientist)mCurrentFragment).AddScientist ();
					mFragmentListScience.RecalculateList ();
					ShowFragment (ViewEnumeration.VIEW_SCIENTIST);
				}
				return true;
			case Resource.Id.action_share:
				ShowFragment (ViewEnumeration.VIEW_SETTINGS);
				return true;
			/*case Android.Resource.Id.:

				break;*/
			default:
				return base.OnOptionsItemSelected (item);
			}
		}
			
		public void ShowFragment (ViewEnumeration f, Java.Lang.Object extraData = null)
		{
			var trans = SupportFragmentManager.BeginTransaction();
			trans.SetCustomAnimations(Resource.Animation.slide_in, Resource.Animation.slide_out, Resource.Animation.slide_in, Resource.Animation.slide_out);
			SupportFragment fragment = null;
			switch (f) {
			case ViewEnumeration.VIEW_ANIMAL:
				if (mCurrentFragment is FragmentListAnimals) {
					return;
				} else if (mCurrentFragment is FragmentAddAnimal)
					trans.Remove (fragmentAddAnimal);
				else if (mCurrentFragment is FragmentListScientists) {
					trans.Remove (mFragmentListScience);
				} else if(mCurrentFragment is FragmentAddScientist){
					trans.Remove(mFragmentAddScience);
				}
					else{
						trans.Remove (mFragmentSetting);

					}
				mFragmentListAnimals = new FragmentListAnimals ();
				trans.Add (Resource.Id.fragmentContainer, mFragmentListAnimals, "FragmentListAnimals");

				fragment = mFragmentListAnimals;
				EnableButtonSave (false);
				mFragmentListAnimals.RecalculateList ();
				break;
			case ViewEnumeration.VIEW_DETAIL_ANIMAL:
				EnableButtonSave (true);
				fragmentAddAnimal = new FragmentAddAnimal ();
				trans.Add (Resource.Id.fragmentContainer, fragmentAddAnimal, "FragmentAnimal");
				fragment = fragmentAddAnimal;
				trans.Remove (mFragmentListAnimals);
				if (extraData != null) {
					fragmentAddAnimal.SetAnimal ((AnimalAndroid) extraData);
				}
				break;
			case ViewEnumeration.VIEW_DETAIL_SCIENTIST:
				EnableButtonSave (true);
				mFragmentAddScience = new FragmentAddScientist ();
				fragment = mFragmentAddScience;
				trans.Remove (mFragmentListScience);
				trans.Add (Resource.Id.fragmentContainer, mFragmentAddScience, "FragmentAddScientist");
				mFragmentAddScience.ResetFields ();
				if (extraData != null) {
					mFragmentAddScience.scientist = ((ScientistAndroid) extraData);
				}
				break;
			case ViewEnumeration.VIEW_SCIENTIST:
				EnableButtonSave (false);
				if (mCurrentFragment is FragmentListScientists) {
					return;
				} else if (mCurrentFragment is FragmentAddAnimal) {
					trans.Remove (fragmentAddAnimal);
				} else if (mCurrentFragment is FragmentListAnimals) {
					trans.Remove (mFragmentListAnimals);
				} else if (mCurrentFragment is FragmentAddScientist) {
					trans.Remove (mFragmentAddScience);
				} else {
					trans.Remove (mFragmentSetting);
				}
				trans.Add (Resource.Id.fragmentContainer, mFragmentListScience, "FragmentListScientist");
				fragment = mFragmentListScience;
			//	fragment.OnCreateView (this.LayoutInflater, this.LayoutInflater, null);
				mFragmentListScience.RecalculateList ();
				break;
			case ViewEnumeration.VIEW_SETTINGS:
				if (mCurrentFragment is FragmentSettings) {
					return;
				} else if (mCurrentFragment is FragmentAddAnimal) {
					trans.Remove (fragmentAddAnimal);
				} else if (mCurrentFragment is FragmentListAnimals) {
					trans.Remove (mFragmentListAnimals);
				} else if (mCurrentFragment is FragmentAddScientist) {
					trans.Remove (mFragmentAddScience);
				} else {
					trans.Remove (mFragmentListScience);
				}
				mFragmentSetting = new FragmentSettings ();
				trans.Add (Resource.Id.fragmentContainer, mFragmentSetting, "FragmentListScientist");
				EnableButtonSave (false);
				SupportActionBar.SetTitle(Resource.String.settings);
				fragment = mFragmentSetting;
				break;
			}
				
			if(fragment != null)
				if(fragment.View != null)
				fragment.View.BringToFront();
			if(mCurrentFragment.View != null)
				mCurrentFragment.View.BringToFront();

			trans.Hide(mCurrentFragment);
			trans.Show(fragment);
			trans.AddToBackStack(null);
			mStackFragments.Push(mCurrentFragment);
			//trans.Remove (mCurrentFragment);
			trans.Commit();
			FragmentManager.ExecutePendingTransactions ();
			//mFragmentManager.executePendingTransactions();

			mCurrentFragment = fragment;
		}
			
		public override void OnBackPressed ()
		{
			
			if (SupportFragmentManager.BackStackEntryCount > 0)
			{
				SupportFragmentManager.PopBackStack();
				if(mStackFragments.Count > 0)
					mCurrentFragment = mStackFragments.Pop();
			}

			else
			{
				base.OnBackPressed();
			}				
		}


		public override Boolean OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.action_menu, menu);
			menuItem = menu.FindItem (Resource.Id.action_save);
			EnableButtonSave (false);
			/*var overflow_item = menu.FindItem (Resource.Id.overflowMenuItem);
			 *		menuItem = menu.FindItem (Resource.Id.action_save);*/
		/*	var shareItem = menu.FindItem(Resource.Id.action_share);
			var test = MenuItemCompat.GetActionProvider(shareItem);
			actionProvider = test.JavaCast<Android.Support.V7.Widget.ShareActionProvider>();
			var intent = new Intent(Intent.ActionSend);
			intent.SetType("text/plain");
			intent.PutExtra(Intent.ExtraText, "ActionBarCompat is Awesome! Support Lib v7 #Xamarin");
			actionProvider.SetShareIntent(intent);*/


	//		Android.Support.V7.Widget.ShareActionProvider shareProv = (Android.Support.V7.Widget.ShareActionProvider)mnuShare.ActionProvider;
	//		shareProv.SetShareHistoryFileName (Android.Support.V7.Widget.ShareActionProvider.DefaultShareHistoryFileName);
        	//shareProv.ShareHistoryFileName();
			//Android.Support.V7.Widget.ShareActionProvider overflow_provider = null;
			//MenuItemCompat.SetActionProvider(overflow_item, overflow_provider);
			//overflow_provider.SetShareIntent (action_save ());*/
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnSaveInstanceState (Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}

			else
			{
				outState.PutString("DrawerState", "Closed");
			}

			base.OnSaveInstanceState (outState);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}

		public void EnableButtonSave(bool b){
			if (menuItem != null) {
				menuItem.SetVisible (b);
				menuItem.SetEnabled (b);
			}
		}

		private void InitBackgroundLocationManager ()
		{
			// This event fires when the ServiceConnection lets the client (our App class) know that
			// the Service is connected. We use this event to start updating the UI with location
			// updates from the Service
			App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
				Log.Debug ("Main activity", "ServiceConnected Event Raised");
				// notifies us of location changes from the system
				App.Current.LocationService.LocationChanged += HandleLocationChanged;
			};
			var preferences = PreferenceManager.GetDefaultSharedPreferences(this);
			bool myValue= preferences.GetBoolean("service", false);
			if (myValue) {
				App.Current.StartLocationService ();	
			} 
		}

		#region Android Location Service methods

		///<summary>
		/// Updates UI with location data
		/// </summary>
		public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		{
			if (e != null) {
				location = e.Location;
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (this);
				ISharedPreferencesEditor editor = prefs.Edit ();
				editor.PutFloat ("Altitude", Convert.ToSingle(location.Altitude));
				editor.PutFloat ("Latitude", Convert.ToSingle(location.Latitude));
			}
		}



		#endregion

		Intent CreateIntent ()
		{
			var sendPictureIntent = new Intent (Intent.ActionSend);
			sendPictureIntent.SetType ("text/*");            
			string temp = ((IInformation)mCurrentFragment).GetInformation ();
			sendPictureIntent.PutExtra (Intent.ExtraStream, temp);

			return sendPictureIntent;
		}

		public bool HasConnectionToInternet(){
			ConnectivityManager connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
			if (isOnline) {
				NetworkInfo wifiInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Wifi);
				if (wifiInfo.IsConnected) {
					return true;
				} else {
					NetworkInfo mobileInfo = connectivityManager.GetNetworkInfo(ConnectivityType.Mobile);
					if (mobileInfo.IsRoaming && mobileInfo.IsConnected) {
						return true;
					}
				}
			}
			return false;
		}

		public void ShowLocationDialog ()
		{
			var dialog = new CustomDialog();
			dialog.Show(SupportFragmentManager, "dialog");
		}
	}
}


