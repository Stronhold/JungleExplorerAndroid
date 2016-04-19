using System;
using Android.Widget;
using Model;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Model.Model;
using Java.IO;
using System.Threading;
using System.Threading.Tasks;
using FFImageLoading.Views;
using FFImageLoading;

namespace JungleExplorer
{
	public class ScientistAdapter:BaseAdapter
	{
		private const String TAG = "ScientistAdapter";
		public List<ScientistAndroid> data { get; set; }
		private LayoutInflater inflater = null;

		public ScientistAdapter (Context c, List<Scientist> d)
		{
			data = new List<ScientistAndroid> ();
			foreach (var a in d) {
				data.Add (new ScientistAndroid (a));
			}
			inflater = LayoutInflater.From(c);
		}

		#region implemented abstract members of BaseAdapter


		public override Java.Lang.Object GetItem (int position)
		{
			return data[position];
		}


		public override long GetItemId (int position)
		{
			return position;
		}


		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View row;
			row = inflater.Inflate (Resource.Layout.CustomListView, parent, false);
			ScientistAndroid a = data [position];
			var image = row.FindViewById<ImageViewAsync> (Resource.Id.imageAnimalList);
			if(!string.IsNullOrEmpty(a.Uri))
				ImageService.LoadFile(a.Uri).DownSample(100,100).Into(image);
			var eName = row.FindViewById<TextView> (Resource.Id.textView_Name);
			var eDesc = row.FindViewById<TextView> (Resource.Id.textView_Description);
			eName.SetText (a.Name, TextView.BufferType.Normal);
			eDesc.SetText (a.Description, TextView.BufferType.Normal);

			return row;
		}


		public override int Count {
			get {
				return data.Count;
			}
		}


		#endregion

		private async Task ResizeImageAsync (ImageView image, string path, CancellationTokenSource ct)
		{
			var options = await BitmapHelpers.GetBitmapOptionsOfImageAsync(path);
			var bitmapToDisplay = await BitmapHelpers.LoadScaledDownBitmapForDisplayAsync (path, options, image.Height, image.Width);

			//Free memory
			if (!ct.IsCancellationRequested) {
				if (bitmapToDisplay != null) {
					image.SetImageBitmap (bitmapToDisplay);
				}
			}
		}
	}
}
