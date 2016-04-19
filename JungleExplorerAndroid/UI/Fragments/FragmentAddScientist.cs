
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


namespace JungleExplorer
{
	public class FragmentAddScientist : Android.Support.V4.App.Fragment, IInformation
	{
		public static Bitmap bitmap;

		public ImageViewAsync image;
		public EditText name, desc;
		public static Java.IO.File _file;
		public static Java.IO.File _dir;
		public int id = 0;
		public string uri = "";
		public ScientistAndroid scientist{ get; set; }

		public override void OnCreate (Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View view = inflater.Inflate(Resource.Layout.FragmentAddScientist, container, false);

			image = view.FindViewById<ImageViewAsync>(Resource.Id.imageScientist);
			name = view.FindViewById<EditText> (Resource.Id.tEditNameScientist);
			desc = view.FindViewById<EditText> (Resource.Id.tEditDescScientist);
			CreateDirectoryForPictures ();
			image.Clickable = true;
			image.Click += TakeAPicture;
			if (scientist != null) {
				name.Text = scientist.Name;
				desc.Text = scientist.Description;
				if(!string.IsNullOrEmpty(scientist.Uri ))
					ImageService.LoadFile(scientist.Uri).DownSample(100, 100).Into(image);
			}
			return view;
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
			//	Activity.SendBroadcast (data);


			// Display in ImageView. We will resize the bitmap to fit the display.
			// Loading the full sized image will consume to much memory
			// and cause the application to crash.

			int height = Resources.DisplayMetrics.HeightPixels;
			int width = image.Height ;
		/*	Bitmap bitmapToDisplay = BitmapHelpers.LoadAndResizeBitmap(_file.AbsolutePath, height, width);

			if (bitmap != null) {
				image.SetImageBitmap (bitmapToDisplay);
				bitmapToDisplay = null;
			}*/

			ImageService.LoadFile(_file.AbsolutePath).DownSample(100, 100).Into(image);
			uri = _file.AbsolutePath;
			// Dispose of the Java side bitmap.
			GC.Collect();
		}

		public void AddScientist(){
			Scientist s = new Scientist (name.Text, desc.Text, uri);
			if (scientist != null) {
				s.Id = scientist.Id;
			}
			DataManager.Instance.AddScientist (s);
		}

		public void ResetFields ()
		{
			if(name != null)
				name.Text = "";
			if(desc != null)
				desc.Text = "";
			if(image != null)
				image.SetImageResource (Resource.Drawable.placeholder);
		}

		#region IInformation implementation

		public string GetInformation ()
		{
			return "I'm using JungleExplorer. I'm looking at an animal called: " + name.Text;
		}

		#endregion
	}
}

