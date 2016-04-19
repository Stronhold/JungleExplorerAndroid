
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using JungleExplorer.Service;
using FFImageLoading;


namespace JungleExplorer
{
	public class FragmentListScientists : Android.Support.V4.App.Fragment, IInformation
	{
		public ImageButton button;
		private ListView lista;
		ScientistAdapter adapter;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}


		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.FragmentListScientists, container, false);

			button = view.FindViewById<ImageButton>(Resource.Id.addButtonScientist);

			button.Click += ChangeFragment;
			lista = view.FindViewById<ListView> (Resource.Id.listaScientist);
			//lista.Adapter = new AnimalAdapter (this.Context, GetAllAnimals());
			lista.Adapter = 
				adapter = new ScientistAdapter (this.Context, GetAllScientists());
			lista.Adapter = adapter;
			lista.ScrollStateChanged += (object sender, Android.Widget.AbsListView.ScrollStateChangedEventArgs scrollArgs) => {
				switch (scrollArgs.ScrollState) {
				case ScrollState.Fling:
					ImageService.SetPauseWork (true); // all image loading requests will be silently canceled
					break;
				case ScrollState.Idle:
					ImageService.SetPauseWork (false); // loading requests are allowed again

					// Here you should have your custom method that forces redrawing visible list items
					// ();
					break;
				}
			};
			lista.ItemClick += lista_Click;
			return view;
		}

		void lista_Click (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (e != null) {
				var scientist = adapter.data[e.Position];
				((MainActivity)Activity).ShowFragment (Model.ViewEnumeration.VIEW_DETAIL_SCIENTIST, scientist);
			}
		}

		List<Model.Model.Scientist> GetAllScientists ()
		{
			return DataManager.Instance.GetAllScientist();
		}

		void ChangeFragment (object sender, EventArgs e)
		{
			if (e != null) {
				((MainActivity)Activity).ShowFragment (Model.ViewEnumeration.VIEW_DETAIL_SCIENTIST);
			}

		}

		public void RecalculateList ()
		{
			if (Context != null) {
				adapter = new ScientistAdapter (this.Context, GetAllScientists ());
				lista.Adapter = adapter;
			}
		}


		#region IInformation implementation
		public string GetInformation ()
		{
			return "I'm using JungleExplorer :D";
		}
		#endregion
	}
}

