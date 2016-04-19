using Android.Widget;
using System.Collections.Generic;
using Model.Model;
using Android.OS;
using Android.Views;
using FFImageLoading;
using JungleExplorer.Service;
using System;


namespace JungleExplorer
{
	public class FragmentListAnimals : Android.Support.V4.App.Fragment, IInformation
	{
		public ImageButton Button;
		private ListView _lista;
		AnimalAdapter adapter;
		private Button buttonFilterAnimals;
		private List<Animal> animalList;
		private List<Animal> animalFilter;
		bool filter = false;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(Resource.Layout.FragmentListAnimals, container, false);


				Button = view.FindViewById<ImageButton>(Resource.Id.addButton);

				Button.Click += ChangeFragment;
			_lista = view.FindViewById<ListView> (Resource.Id.listaAnimales);
			//lista.Adapter = new AnimalAdapter (this.Context, GetAllAnimals());
			animalList = GetAllAnimals();
			CalculateFilter ();
			buttonFilterAnimals = view.FindViewById<Button> (Resource.Id.buttonFilter);
			adapter = new AnimalAdapter (this.Context, animalList);
			_lista.Adapter = adapter;
			_lista.ScrollStateChanged += (object sender, Android.Widget.AbsListView.ScrollStateChangedEventArgs scrollArgs) => {
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
			_lista.ItemClick += lista_Click;
			buttonFilterAnimals.Click += filter_animal;
			return view;
		}

		void lista_Click (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (e != null) {
				var animal = adapter.data[e.Position];
				((MainActivity)Activity).ShowFragment (Model.ViewEnumeration.VIEW_DETAIL_ANIMAL, animal);
			}
		}

	    private void ChangeFragment (object sender, EventArgs e)
		{
			if (e != null) {
				((MainActivity)Activity).ShowFragment (Model.ViewEnumeration.VIEW_DETAIL_ANIMAL);
			}

		}

		public List<Model.Model.Animal> GetAllAnimals ()
		{
			return DataManager.Instance.GetAllAnimals();
		}
			
		public void RecalculateList ()
		{
			if (Context != null) {
				adapter = new AnimalAdapter (this.Context, GetAllAnimals ());
				_lista.Adapter = adapter;
			}
		}

		#region IInformation implementation

		public string GetInformation ()
		{
			return "I'm using JungleExplorer :D";
		}

		#endregion

		void filter_animal (object sender, EventArgs e)
		{
			if (e != null) {
				if (animalFilter == null || animalFilter.Count > 0) {
					CalculateFilter ();
				}
				if (animalFilter != null && animalFilter.Count > 0) {
					filter = !filter;
					if (filter) {
						adapter = new AnimalAdapter (this.Context, animalFilter);
						_lista.Adapter = adapter;
					} else {
						adapter = new AnimalAdapter (this.Context, animalList);
						_lista.Adapter = adapter;
					}
				}
			}
		}

		void CalculateFilter ()
		{
			animalFilter = new List<Animal> ();
			foreach (var a in animalList) {
				var l = ((MainActivity)Activity).location;
				if (l == null) {
					Toast.MakeText (this.Context, "There is no location yet", ToastLength.Long).Show ();
					return;
				} else {
					float distance = CalculateDistance (a, l);
					if (distance < 10) {
						animalFilter.Add (a);
					}
				}
			}
		}

		float CalculateDistance (Animal a,  Android.Locations.Location l)
		{
			double altitude = Math.Pow(a.altitude - l.Altitude,2);
			double latitude = Math.Pow (a.latitude - l.Latitude, 2);
			return Convert.ToSingle(Math.Sqrt(altitude+latitude));
		}
	}
}