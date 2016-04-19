using System;
using Android.Widget;
using Android.Views;
using System.Collections.Generic;
using JungleExplorer.Service;
using Android.Graphics;
using Android.Content;


namespace JungleExplorer
{
	public class ScientistListAdapter:BaseAdapter
	{
		private const String TAG = "ScientistAdapter";
		public List<ScientistAdded> data { get; set; }
		private LayoutInflater inflater = null;

		public ScientistListAdapter(Context c, int animalID)
		{
			data = new List<ScientistAdded> ();
			data = DataManager.Instance.GetScientistForAnimal (animalID);		
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

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			View row;
			row = inflater.Inflate (Resource.Layout.ListScientistForAnimal, parent, false);
			var s = data [position];

			var eName = row.FindViewById<TextView> (Resource.Id.textView_ScientistID);
			var eDesc = row.FindViewById<TextView> (Resource.Id.textView_Scientist_name);
			eName.SetText (s.id + "", TextView.BufferType.Normal);
			eDesc.SetText (s.name, TextView.BufferType.Normal);
			if (s.added) {
				row.SetBackgroundColor (Color.Green);
			} else {
				row.SetBackgroundColor (Color.Red);
			}
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

