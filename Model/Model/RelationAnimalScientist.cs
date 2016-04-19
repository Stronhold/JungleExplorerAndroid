using SQLiteNetExtensions.Attributes;

namespace Model.Model
{
	public class RelationAnimalScientist
	{
		[ForeignKey(typeof(Animal))]
		public int AnimalId { get; set; }

		[ForeignKey(typeof(Scientist))]
		public int ScientistId { get; set; }

		public RelationAnimalScientist ()
		{
		}

		public RelationAnimalScientist(int aID, int sID){
			AnimalId = aID;
			ScientistId = sID;
		}
	}
}

