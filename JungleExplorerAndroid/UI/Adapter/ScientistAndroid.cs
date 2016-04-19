using System;
using Android.Widget;
using Model;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Model.Model;
using Java.IO;

namespace JungleExplorer
{
	public class ScientistAndroid : Java.Lang.Object
	{

		public int Id{ get; set;}
		public string Name{ get; set;}
		public string Description { get; set;}
		public string Uri { get; set;}


		public ScientistAndroid(Scientist s){
			Id = s.Id;
			Name = s.Name;
			Description = s.Description;
			Uri = s.Uri;
		}
	}

}
