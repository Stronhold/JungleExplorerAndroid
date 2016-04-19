using System;
using Model.Model;

namespace Model
{
	public class RelationAnimalScientistOnline
	{
		public int id{ get; set;}
		public int idAnimal { get; set; }
		public int idScientist { get; set; }

		public RelationAnimalScientistOnline ()
		{
		}

		public RelationAnimalScientistOnline(RelationAnimalScientist r){
			idAnimal = r.AnimalId;
			idScientist = r.ScientistId;
		}
	}
}

