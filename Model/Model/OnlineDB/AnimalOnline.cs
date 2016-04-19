using System;
using Model.Model;

namespace Model
{
	public class AnimalOnline
	{
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }

		public AnimalOnline ()
		{
		}

		public AnimalOnline(Animal a){
			id = a.ID;
			name = a.Name;
			description = a.Description;
		}
	}
}

