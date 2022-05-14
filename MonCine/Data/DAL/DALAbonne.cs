using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MongoDB.Driver;

namespace MonCine.Data
{
    public class DALAbonne : DAL, ICRUD<Abonne>
    {
        #region Const

        public const int NB_CAT_MAX = 3;
        public const int NB_ACTEUR_MAX = 5;
        public const int NB_REALISATEUR_MAX = 5;

        #endregion
        public string CollectionName { get; set; }

        public DALAbonne(IMongoClient client = null) : base(client)
        {
            CollectionName = "Abonne";
            AddDefaultAbonnes();
        }

        public bool AddItem(Abonne pabonne)
        {
            if (pabonne is null)
            {
                throw new ArgumentNullException("pFilm", "Le film ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Abonne>(CollectionName);
                collection.InsertOne(pabonne);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ajouter le film {pabonne.FirstName} dans la collection {ex.Message}",
                    "Erreur d'ajout", MessageBoxButton.OK, MessageBoxImage.Error);

                throw;
            }

            return true;
        }

        private async void AddDefaultAbonnes()
        {
            DateTime uneDate = new DateTime();
            uneDate = DateTime.Today;
            List<Abonne> abonnes = new List<Abonne>
            {
                new Abonne("Gwenael", "Galliot", "Abonne 1", 12, uneDate),
                new Abonne("Loan", "Rage", "Abonne 2", 3, uneDate),
                new Abonne("Ahmed", "Toumi", "Abonne 3", 22, uneDate)
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

        public bool DeleteItem(Abonne pObj)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(Abonne pAbonne)
        {
            if (pAbonne is null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonne ne peut pas être null");
            }

            try
            {
                var collection = database.GetCollection<Abonne>(CollectionName);
                collection.ReplaceOne(x => x.Id == pAbonne.Id, pAbonne);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Impossible de mettre à jour l'abonne {pAbonne.Username} dans la collection {ex.Message}",
                    "Erreur de mise à jour", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }

            return true;
        }


        #region Cat

        /// <summary>
        /// Cette méthode permet d'ajouter une catégorie en favorie.
        /// </summary>
        /// <param name="pCategorie"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AjouterCategorieFavorie(Abonne pAbonne, string pCategorieToAdd)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            try
            {
                pCategorieToAdd = pCategorieToAdd?.Trim();
                bool error = string.IsNullOrWhiteSpace(pCategorieToAdd) || pAbonne.CategoriesPref.Count > DALAbonne.NB_CAT_MAX ||
                             pAbonne.CategoriesPref.Contains(pCategorieToAdd);
                if (error)
                {
                    throw new ArgumentException("La catégorie ne peut pas être insérée dans la liste des préférences",
                        "pCategorie", null);
                }

                pAbonne.CategoriesPref.Add(pCategorieToAdd);
                return UpdateItem(pAbonne);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

        }

        /// <summary>
        /// Permet de supprimer une catégorie favorite
        /// </summary>
        /// <param name="pCategorieToDelete">Catégorie à supprimer</param>
        /// <returns>Vrai si catégorie supprimé</returns>
        /// <exception cref="Exception"></exception>
        public bool SupprimerCategorieFavorie(Abonne pAbonne, string pCategorieToDelete)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            try
            {
                pCategorieToDelete = pCategorieToDelete?.Trim();
                bool error = string.IsNullOrWhiteSpace(pCategorieToDelete) ||
                             !pAbonne.CategoriesPref.Contains(pCategorieToDelete);
                if (error)
                {
                    throw new ArgumentException("La catégorie ne peut pas être supprimée de la liste des préférences",
                        "pCategorieToDelete");
                }

                pAbonne.CategoriesPref.Remove(pCategorieToDelete);
                return UpdateItem(pAbonne);
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

        }

        #endregion


        #region Acteur

        /// <summary>
        /// Permet d'ajouter un acteur favori dans la liste, le nombre maximale est de 5 acteurs.
        /// </summary>
        /// <param name="pActeur"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool AjouterActeurFavori(Abonne pAbonne, Acteur pActeur)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            if (pActeur is null)
            {
                throw new ArgumentNullException("pActeur", "L'acteur ne peut pas être null ");
            }
            try
            {
                bool exists = pAbonne.ActeursPref.Where(a => a.Id == pActeur.Id).ToList().Count > 0;
                bool acteurIsToAdd = pAbonne.ActeursPref.Count < DALAbonne.NB_ACTEUR_MAX && !exists;

                if (acteurIsToAdd)
                {
                    pAbonne.ActeursPref.Add(pActeur);
                    return UpdateItem(pAbonne);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return false;
        }

        public bool SupprimerActeurFavori(Abonne pAbonne, Acteur pActeur)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            if (pActeur is null)
            {
                throw new ArgumentNullException("pActeur", "L'acteur ne peut pas être null ");
            }
            try
            {
                bool acteurIsToDelete = pAbonne.ActeursPref.Where(a => a.Id == pActeur.Id).ToList().Count > 0;
                if (acteurIsToDelete)
                {
                    pAbonne.ActeursPref.Remove(pActeur);
                    return UpdateItem(pAbonne);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return false;
        }

        #endregion



        #region Realisateur

        public bool AjouterRealisateurFavori(Abonne pAbonne, Realisateur pRealisateur)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            if (pRealisateur is null)
            {
                throw new ArgumentNullException("pRealisateur", "Le réalisateur ne peut pas être null");
            }
            try
            {
                bool exists = pAbonne.RealisationsPref.Where(r => r.Id == pRealisateur.Id).ToList().Count > 0;
                bool realisateurIsToAdd = pAbonne.RealisationsPref.Count < DALAbonne.NB_REALISATEUR_MAX && !exists;

                if (realisateurIsToAdd)
                {
                    pAbonne.RealisationsPref.Add(pRealisateur);
                    return UpdateItem(pAbonne);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return false;
        }

        public bool SupprimerRealisateurFavori(Abonne pAbonne, Realisateur pRealisateur)
        {
            if (pAbonne == null)
            {
                throw new ArgumentNullException("pAbonne", "L'abonné ne peut pas être null");
            }
            if (pRealisateur is null)
            {
                throw new ArgumentNullException("pRealisateur", "Le réalisateur ne peut pas être null");
            }
            try
            {
                bool realisateurIsToDelete  = pAbonne.RealisationsPref.Where(r => r.Id == pRealisateur.Id).ToList().Count > 0;
                if (realisateurIsToDelete)
                {
                    pAbonne.RealisationsPref.Remove(pRealisateur);
                    return UpdateItem(pAbonne);
                }
            }
            catch (ArgumentNullException e)
            {
                throw new ArgumentNullException($"{e.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return false;
        }

        #endregion
    }
}