using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALAbonne : DAL, ICRUD<Abonne>
    {
        public string CollectionName { get; set; }


        public DALAbonne(IMongoClient client = null) : base(client)
        {
            CollectionName = "Abonne";
            AddDefaultAbonnes();
        }

        public bool AddItem(Abonne pObj)
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Abonne pObj)
        {
            throw new NotImplementedException();
        }

        public List<Abonne> ReadItems()
        {
            List<Abonne> abonnes = new List<Abonne>();

            try
            {
                var collection = database.GetCollection<Abonne>(CollectionName);
                abonnes = collection.FindSync(Builders<Abonne>.Filter.Empty).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'obtenir la collection " + ex.Message, "Erreur", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return abonnes;
        }

        public bool UpdateItem(Abonne pObj)
        {
            throw new NotImplementedException();
        }

        private async void AddDefaultAbonnes()
        {
            DateTime uneDate = new DateTime();
            uneDate = DateTime.Today;
            List<Abonne> abonnes = new List<Abonne>
            {
                new Abonne("Abonne 1", "Leonardo Di caprio", "Denis Villeneuve", 12, uneDate, "Gwenael", "Galliot"),
                new Abonne("Abonne 2", "Johnny depp", "Denis Villeneuve", 3, uneDate, "Loan", "Rage"),
                new Abonne("Abonne 3", "robert downey jr", "Denis Villeneuve", 22, uneDate, "Ahmed", "Toumi")
            };

            try
            {
                var collection = database.GetCollection<Abonne>(CollectionName);
                if (collection.CountDocuments(Builders<Abonne>.Filter.Empty) <= 0)
                {
                    await collection.InsertManyAsync(abonnes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des abonnés dans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }
    }
}