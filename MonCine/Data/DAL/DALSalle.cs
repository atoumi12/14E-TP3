using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALSalle : DAL, ICRUD<Salle>
    {
        public string CollectionName { get; set; }

        public DALSalle(IMongoClient client = null) : base(client)
        {
            CollectionName = "Salle";
            AddDefaultsalle();
        }

        private void AddDefaultsalle()
        {

            List<Place> listDePlaceGenerique = CreeListeDePlace();

            List<Salle> salles = new List<Salle>
            {
                new Salle(12,listDePlaceGenerique),
                new Salle(24,listDePlaceGenerique),
                new Salle(31,listDePlaceGenerique)
            };

            try
            {
                var collection = database.GetCollection<Salle>(CollectionName);
                if (collection.CountDocuments(Builders<Salle>.Filter.Empty) <= 0)
                {
                    collection.InsertManyAsync(salles);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des salles dans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public List<Place> CreeListeDePlace()
        {
            List<Place> lstPlace = new List<Place>();
            Place Place1 = new Place(0);
            Place Place2 = new Place(1);
            Place Place3 = new Place(2);
            Place Place4 = new Place(3);
            Place Place5 = new Place(4);
            Place Place6 = new Place(5);
            lstPlace.Add(Place1);
            lstPlace.Add(Place2);
            lstPlace.Add(Place3);
            lstPlace.Add(Place4);
            lstPlace.Add(Place5);
            lstPlace.Add(Place6);

            return lstPlace;
        }

        public List<Salle> ReadItems()
        {
            List<Salle> salles = new List<Salle>();

            try
            {
                var collection = database.GetCollection<Salle>(CollectionName);
                salles = collection.FindSync(Builders<Salle>.Filter.Empty).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'obtenir la collection " + ex.Message, "Erreur", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return salles;
        }


        public bool AddItem(Salle pSalle)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(Salle pObj)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Salle pObj)
        {
            throw new NotImplementedException();
        }
    }
}