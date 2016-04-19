using System;
using Android.Content;
using Android.Appwidget;
using Location.Droid.Services;
using Android.App;

namespace JungleExplorer
{
	[BroadcastReceiver (Label = "@string/widget_name")]
	[IntentFilter (new string [] { "android.appwidget.action.APPWIDGET_UPDATE" })]
	[MetaData ("android.appwidget.provider", Resource = "@xml/widget_word")]
	public class ExplorerWidgetAppWidgetProvider:AppWidgetProvider
	{
		public override void OnUpdate (Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
		{
			// To prevent any ANR timeouts, we perform the update in a service
			context.StartService (new Intent (context, typeof (LocationService)));
		}

		public override void OnReceive (Context context, Intent intent)
		{
			base.OnReceive (context, intent);
			var bundle = intent.Extras;
			//var id = bundle.GetInt ("id");
		}
	}
}

