using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using MonCine.Data.Entitites;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALRecompense : DAL, ICRUD<Recompense>
    {
        public string CollectionName { get; set; }

        public DALRecompense(IMongoClient client = null) : base(client)
        {
            CollectionName = "Recompense";
            AddDefaultRecompenses();
        }

        private async void AddDefaultRecompenses()
        {
            List<Recompense> recompenses = new List<Recompense>
            {
                new Recompense(TypeRecompense.AvantPremiere),
                new Recompense(TypeRecompense.Reprojection),
                new Recompense(TypeRecompense.AvantPremiere),
                new Recompense(TypeRecompense.Reprojection),
            };

            try
            {
                var collection = database.GetCollection<Recompense>(CollectionName);
                if (collection.CountDocuments(Builders<Recompense>.Filter.Empty) <= 0)
                {
                    await collection.InsertManyAsync(recompenses);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des récompenses zdans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }


        public List<Recompense> ReadItems()
        {
            throw new NotImplementedException();
        }

        public bool AddItem(Recompense pObj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(Recompense pRecompense)
        {
            if (pRecompense is null)
            {
                throw new ArgumentNullException("pRecompense", "La récompense ne peut pas être nulle");
            }

            try
            {
                var collection = database.GetCollection<Recompense>(CollectionName);
                collection.ReplaceOne(r => r.Id == pRecompense.Id, pRecompense);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des récompenses zdans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

            return true;
        }

        public bool DeleteItem(Recompense pObj)
        {
            throw new NotImplementedException();
        }

        // TODO: Methode tests

        public bool AssignerRecompense(Recompense pRecompense, Abonne pAbonne, Film pFilm)
        {
            if (pAbonne is null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être nul");
            }
            if (pFilm is null)
            {
                throw new ArgumentNullException("pFilm", "Le film ne peut pas être nul");
            }
            if (pRecompense is null)
            {
                throw new ArgumentNullException("pRecompense",
                    "Vous devez fournir un type de récompense afin de l'assigner au film");
            }

            try
            {
                var collection = database.GetCollection<Recompense>(CollectionName);

                // Récuperer la récompense consernée
                var filter = Builders<Recompense>.Filter.Eq(r => r.Id, pRecompense.Id);
                Recompense recompense = collection.FindSync(filter).ToList()[0];
                recompense.Abonne = pAbonne;
                recompense.Film = pFilm;
                return UpdateItem(recompense);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des récompenses zdans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

        }
    }
}