using System;
using Android.Widget;
using Model;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Model.Model;
using Java.IO;
using System.Threading.Tasks;
using System.Threading;
using Android.Graphics;
using FFImageLoading;
using FFImageLoading.Views;

namespace JungleExplorer
{
	public class AnimalAdapter : BaseAdapter
	{

		private const String TAG = "AnimalAdapter";
		public List<AnimalAndroid> data { get; set; }
		private LayoutInflater inflater = null;

		public AnimalAdapter (Context c, List<Animal> d)
		{
			data = new List<AnimalAndroid> ();
			foreach (var a in d) {
				data.Add (new AnimalAndroid (a));
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
			AnimalAndroid a = data [position];
			var image = row.FindViewById<ImageViewAsync> (Resource.Id.imageAnimalList);
			if(!string.IsNullOrEmpty(a.Uri))
				ImageService.LoadFile(a.Uri).DownSample(100,100).Into(image);

			var eName = row.FindViewById<TextView> (Resource.Id.textView_Name);
			var eDesc = row.FindViewById<TextView> (Resource.Id.textView_Description);
			eName.SetText (a.name, TextView.BufferType.Normal);
			eDesc.SetText (a.desc, TextView.BufferType.Normal);

			return row;
		}


		public override int Count {
			get {
				return data.Count;
			}
		}
		#endregion

	}
}

