using System;
using Model.Model;

namespace Model
{
	public class ScientistOnline
	{
		public int id { get; set; }
		public string name { get; set; }
		public string description { get; set; }

		public ScientistOnline ()
		{
		}

		public ScientistOnline(Scientist s){
			id = s.Id;
			name = s.Name;
			description = s.Description;
		}
	}
}

