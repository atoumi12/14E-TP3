using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonCine.Data
{
    public class Film
    {
        [BsonId] public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<Categorie> Categories { get; set; }
        public DateTime DateSortie { get; set; }
        public List<DateTime> DatesProjection { get; set; }
        public DateTime DerniereProjection { get; set; }
        public bool SurAffiche { get; set; }
        public List<Acteur> Acteurs { get; set; }
        public List<Realisateur> Realisateurs { get; set; }
        public List<int> Notes { get; set; }
        public double NoteMoyenne { get; set; }


        [Range(0, 2, ErrorMessage = "Le nombre de projection d'un film ne peut pas dépasser 2 projections pas années")]
        private int NbProjection { get; set; }


        public Film(string pName, List<Categorie> pCategories = null, List<Acteur> pActeurs = null,
            List<Realisateur> pRealisateurs = null, bool pSurAffiche = false)
        {
            Name = pName;
            Categories = pCategories ?? GenerateCategories();
            Acteurs = pActeurs ?? new List<Acteur>();
            Realisateurs = pRealisateurs ?? new List<Realisateur>();
            SurAffiche = pSurAffiche;

            Notes = GenerateNotes();
            NoteMoyenne = CalculerMoyennesNotes();

            DatesProjection = new List<DateTime>();
            NbProjection = 0;
        }

        /// <summary>
        /// Constructeur permettant de forcer une génération d'id unique.
        /// </summary>
        /// <param name="genID"></param>
        /// <param name="pName"></param>
        /// <param name="pCategories"></param>
        /// <param name="pActeurs"></param>
        /// <param name="pRealisateurs"></param>
        /// <param name="pSurAffiche"></param>
        public Film(bool genID, string pName, List<Categorie> pCategories = null, List<Acteur> pActeurs = null,
            List<Realisateur> pRealisateurs = null, bool pSurAffiche = false)
        {
            if (genID)
            {
                Id = ObjectId.GenerateNewId();
            }

            Name = pName;
            Categories = pCategories ?? GenerateCategories();
            Acteurs = pActeurs ?? new List<Acteur>();
            Realisateurs = pRealisateurs ?? new List<Realisateur>();
            SurAffiche = pSurAffiche;

            Notes = GenerateNotes();
            NoteMoyenne = CalculerMoyennesNotes();

            DatesProjection = new List<DateTime>();
            NbProjection = 0;
        }


        #region DEV

        private List<int> GenerateNotes()
        {
            Random random = new Random();
            List<int> notes = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                notes.Add(random.Next(1, 11));
            }

            return notes;
        }

        private List<Categorie> GenerateCategories()
        {
            Random random = new Random();

            List<String> enumNames = typeof(Categorie).GetEnumNames().ToList();
            List<Categorie> categories = new List<Categorie>();

            for (int i = 0; i < 5; i++)
            {
                int indiceCat = random.Next(enumNames.Count);
                Categorie cat = (Categorie)indiceCat;

                if (!categories.Contains(cat))
                {
                    categories.Add(cat);
                }
            }

            return categories;
        }

        #endregion

        public double CalculerMoyennesNotes()
        {
            Notes ??= new List<int>();
            int taille = Notes.Count > 0 ? Notes.Count : 1;

            return Notes.Sum(x => x) / taille;
        }


        public void AjouterDateProjection(Projection pProjection)
        {
            if (pProjection is null)
            {
                throw new ArgumentNullException("pProjection", "La projection ne peut pas être nulle");
            }

            DatesProjection.Add(pProjection.DateDebut);
            NbProjection++;
        }

        public override string ToString()
        {
            return $"{Name} - ({NoteMoyenne}/10)";
        }
    }
}