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
            List<Salle> salles = new List<Salle>
            {
                new Salle(12),
                new Salle(24),
                new Salle(31)
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