using System;
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

        public List<Acteur> ActeursPref { get; set; }
        public List<Realisateur> RealisationsPref { get; set; }
        public List<string> CategoriesPref { get; private set; }



        public Abonne(string pUsername)
        {
            Username = pUsername;


            ActeursPref ??= new List<Acteur>();
            RealisationsPref ??= new List<Realisateur>();
            CategoriesPref ??= new List<string>();
        }

        public Abonne(string pUsername, string pActeurFavorie, string pRealisateurFavorie,
            int pnbSeanceAssistees, DateTime pDateAdhesion, string pFirstName, string pLastname):base(pFirstName, pLastname)
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

        public bool AimeCategorie(string pCategorie)
        {
            try
            {
               CategoriesPref.Add(pCategorie);
               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return true;
        }

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