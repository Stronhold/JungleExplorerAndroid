
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
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Java.IO;
using Android.Provider;
using Model;
using Android.Graphics;
using JungleExplorer.Service;
using Model.Model;
using FFImageLoading;
using FFImageLoading.Views;
using Android.Locations;
using System.Threading.Tasks;
using Android.Preferences;


namespace JungleExplorer
{
	public class FragmentAddAnimal : Android.Support.V4.App.Fragment, IInformation
	{
		public static Bitmap bitmap;

		public ImageViewAsync image;
		public EditText name, desc;
		public static Java.IO.File _file;
		public static Java.IO.File _dir;


		List<Scientist> listaScient;
		Scientist s;
		AnimalAndroid animalAndroid;
		ListView scientistForAnimal;
		ScientistListAdapter listAdapter;


		string uri = "";

		int id = 0;

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			// Create your fragment here

		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.FragmentAddAnimal, container, false);
			image = view.FindViewById<ImageViewAsync>(Resource.Id.image);
			CreateDirectoryForPictures ();
			image.Clickable = true;
			image.Click += TakeAPicture;
			name = view.FindViewById<EditText> (Resource.Id.tEditName);
			desc = view.FindViewById<EditText> (Resource.Id.tEditDesc);

			var listaNombres = DataManager.Instance.GetScientistNames (id);
			var adapter = new ArrayAdapter<String> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, listaNombres);
			scientistForAnimal = view.FindViewById<ListView> (Resource.Id.scientistsForAnimal);
			//listaScient = DataManager.Instance.GetScientist (0);
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			if (animalAndroid != null) {
				GetAnimalData ();
				listAdapter = new ScientistListAdapter (this.Context, animalAndroid.id);
			} else {
				listAdapter = new ScientistListAdapter (this.Context, 0);
			}
			scientistForAnimal.Adapter = listAdapter;
			scientistForAnimal.ItemClick += lista_Click;
			return view;
		}



		void BAddScientist_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			if (e != null) {
				s = listaScient[e.Position];
			}
			
		}
			

		private void CreateDirectoryForPictures ()
		{
			_dir = new Java.IO.File (
				Android.OS.Environment.GetExternalStoragePublicDirectory (
					Android.OS.Environment.DirectoryPictures), "JungleExplorer");
			if (!_dir.Exists ())
			{
				_dir.Mkdirs( );
			}
		}

		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);

			_file = new File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));

			intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
			Activity.SetResult (Result.Ok, intent);
			StartActivityForResult(intent, 0);
		}
			
		public override void OnActivityResult (int requestCode, int resultCode, Intent data)
		{
			base.OnActivityResult (requestCode, resultCode, data);

			// Make it available in the gallery

			Intent mediaScanIntent = new Intent (Intent.ActionMediaScannerScanFile);

			Uri contentUri = Uri.FromFile (_file);
			mediaScanIntent.SetData (contentUri);
			Activity.SendBroadcast (mediaScanIntent);

			uri = _file.AbsolutePath;
			if(!string.IsNullOrEmpty(uri))
				ImageService.LoadFile(uri).DownSample(100, 1000).Into(image);
			GC.Collect ();
		}

		public void SaveAnimal (Android.Locations.Location l){
			
			Animal a = new Animal (name.Text, desc.Text, uri);
			if (animalAndroid != null) {
				a.ID = animalAndroid.id;
			}

			if (l != null) {
				a.latitude = l.Latitude;
				a.altitude = l.Altitude;
			} else {
				ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences (Activity);

				var altitude = prefs.GetFloat ("Altitude", float.MaxValue);
				var latitude = prefs.GetFloat ("Latitude", float.MaxValue);
				a.latitude = latitude;
				a.altitude = altitude;
			}
			id = 0;
			var scientists = new List<ScientistAdded>();
			if (a.Subjects == null) {
				a.Subjects = new List<Scientist> ();
			}
			foreach (var scien in listAdapter.data) {
				if (scien.added) {
					var scientist = DataManager.Instance.GetOnlyOneScientistForId (scien.id);
					a.Subjects.Add (scientist);
				}
			}
			DataManager.Instance.AddAnimal (a);
		}

		public void SetAnimal(AnimalAndroid a){
			animalAndroid = a;
		}

		public void GetAnimalData ()
		{
			uri = animalAndroid.Uri;
			id = animalAndroid.id;
			name.Text = animalAndroid.name;
			desc.Text = animalAndroid.desc;
			ImageService.LoadFile(animalAndroid.Uri).DownSample(100,100).Into(image);
			var listaNombres = DataManager.Instance.GetScientistNames (id);
			var adapter = new ArrayAdapter<String> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, listaNombres);
			listaScient = DataManager.Instance.GetScientist (animalAndroid.id);
			var listaNames = DataManager.Instance.GetScientistNamesFromAnimal (animalAndroid.id);
			s = null;
		}

		public void ResetFields ()
		{
			if (FieldsAreNull ()) {
				InitializeFields ();
			}
			name.Text = "";
			desc.Text = "";
			image.SetImageResource (Resource.Drawable.placeholder);
			s = null;
			//ImageService.LoadFile(animalAndroid.Uri).DownSample(image.Width, image.Height).Into(image);

		}

	


		async void GetLocation (object sender, EventArgs e)
		{
		}

		#region IInformation implementation

		public string GetInformation ()
		{
			return "I'm using JungleExplorer!, I'm looking at a animal called: " + name.Text ;
		}

		#endregion

		void InitializeFields ()
		{
			/*var inflater = Activity.LayoutInflater;
			View view = inflater.Inflate(Resource.Layout.FragmentAddAnimal, container, false);
			image = view.FindViewById<ImageViewAsync>(Resource.Id.image);
			CreateDirectoryForPictures ();
			image.Clickable = true;
			image.Click += TakeAPicture;
			name = view.FindViewById<EditText> (Resource.Id.tEditName);
			desc = view.FindViewById<EditText> (Resource.Id.tEditDesc);
			bAddScientist = view.FindViewById<Spinner> (Resource.Id.buttonAddScientist);
			bSeeScientists = view.FindViewById<Spinner> (Resource.Id.buttonSeeScientist);
			bLocation = view.FindViewById<Button> (Resource.Id.buttonGPS);
			var listaNombres = DataManager.Instance.GetScientistNames (id);
			var adapter = new ArrayAdapter<String> (this.Activity, Resource.Layout.support_simple_spinner_dropdown_item, listaNombres);
			bAddScientist.Adapter = adapter;
			bAddScientist.ItemSelected += BAddScientist_ItemSelected;
			listaScient = DataManager.Instance.GetScientist (0);
			// Use this to return your custom view for this Fragment
			// return inflater.Inflate(Resource.Layout.YourFragment, container, false);
			bLocation.Click += GetLocation;*/

		}

		bool FieldsAreNull ()
		{
			return false;
		}

		void lista_Click (object sender, AdapterView.ItemClickEventArgs e)
		{
			if (e != null) {
				listAdapter.data[e.Position].added = !listAdapter.data[e.Position].added;
				if (listAdapter.data [e.Position].added) {
					scientistForAnimal.GetChildAt (e.Position).SetBackgroundColor (Color.Green);
				} else {
					scientistForAnimal.GetChildAt (e.Position).SetBackgroundColor (Color.Red);
				}
				//s.added = !s.added;
			}
		}
	}
}

