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


        public bool AjouterDateProjection(Projection pProjection)
        {
            if (pProjection is null)
            {
                throw new ArgumentNullException("pProjection", "La projection ne peut pas être nulle");
            }

            SurAffiche = true;

            if (DatesProjection.Count < 2)
            {
                DatesProjection.Add(pProjection.DateFin);
                return true;
            }

            return false;
        }


        public bool AdmissibleReprojection()
        {
            // Si moins que deux projections, on peut pas procéder à une reprojection, il nous faut un min de deux projections ^_^
            if (!DatesProjection.Any() || DatesProjection.Count < 2)
            {
                // TODO: false, Test : true
                return true;
            }

            DateTime datePremiereProjectioons = DatesProjection.OrderBy(x => x.Date).ToList()[0];
            bool b = datePremiereProjectioons > DateTime.Now.AddYears(1);
            return b;
        }


        public bool Recompenses_AbonneAdmissibleReProjection(Abonne pAbonne)
        {
            bool exists = false;
            pAbonne.CategoriesPref?.ForEach(cat =>
            {
                if (Categories.Contains(Enum.Parse<Categorie>(cat)))
                {
                    exists = true;
                }
            });

            return exists;
        }



        public bool AdmissibleAvantPremiere()
        {
            // l'avant premiere d'un film est la toute premiere projection de ce film lors de la sortie cinema
            // je verifie donc qu'il est la l'affiche et qu'ils possaide plusieurs projections. L'avant premiere sera 
            // la premiere de la liste
            if(SurAffiche && DatesProjection.Count > 0)
            {
                return true;
            }

            return false;
        }







        public override string ToString()
        {
            return $"{Name} - ({NoteMoyenne}/10)";
        }
    }
}