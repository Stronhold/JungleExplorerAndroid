using System;

using Android.App;
using Android.Util;
using Android.Content;
using Android.OS;
using Android.Locations;
using JungleExplorer;
using Model.Model;
using JungleExplorer.Service;
using Android.Appwidget;
using Android.Widget;
using Android.Preferences;

namespace Location.Droid.Services
{
	[Service]
	public class LocationService : Service, ILocationListener
	{
		public event EventHandler<LocationChangedEventArgs> LocationChanged = delegate { };
		public event EventHandler<ProviderDisabledEventArgs> ProviderDisabled = delegate { };
		public event EventHandler<ProviderEnabledEventArgs> ProviderEnabled = delegate { };
		public event EventHandler<StatusChangedEventArgs> StatusChanged = delegate { };
		public static int id = 0;

		public LocationService() 
		{
		}

		// Set our location manager as the system location service
		protected LocationManager LocMgr = Android.App.Application.Context.GetSystemService ("location") as LocationManager;

		readonly string logTag = "LocationService";
		IBinder binder;

		public override void OnCreate ()
		{
			base.OnCreate ();
			Log.Debug (logTag, "OnCreate called in the Location Service");
		}

		// This gets called when StartService is called in our App class
		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			Log.Debug (logTag, "LocationService started");

			return StartCommandResult.Sticky;
		}

		// This gets called once, the first time any client bind to the Service
		// and returns an instance of the LocationServiceBinder. All future clients will
		// reuse the same instance of the binder
		public override IBinder OnBind (Intent intent)
		{
			Log.Debug (logTag, "Client now bound to service");

			binder = new LocationServiceBinder (this);
			return binder;
		}

		// Handle location updates from the location manager
		public void StartLocationUpdates () 
		{
			//we can set different location criteria based on requirements for our app -
			//for example, we might want to preserve power, or get extreme accuracy
			var locationCriteria = new Criteria();
			
			locationCriteria.Accuracy = Accuracy.NoRequirement;
			locationCriteria.PowerRequirement = Power.NoRequirement;

			// get provider: GPS, Network, etc.
			var locationProvider = LocMgr.GetBestProvider(locationCriteria, true);
			Log.Debug (logTag, string.Format ("You are about to get location updates via {0}", locationProvider));

			// Get an initial fix on location
			LocMgr.RequestLocationUpdates(locationProvider, 60000, 0, this);

			Log.Debug (logTag, "Now sending location updates");
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();
			Log.Debug (logTag, "Service has been terminated");
		}

		#region ILocationListener implementation
		// ILocationListener is a way for the Service to subscribe for updates
		// from the System location Service

		public void OnLocationChanged (Android.Locations.Location location)
		{
			this.LocationChanged (this, new LocationChangedEventArgs (location));
			var nMgr = (NotificationManager)GetSystemService (NotificationService);
			var animal = GetNearestAnimal (location);
			var notification = new Notification (Resource.Drawable.Icon, "Message from JungleExplorer");
			var intent = new Intent (this, typeof(MainActivity));
			Bundle b = new Bundle ();
			b.PutInt ("id", animal.ID);
			intent.PutExtras (b);
		//	PendingIntent contentIntent = PendingIntent.GetActivity(Context, animal.ID, intent, 0);
			id = animal.ID;
			var prefs = PreferenceManager.GetDefaultSharedPreferences(this); 
			var send = prefs.GetBoolean ("notifications", true);
			if (send) {
				var pendingIntent = PendingIntent.GetActivity (this, animal.ID, intent, 0);
				notification.SetLatestEventInfo (this, "Jungle Explorer Notification", "Animal near to you!, id: " + animal.ID, pendingIntent);
				nMgr.Notify (0, notification);
			}
			//AppWidgetManager a = AppWidgetManager.GetInstance (this);
			SendInfoToWidget (animal.ID);
			//a.UpdateAppWidget ();
		}

		public void OnProviderDisabled (string provider)
		{
			this.ProviderDisabled (this, new ProviderDisabledEventArgs (provider));
		}

		public void OnProviderEnabled (string provider)
		{
			this.ProviderEnabled (this, new ProviderEnabledEventArgs (provider));
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			this.StatusChanged (this, new StatusChangedEventArgs (provider, status, extras));
		} 

		#endregion

		Animal GetNearestAnimal (Android.Locations.Location location)
		{
			var animals = DataManager.Instance.GetAllAnimals ();
			var animal = new Animal ();
			animal.ID = 0;
			float distance = float.MaxValue;
			bool firstTime = true;
			foreach (var a in animals) {
				float d = GetSquareDistance (a, location);
				if (d < distance) {
					animal = a;
					distance = d;
				}
				if (firstTime) {
					animal = a;
					distance = d;
					firstTime = false;
				}
			}
			return animal;
		}

		float GetSquareDistance (Animal a, Android.Locations.Location l)
		{
			var latitude = Math.Pow (l.Latitude - a.latitude, 2);
			var altitude = Math.Pow (l.Altitude - a.altitude, 2);
			return Convert.ToSingle(Math.Sqrt (latitude+altitude));
		}

		void SendInfoToWidget (int id)
		{
			// Build an update that holds the updated widget contents
			var updateViews = new RemoteViews (this.PackageName, Resource.Layout.widget_message);

			updateViews.SetTextViewText (Resource.Id.message, "Your nearest animal is: " + id );
			//updateViews.SetTextViewText (Resource.Id.creator, entry.Creator);

			// When user clicks on widget, launch to Wiktionary definition page

			Intent defineIntent = new Intent (this, typeof(MainActivity));
			Bundle b = new Bundle ();
			b.PutInt ("id", id);
			defineIntent.PutExtras (b);
			PendingIntent pendingIntent = PendingIntent.GetActivity (this, id, defineIntent, 0);
			updateViews.SetOnClickPendingIntent (Resource.Id.widget, pendingIntent);

			ComponentName thisWidget = new ComponentName (this, Java.Lang.Class.FromType (typeof (ExplorerWidgetAppWidgetProvider)).Name);
			AppWidgetManager manager = AppWidgetManager.GetInstance (this);
			manager.UpdateAppWidget (thisWidget, updateViews);
		}
	}
}

