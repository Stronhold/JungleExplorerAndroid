using System;
using System.IO;
using Model.Model;
using SQLite.Net;
using SQLite.Net.Interop;
using System.Collections.Generic;
using Java.IO;
using Android.Content.Res;

namespace JungleExplorer.Service
{
	public class DataManager
	{
		private static DataManager _instance;

	    public readonly string _dbPath;

		public SQLiteConnection Db { get; set; }


	    public static DataManager Instance{
			get { return _instance ?? (_instance = new DataManager()); }
		    set{ 
				_instance = value;
			}
		}

		private DataManager ()
		{
			_dbPath =  Path.Combine (
				Environment.GetFolderPath (Environment.SpecialFolder.Personal),
				"jungleexplorer.db3");
			Java.IO.File f = new Java.IO.File (_dbPath);
			Java.IO.File output = new Java.IO.File ("storage/sdcard0/Android/data/test.db3");
			//if(f.Exists())
			//	copyFile (new FileInputStream(f), new FileOutputStream(output));
			Db = new SQLiteConnection(new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(), _dbPath);
			CreateDatabaseIfNeeded ();
		}

		private void copyFile(InputStream input, OutputStream output)  {
			byte[] buffer = new byte[1024];
			int read;
			while((read = input.Read(buffer)) != -1){
				output.Write(buffer, 0, read);
			}
		}


		void CreateDatabaseIfNeeded ()
		{
			Db.CreateTable<Animal> ();
			Db.CreateTable<Scientist> ();
			Db.CreateTable<RelationAnimalScientist>();
		}

		public List<string> GetScientistsForId(int id){
			var animal = Db.Query<Animal> ("Select * from Animal where id=" + id)[0];
			var listaADevolver = new List<string> ();
			foreach (var s in animal.Subjects) {
				listaADevolver.Add (s.Name);
			}
			return listaADevolver;
		}

		public void AddAnimal(Animal a){
			if (a.Subjects!= null) {
				Db.Query<RelationAnimalScientist> ("Delete from RelationAnimalScientist where animalid = " + a.ID);
				foreach (var s in a.Subjects) {
					AddScientistToAnimal (s, a);
				}
			}
			else{
				a.Subjects = new List<Scientist> ();
			}

			if (Db.Query<Animal> ("Select * from Animal where id = " + a.ID).Count > 0) {
				Db.Update (a);
			}
			else
				Db.Insert (a);
		}

		public void AddScientist(Scientist s){
			if (Db.Query<Scientist> ("Select * from Scientist where id = " + s.Id).Count > 0) {
				Db.Update (s);
			} else {
				Db.Insert (s);
			}
		}

		public void AddScientistToAnimal(Scientist s, Animal a){
			Db.Insert (new RelationAnimalScientist (a.ID, s.Id));
		}

		public List<Animal> GetAllAnimals ()
		{
			List<Animal> lista = Db.Query<Animal> ("Select * from Animal");
			return lista;
		}



		public List<Model.Model.Scientist> GetAllScientist ()
		{
			List<Scientist> lista = Db.Query<Scientist> ("Select * from Scientist");
			return lista;
		}

		public Scientist GetOnlyOneScientistForId(int id){
			return Db.Query<Scientist> ("Select * from Scientist where id = " + id)[0];
		}

		public List<string> GetScientistNames (int id)
		{
			var listaADevolver = new List<string> ();
			if (id != 0) {
				var lista = Db.Query<Scientist> ("Select * from Scientist");
				var animal = Db.Query<RelationAnimalScientist> ("Select * from RelationAnimalScientist where animalid=" + id);
				if (animal == null) {
					foreach (var s in lista) {
						listaADevolver.Add (s.Name);
					}
				} else {
					foreach (var s in lista) {
						bool exists = false;
						for (int i = 0; i < animal.Count; i++) {
							var idCientifico = animal [i].ScientistId;
							if (idCientifico == s.Id) {
								exists = true;
								break;
							}
						}
						if (!exists) {
							listaADevolver.Add (s.Name);
						}
					}
				}
			} else {
				var lista = Db.Query<Scientist> ("Select * from Scientist");
				foreach (var s in lista) {
					listaADevolver.Add (s.Name);
				}
			}
			return listaADevolver;
		}

		public List<Scientist> GetScientist (int id)
		{
			var listaADevolver = new List<Scientist> ();
			if (id != 0) {
				var lista = Db.Query<Scientist> ("Select * from Scientist");
				var animal = Db.Query<Animal> ("Select * from Animal where id=" + id)[0];
				var cientificos = animal.Subjects;
				if (lista == null) {
					lista = new List<Scientist> ();
				}
				if (cientificos == null) {
					cientificos = new List<Scientist> ();
				}
				foreach (var s in lista) {
					if (!cientificos.Contains (s)) {
						listaADevolver.Add (s);
					}
				}
			} else {
				var lista = Db.Query<Scientist> ("Select * from Scientist");
				foreach (var s in lista) {
					listaADevolver.Add (s);
				}
			}
			return listaADevolver;
		}

		public List<string> GetScientistNamesFromAnimal (int id)
		{
			//var scientists = List<Scientist> ();
			var relation = Db.Query<RelationAnimalScientist> ("Select * from RelationAnimalScientist where AnimalId=" + id);
			var lista = new List<string> ();
			if (relation != null) {
				foreach(var r in relation){
					var s = Db.Query<Scientist> ("Select * from Scientist where id= " + r.ScientistId) [0];
					lista.Add (s.Name);
				}
			}
			return lista;
		}

		public AnimalAndroid GetAnimal(int id){
			return new AnimalAndroid(Db.Query<Animal> ("Select * from Animal where id=" + id)[0]);
		}

		public List<ScientistAdded> GetScientistForAnimal (int animalID)
		{
			var listaFinal = new List<ScientistAdded> ();
			var listaCientificos = GetAllScientist ();
			var lista = Db.Query<RelationAnimalScientist> ("Select * from RelationAnimalScientist where AnimalId=" + animalID);
			foreach (var c in listaCientificos) {
				bool isThere = false;
				foreach (var relation in lista) {
					if (relation.ScientistId == c.Id) {
						isThere = true;
					}
				}
				listaFinal.Add (new ScientistAdded (c, isThere));
			}
			return listaFinal;
		}
	}
}

