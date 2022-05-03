using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALRealisateur : DAL, ICRUD<Realisateur>
    {
        public string CollectionName { get; set; }

        public DALRealisateur(IMongoClient client = null) : base(client)
        {
            CollectionName = "Realisateur";
            AddDefaultRealisateurs();
        }


        private async void AddDefaultRealisateurs()
        {
            List<Realisateur> realisateurs = new List<Realisateur>
            {
                new Realisateur("R-Jane", "Doe"),
                new Realisateur("R-Jack", "Jones"),
                new Realisateur("R-Jane", "Doe")
            };

            try
            {
                var colllection = database.GetCollection<Realisateur>("Realisateur");
                if (colllection.CountDocuments(Builders<Realisateur>.Filter.Empty) <= 0)
                {
                    await colllection.InsertManyAsync(realisateurs);
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

        public List<Realisateur> ReadItems()
        {
            List<Realisateur> realisateurs = new List<Realisateur>();

            try
            {
                var colllection = database.GetCollection<Realisateur>(CollectionName);
                realisateurs = colllection.Find(Builders<Realisateur>.Filter.Empty).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'obtenir les données dans la collection Realisateur {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            return realisateurs;
        }

        public bool AddItem(Realisateur pObj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(Realisateur pObj)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Realisateur pObj)
        {
            throw new NotImplementedException();
        }
    }
}