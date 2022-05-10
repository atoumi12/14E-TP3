using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Accessibility;
using MonCine.Data;

namespace MonCine.Vues
{
    /// <summary>
    /// Interaction logic for FPreferencesAbonne.xaml
    /// </summary>
    public partial class FPreferencesAbonne : Window
    {
        private string Saluer(string nom) => $"Bonjour, {nom} ! Voici vos préférences";

        #region CONST

        private const int NB_CAT_MAX = 3;
        private const int NB_ACTEUR_MAX = 5;
        private const int NB_REALISATEUR_MAX = 5;

        #endregion


        private DALRealisateur _dalRealisateur { get; set; }
        private DALAbonne _dalAbonne { get; set; }
        private DALActeur _dalActeur { get; set; }

        #region Lists

        private List<string> _categories { get; set; }
        private List<string> _categoriesPref { get; set; }
        private List<Acteur> _acteurs { get; set; }
        private List<Acteur> _acteursPref { get; set; }
        private List<Realisateur> _realisateurs { get; set; }
        private List<Realisateur> _realisateursPref { get; set; }


        private Abonne _abo { get; set; }

        #endregion

        public FPreferencesAbonne(DALRealisateur pDalRealisateur, DALAbonne pDalAbonne, DALActeur pDalActeur)
        {
            InitializeComponent();

            _dalRealisateur = pDalRealisateur;
            _dalAbonne = pDalAbonne;
            _dalActeur = pDalActeur;

            List<Abonne> a = pDalAbonne.ReadItems();
            _abo = a[0];

            InstanceOfItems();
        }

        private void InstanceOfItems()
        {
            txtSalutations.Text = Saluer(_abo.FirstName);

            _realisateurs = _dalRealisateur.ReadItems();


            // Categories
            _categories = typeof(Categorie).GetEnumNames().ToList();
            lstCategories.ItemsSource = _categories;

            _categoriesPref ??= new List<string>();

            //lstCategoriesPref.ItemsSource = _categoriesPref;
            if (_abo.CategoriesPref.Count > 0)
            {
                _abo.CategoriesPref.ForEach(cat =>
                {
                    _categoriesPref.Add(cat);
                    lstCategoriesPref.Items.Add(cat);
                });
            }

            // Acteurs
            _acteurs = _dalActeur.ReadItems();
            lstActeurs.ItemsSource = _acteurs;

            _acteursPref ??= new List<Acteur>();
            if (_abo.CategoriesPref.Count > 0)
            {
                _abo.ActeursPref.ForEach(acteur =>
                {
                    _acteursPref.Add(acteur);
                    lstActeursPref.Items.Add(acteur);
                });
            }

            // Realisateurs
            // TODO: Populer les lst
            _realisateurs = _dalRealisateur.ReadItems();
            
            
            _realisateursPref ??= new List<Realisateur>();
        }

        #region categories

        private void lstCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstCategories.SelectedIndex == -1)
            {
                return;
            }

            if (lstCategories.SelectedItem != null)
            {
                // Limite de trois catégorie préférée
                if (lstCategoriesPref.Items.Count >= FPreferencesAbonne.NB_CAT_MAX)
                {
                    MessageBox.Show(
                        $"Le nombre maximale de catégories en favories est  : {FPreferencesAbonne.NB_CAT_MAX}, " +
                        $"veuillez en supprimer pour en rajouter", "Ajout de catégorie préférée", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                string categorie = lstCategories.SelectedItem.ToString()?.Trim() ?? "";

                // N'est pas contenu dans la liste locale.
                bool catToAdd = !string.IsNullOrWhiteSpace(categorie) && !_categoriesPref.Contains(categorie);
                if (catToAdd)
                {
                    bool catAjoutee = _abo.AjouterCategorieFavorie(categorie) && _dalAbonne.UpdateItem(_abo);
                    if (catAjoutee)
                    {
                        _categoriesPref.Add(categorie);
                        lstCategoriesPref.Items.Add(categorie);
                    }
                    else
                    {
                        MessageBox.Show("Erreur d'ajout de catégorie !", "Erreur", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"La catégorie \"{categorie}\" existe déjà dans votre liste de préférences.",
                        "Ajout de catégorie",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void lstCategoriesPref_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstCategoriesPref.SelectedIndex == -1)
            {
                return;
            }

            string categorie = lstCategoriesPref.SelectedItem.ToString()?.Trim() ?? "";

            bool catCanBeDeleted = !string.IsNullOrWhiteSpace(categorie) && _categoriesPref.Contains(categorie);
            if (catCanBeDeleted)
            {
                bool catDeleted = _abo.SupprimerCategorieFavorie(categorie) && _dalAbonne.UpdateItem(_abo);

                if (catDeleted)
                {
                    _categoriesPref.Remove(categorie);
                    lstCategoriesPref.Items.Remove(categorie);
                }
                else
                {
                    MessageBox.Show("Erreur de suppression de catégorie ! ", "Erreur", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"La catégorie \"{categorie}\" n'existe pas dans votre liste de préférences.",
                    "Suppression de catégorie",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion


        #region acteurs

        private void lstActeurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstActeurs.SelectedIndex == -1)
            {
                return;
            }

            if (lstActeurs.SelectedItem != null)
            {
                if (lstActeurs.Items.Count >= FPreferencesAbonne.NB_ACTEUR_MAX)
                {
                    MessageBox.Show(
                        $"Le nombre maximale d'acteurs en favoris est  : {FPreferencesAbonne.NB_ACTEUR_MAX}, " +
                        $"veuillez en supprimer pour en rajouter", "Ajout d'acteur préféré(s)", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }


                Acteur acteur = lstActeurs.SelectedItem as Acteur;
                bool acteurIsToAdd = !_acteursPref.Contains(acteur);

                if (acteurIsToAdd)
                {
                    bool acteurAdded = _abo.AjouterActeurFavori(acteur) && _dalAbonne.UpdateItem(_abo);
                    if (acteurAdded)
                    {
                        _acteursPref.Add(acteur);
                        lstActeursPref.Items.Add(acteur);
                    }
                    else
                    {
                        MessageBox.Show("Erreur d'ajout d'acteur !", "Erreur", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"L'acteur \"{acteur}\" existe déjà dans votre liste de préférences.",
                        "Ajout d'acteur",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        private void lstActeursPref_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstActeursPref.SelectedIndex == -1)
            {
                return;
            }

            Acteur acteur = lstActeursPref.SelectedItem as Acteur;

            bool acteurCanBeDeleted = _acteursPref.Contains(acteur);

            if (acteurCanBeDeleted)
            {
                bool acteurDeleted = _abo.SupprimerActeurFavori(acteur) && _dalAbonne.UpdateItem(_abo);
                if (acteurDeleted)
                {
                    _acteursPref.Remove(acteur);
                    lstActeursPref.Items.Remove(acteur);
                }
                else
                {
                    MessageBox.Show("Erreur de suppression d'acteur ! ", "Erreur", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"L'acteur \"{acteur}\" n'existe pas dans votre liste de préférences.",
                    "Suppression d'acteur",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion


        #region realisateurs

        #endregion
    }
}