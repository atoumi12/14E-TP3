using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALActeur : DAL, ICRUD<Acteur>
    {
        public string CollectionName { get; set; }

        public DALActeur(IMongoClient client = null) : base(client)
        {
            CollectionName = "Acteur";
            AddDefaultActeurs();
        }


        private async void AddDefaultActeurs()
        {
            List<Acteur> acteurs = new List<Acteur>
            {
                new Acteur("A-Tom", "Tremblay"),
                new Acteur("A-Giorgina", "Loza"),
                new Acteur("A-Berny", "Fleury")
            };

            try
            {
                var collection = database.GetCollection<Acteur>("Acteur");
                if (collection.CountDocuments(Builders<Acteur>.Filter.Empty) <= 0)
                {
                    await collection.InsertManyAsync(acteurs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible d'ajouter les réalisateurs et les acteurs dans la base de donnée [ {ex.Message} ]",
                    "Erreur de chargement de BD",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }


        public List<Acteur> ReadItems()
        {
            List<Acteur> acteurs = new List<Acteur>();

            try
            {
                var collection = database.GetCollection<Acteur>(CollectionName);
                acteurs = collection.Aggregate().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'obtenir la collection " + ex.Message, "Erreur", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return acteurs;
        }

        public bool AddItem(Acteur pObj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(Acteur pObj)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Acteur pObj)
        {
            throw new NotImplementedException();
        }
    }
}