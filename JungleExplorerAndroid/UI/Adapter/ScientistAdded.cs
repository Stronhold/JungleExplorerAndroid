using System;
using Android.Widget;
using Android.Views;
using System.Collections.Generic;
using Model.Model;

namespace JungleExplorer
{

	public class ScientistAdded :  Java.Lang.Object
	{
		public int id {get;set;}
		public string name {get;set;}
		public bool added { get; set; }

		public ScientistAdded(Scientist s, bool a){
			id = s.Id;
			name = s.Name;
			added = a;
		}
	}

}

