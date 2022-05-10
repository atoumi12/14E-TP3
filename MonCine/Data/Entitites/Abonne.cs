using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace MonCine.Data
{
    public class Abonne : Personne
    {
        //private ObjectId Id { get; set; }
        public string Username { get; set; }
        public DateTime DateAdhesion { get; set; }

        public String ActeurFavorie { get; set; }

        public String RealisateurFavorie { get; set; }

        public String Categorie { get; set; }

        public bool Recomprenses { get; set; }

        public bool Reservation { get; set; }

        public int nbSeanceAssistees { get; set; }


        public List<string> CategoriesPref { get; private set; }
        public List<Acteur> ActeursPref { get; set; }
        public List<Realisateur> RealisationsPref { get; set; }


        public Abonne(string pUsername)
        {
            Username = pUsername;


            ActeursPref ??= new List<Acteur>();
            RealisationsPref ??= new List<Realisateur>();
            CategoriesPref ??= new List<string>();
        }

        public Abonne(string pUsername, string pActeurFavorie, string pRealisateurFavorie,
            int pnbSeanceAssistees, DateTime pDateAdhesion, string pFirstName, string pLastname) : base(pFirstName,
            pLastname)
        {
            Username = pUsername;
            ActeurFavorie = pActeurFavorie;
            RealisateurFavorie = pRealisateurFavorie;
            nbSeanceAssistees = pnbSeanceAssistees;
            DateAdhesion = pDateAdhesion;
            FirstName = pFirstName;
            LastName = pLastname;

            ActeursPref ??= new List<Acteur>();
            RealisationsPref ??= new List<Realisateur>();
            CategoriesPref ??= new List<string>();
        }


        public void NoterFilm()
        {
            throw new NotImplementedException();
        }

        #region Cat

        /// <summary>
        /// Cette méthode permet d'ajouter une catégorie en favorie.
        /// </summary>
        /// <param name="pCategorie"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AjouterCategorieFavorie(string pCategorieToAdd)
        {
            try
            {
                pCategorieToAdd = pCategorieToAdd.Trim();
                if (string.IsNullOrWhiteSpace(pCategorieToAdd) || CategoriesPref.Count > 2 ||
                    CategoriesPref.Contains(pCategorieToAdd))
                {
                    throw new ArgumentException("La catégorie ne peut pas être insérée dans la liste des préférences",
                        "pCategorie");
                }

                CategoriesPref.Add(pCategorieToAdd);
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return true;
        }

        /// <summary>
        /// Permet de supprimer une catégorie favorite
        /// </summary>
        /// <param name="pCategorieToDelete">Catégorie à supprimer</param>
        /// <returns>Vrai si catégorie supprimé</returns>
        /// <exception cref="Exception"></exception>
        public bool SupprimerCategorieFavorie(string pCategorieToDelete)
        {
            try
            {
                pCategorieToDelete = pCategorieToDelete.Trim();
                if (string.IsNullOrWhiteSpace(pCategorieToDelete) || !CategoriesPref.Contains(pCategorieToDelete))
                {
                    throw new ArgumentException("La catégorie ne peut pas être supprimée de la liste des préférences",
                        "pCategorieToDelete");
                }

                CategoriesPref.Remove(pCategorieToDelete);
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return true;
        }

        #endregion

        #region Acteur

        /// <summary>
        /// Permet d'ajouter un acteur favori dans la liste, le nombre maximale est de 5 acteurs.
        /// </summary>
        /// <param name="pActeur"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AjouterActeurFavori(Acteur pActeur)
        {
            if (pActeur is null)
            {
                throw new ArgumentNullException("pActeur", "L'acteur ne peut pas être null ");
            }

            bool acteurIsToAdd = ActeursPref.Count < 5 && !ActeursPref.Contains(pActeur);

            if (acteurIsToAdd)
            {
                ActeursPref.Add(pActeur);
            }
        }

        #endregion



        public bool EstPrioriaitaire()
        {
            throw new NotImplementedException();
        }


        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}