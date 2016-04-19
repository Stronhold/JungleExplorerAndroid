using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Model.Model
{
	public class Animal
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set;}
		public string Name{ get; set;}
		public string Description{ get; set;}
		public string Uri{ get; set;}
		[ManyToMany(typeof(Scientist))]
		public List<Scientist> Subjects { get; set; } 
		public double latitude{ get; set; }
		public double altitude{ get; set; }

		public Animal ()
		{
		}

		public Animal(string n, string d, string u){
			Name = n;
			Description = d;
			Uri = u;
			latitude = 0;
			altitude = 0;
		}
	}
}

