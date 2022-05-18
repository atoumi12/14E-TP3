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
        }


        public List<Recompense> ReadItems()
        {
            try
            {
                var collection = database.GetCollection<Recompense>(CollectionName);
                return collection.FindSync(Builders<Recompense>.Filter.Empty).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible de récupérer les récompenses de la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }

        public bool AddItem(Recompense pRecompense)
        {
            if (pRecompense is null)
            {
                throw new ArgumentNullException("pRecompense", "La récompense ne peut pas être nulle");
            }

            try
            {
                var collection = database.GetCollection<Recompense>(CollectionName);
                collection.InsertOne(pRecompense);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des récompenses dans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }

            return true;
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

                // If does not exists !!
                var filter = Builders<Recompense>.Filter.Eq(r => r.Id, pRecompense.Id);
                bool exists = collection.FindSync(filter).ToList().Count > 0;
                if (!exists)
                {
                    return AddItem(pRecompense);
                }

                collection.ReplaceOne(r => r.Id == pRecompense.Id, pRecompense);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible de mettte à jouer la récompense {pRecompense} dans la collection " + ex.Message,
                    "Erreur",
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
                if (AbonneAdmissibleRecompense(pRecompense.Type, pAbonne, pFilm))
                {
                    pRecompense.Abonne = pAbonne;
                    pRecompense.Film = pFilm;
                    return AddItem(pRecompense);
                }

                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible d'assigner la récompense {pRecompense} aux éléments attribués" + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }


        public bool AbonneAdmissibleRecompense(TypeRecompense pTypeRecompense, Abonne pAbonne, Film pFilm)
        {
            List<Recompense> recompenses = ReadItems();

            bool admissbile = true;
            recompenses.ForEach(r =>
            {
                if (r.Type == pTypeRecompense && r.Abonne.Id == pAbonne.Id && r.Film.Id == pFilm.Id)
                {
                    admissbile = false;
                }
            });

            return admissbile;
        }
    }
}