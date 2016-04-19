using System;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Util;

using Location.Droid.Services;

/// <summary>
/// Singleton class for Application wide objects. 
/// </summary>
namespace Location.Droid
{
	public class App
	{
		// events
		public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate {};
		
		// declarations
		protected readonly string logTag = "App";
		protected LocationServiceConnection locationServiceConnection;
		
		// properties
		
		public static App Current
		{
			get { return current; }
		} private static App current;
		
		public LocationService LocationService
		{
			get {
				if (this.locationServiceConnection.Binder == null)
					throw new Exception ("Service not bound yet");
				// note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
				return this.locationServiceConnection.Binder.Service;
			}
		}

		#region Application context

		static App ()
		{
			current = new App();
		}
		protected App () 
		{

		}

		public void StartLocationService()
		{
			new Task ( () => { 

				// start our main service
				Log.Debug (logTag, "Calling StartService");
				Android.App.Application.Context.StartService (new Intent (Android.App.Application.Context, typeof(LocationService)));

				// create a new service connection so we can get a binder to the service
				this.locationServiceConnection = new LocationServiceConnection (null);

				// this event will fire when the Service connectin in the OnServiceConnected call 
				this.locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {

					Log.Debug (logTag, "Service Connected");
					// we will use this event to notify MainActivity when to start updating the UI
					this.LocationServiceConnected ( this, e );

				};

				// bind our service (Android goes and finds the running service by type, and puts a reference
				// on the binder to that service)
				// The Intent tells the OS where to find our Service (the Context) and the Type of Service
				// we're looking for (LocationService)
				Intent locationServiceIntent = new Intent (Android.App.Application.Context, typeof(LocationService));
				Log.Debug (logTag, "Calling service binding");

				// Finally, we can bind to the Service using our Intent and the ServiceConnection we
				// created in a previous step.
				Android.App.Application.Context.BindService (locationServiceIntent, locationServiceConnection, Bind.AutoCreate);

			} ).Start ();
		}

		public void StopLocationService ()
		{
			// Check for nulls in case StartLocationService task has not yet completed.
			Log.Debug("App", "StopLocationService");

			// Unbind from the LocationService; otherwise, StopSelf (below) will not work:
			if (locationServiceConnection != null)
			{ 
				Log.Debug("App", "Unbinding from LocationService");
				Android.App.Application.Context.UnbindService(locationServiceConnection);
			}

			// Stop the LocationService:
			if (Current.LocationService != null)
			{ 
				Log.Debug("App", "Stopping the LocationService");
				Current.LocationService.StopSelf();
			}
		}


	
		#endregion

	}
}


