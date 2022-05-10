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
        #region CONST

        private const string SALUTATIONS = "Bonjour, ";
        private const int NB_CAT_MAX = 3;

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

        public FPreferencesAbonne( DALRealisateur pDalRealisateur, DALAbonne pDalAbonne, DALActeur pDalActeur)
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
            txtSalutations.Text = FPreferencesAbonne.SALUTATIONS + _abo.FirstName;

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
                // TODO: Populer la liste des favori au depart si jamais
            }


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
                if (lstCategoriesPref.Items.Count > FPreferencesAbonne.NB_CAT_MAX)
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
                    bool catAjoutee = lstCategoriesPref.Items.Count < 3 && _abo.AjouterCategorieFavorie(categorie) &&
                                      _dalAbonne.UpdateItem(_abo);
                    if (catAjoutee)
                    {
                        _categoriesPref.Add(categorie);
                        lstCategoriesPref.Items.Add(categorie);
                    }
                    else
                    {
                        string nbDepasse = _categoriesPref.Count >= 3
                            ? $"\n - Le nombre maximale de catégorie est de : {FPreferencesAbonne.NB_CAT_MAX}"
                            : "";


                        MessageBox.Show($"Erreur d'ajout de catégorie !  {nbDepasse}", "Erreur", MessageBoxButton.OK,
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
                
                // TODO: validation nombre acteurs

                Acteur acteur = lstActeurs.SelectedItem as Acteur;
                
                
                _acteursPref.Add(acteur);

            }
        }

        # endregion
    }
}