using System;
using System.Collections.Generic;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace Model.Model
{
	public class Scientist
	{
		[PrimaryKey, AutoIncrement]
		public int Id{ get; set;}
		public string Name{ get; set;}
		public string Description { get; set;}
		public string Uri { get; set;}
		[ManyToMany(typeof(Animal))]
		public List<Animal> Subjects { get; set; } 

		public Scientist ()
		{
		}

		public Scientist(string n, string d, string u){
			Name = n;
			Description = d;
			Uri = u;
		}
	}
}

