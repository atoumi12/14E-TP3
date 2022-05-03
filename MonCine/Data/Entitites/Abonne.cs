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


        public Abonne(string pUsername)
        {
            Username = pUsername;
        }

        public Abonne(string pUsername, string pActeurFavorie, string pRealisateurFavorie,
            int pnbSeanceAssistees, DateTime pDateAdhesion, string pFirstName, string pLastname)
        {
            Username = pUsername;
            ActeurFavorie = pActeurFavorie;
            RealisateurFavorie = pRealisateurFavorie;
            nbSeanceAssistees = pnbSeanceAssistees;
            DateAdhesion = pDateAdhesion;
            FirstName = pFirstName;
            LastName = pLastname;
        }


        public void NoterFilm()
        {
            throw new NotImplementedException();
        }

        public bool AimeCategorie()
        {
            throw new NotImplementedException();
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