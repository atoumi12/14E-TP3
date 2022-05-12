using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;

namespace MonCine.Data
{
    public class Abonne : Personne
    {
        //private ObjectId Id { get; set; }
        public string Username { get; set; }
        public DateTime DateAdhesion { get; set; }


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

        public Abonne(string pFirstName, string pLastname, string pUsername,
            int pnbSeanceAssistees, DateTime pDateAdhesion ) : base(pFirstName,
            pLastname)
        {
            Username = pUsername;
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

       
        
       

        #region Realisateur

        public bool AjouterRealisateurFavori(Realisateur pRealisateur)
        {
            try
            {
                if (pRealisateur is null)
                {
                    throw new ArgumentNullException("pRealisateur", "Le réalisateur ne peut pas être null");
                }

                bool realisateurIsToAdd = RealisationsPref.Count < 5 && !RealisationsPref.Contains(pRealisateur);

                if (realisateurIsToAdd)
                {
                    RealisationsPref.Add(pRealisateur);
                }
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return true;
        }

        public bool SupprimerRealisateurFavori(Realisateur pRealisateur)
        {
            try
            {
                if (pRealisateur is null)
                {
                    throw new ArgumentNullException("pRealisateur", "Le réalisateur ne peut pas être null ");
                }

                bool realisateurIsToDelete = RealisationsPref.Contains(pRealisateur);
                if (realisateurIsToDelete)
                {
                    RealisationsPref.Remove(pRealisateur);
                }
            }
            catch (Exception e)
            {
                throw new Exception($" [{e.GetType()}] : {e.Message}");
            }

            return true;
        }


        #endregion


        public bool EstPrioriaitaire()
        {
            throw new NotImplementedException();
        }


        public string AfficherActeurs()
        {
            string res = "";
            ActeursPref.ForEach(acteur=> res += acteur);
            return res.Length > 0 ? res : "Aucun acteur ajouté en favori";
        }

        public string AfficherRealisateurs()
        {
            string res = "";
            RealisationsPref.ForEach(r => res += r);
            return res.Length > 0 ? res : "Aucun réalisateur ajouté en favori";
        }
        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}