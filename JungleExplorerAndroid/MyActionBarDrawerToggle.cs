using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace JungleExplorer
{
	public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
	{
		private AppCompatActivity mHostActivity;


		public MyActionBarDrawerToggle (AppCompatActivity host, DrawerLayout drawerLayout) 
			: base(host, drawerLayout,0,0)
		{
			mHostActivity = host;

		}

		public override void OnDrawerOpened (Android.Views.View drawerView)
		{	
			int drawerType = (int)drawerView.Tag;

			if (drawerType == 0)
			{
				base.OnDrawerOpened (drawerView);
			}
		}

		public override void OnDrawerClosed (Android.Views.View drawerView)
		{
			int drawerType = (int)drawerView.Tag;

			if (drawerType == 0)
			{
				base.OnDrawerClosed (drawerView);
			}				
		}

		public override void OnDrawerSlide (Android.Views.View drawerView, float slideOffset)
		{
			int drawerType = (int)drawerView.Tag;

			if (drawerType == 0)
			{
				base.OnDrawerSlide (drawerView, slideOffset);
			}
		}
	}
}

