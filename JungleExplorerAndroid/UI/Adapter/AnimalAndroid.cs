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
	public class AnimalAndroid: Java.Lang.Object
	{
		public int id { get; set; }
		public string name{ get; set; }
		public string desc{get;set;}
		public string Uri { get; set; }
		public List<Scientist> scientists { get; set;}

		public AnimalAndroid(Animal a){
			this.id = a.ID;
			this.name = a.Name;
			this.desc = a.Description;
			this.Uri = a.Uri;
			this.scientists = a.Subjects;
		}
	}

}

