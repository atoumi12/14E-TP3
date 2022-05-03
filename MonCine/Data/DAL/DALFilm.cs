using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALFilm : DAL, ICRUD<Film>
    {
        public string CollectionName { get; set; }

        public DALFilm(IMongoClient client = null) : base(client)
        {
            CollectionName = "Film";
            AddDefaultFilms();
        }


        /// <summary>
        /// Populer la collections film au départ
        /// </summary>
        private async void AddDefaultFilms()
        {
            List<Film> films = new List<Film>
            {
                new Film("Film 1")
            };

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                if (collection.CountDocuments(Builders<Film>.Filter.Empty) <= 0)
                {
                    await collection.InsertManyAsync(films);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'ajouter des films dans la collection " + ex.Message, "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                throw;
            }
        }


        /// <summary>
        /// Récupère l'ensemble des films de la BD
        /// </summary>
        /// <returns>Liste de films</returns>
        public List<Film> ReadItems()
        {
            List<Film> films = new List<Film>();

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                films = collection.FindSync(Builders<Film>.Filter.Empty).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Impossible d'obtenir la collection " + ex.Message, "Erreur", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return films;
        }


        public bool AddItem(Film pFilm)
        {
            if (pFilm is null)
            {
                throw new ArgumentNullException("pFilm", "Le film ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                collection.InsertOne(pFilm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ajouter le film {pFilm.Name} dans la collection {ex.Message}",
                    "Erreur d'ajout", MessageBoxButton.OK, MessageBoxImage.Error);

                throw;
            }

            return true;
        }

        public bool UpdateItem(Film pFilm)
        {
            if (pFilm is null)
            {
                throw new ArgumentNullException("pFilm", "Le film ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                collection.ReplaceOne(x => x.Id == pFilm.Id, pFilm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de mettre à jour le film {pFilm.Name} dans la collection {ex.Message}",
                    "Erreur de mise à jour", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            return true;
        }

        public bool DeleteItem(Film pFilm)
        {
            if (pFilm is null)
            {
                throw new ArgumentNullException("pFilm", "Le film ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                collection.DeleteOne(Builders<Film>.Filter.Eq(x => x.Id, pFilm.Id));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de supprimer le film {pFilm.Name} dans la collection {ex.Message}",
                    "Erreur de mise à jour", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            return true;
        }


        /// <summary>
        /// Permet d'attribuer la date de la projection au film concerné
        /// </summary>
        /// <param name="pProjection">La projection en question</param>
        /// <returns>Vrai si c'est réussi, sinon False</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AddProjectionDate(Projection pProjection)
        {
            if (pProjection is null)
            {
                throw new ArgumentNullException("pProjection", "La projection ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Film>(CollectionName);
                pProjection.Film.AjouterDateProjection(pProjection);

                UpdateItem(pProjection.Film);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de mettre à jour le film {pProjection.Film.Name} dans la collection {ex.Message}",
                    "Erreur de mise à jour", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            return true;
        }
    }
}