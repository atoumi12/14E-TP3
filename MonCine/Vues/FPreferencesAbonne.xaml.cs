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
        private string Saluer(Abonne abo) => $"Bonjour, {abo.FirstName} ! Voici vos préférences";

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

            // TODO: Choix de l'abonné
            List<Abonne> a = pDalAbonne.ReadItems();
            _abo = a[0];

            InstanceOfItems();
        }

        // TODO: Reparer beug dédoublement valeurs dans les listes.
        private void InstanceOfItems()
        {
            txtSalutations.Text = Saluer(_abo);

            // Categories
            _categories = typeof(Categorie).GetEnumNames().ToList();
            lstCategories.ItemsSource = _categories;

            _categoriesPref ??= new List<string>();
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
            if (_abo.ActeursPref.Count > 0)
            {
                List<Acteur> acteurs = _abo.ActeursPref.OrderBy(a=>a.FirstName).ToList();
                acteurs.ForEach(acteur =>
                {
                    _acteursPref.Add(acteur);
                    lstActeursPref.Items.Add(acteur);
                });
            }

            // Realisateurs
            _realisateurs = _dalRealisateur.ReadItems();
            lstRealisateurs.ItemsSource = _realisateurs;
            
            _realisateursPref ??= new List<Realisateur>();
            if (_abo.RealisationsPref.Any())
            {
                List<Realisateur> realisateurs = _abo.RealisationsPref.OrderBy(r => r.FirstName).ToList();
                realisateurs.ForEach(r =>
                {
                    _realisateursPref.Add(r);
                    lstRealisateursPref.Items.Add(r);
                });
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
                if (lstCategoriesPref.Items.Count >= FPreferencesAbonne.NB_CAT_MAX)
                {
                    MessageBox.Show(
                        $"Le nombre maximale de catégories en favories est : {FPreferencesAbonne.NB_CAT_MAX}" +
                        $"\n\n Veuillez en supprimer pour en rajouter", "Ajout de catégorie préférée", MessageBoxButton.OK,
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

            Acteur acteur = lstActeurs.SelectedItem as Acteur;
            if (acteur != null)
            {
                if (_acteursPref.Count >= FPreferencesAbonne.NB_ACTEUR_MAX)
                {
                    MessageBox.Show(
                        $"Le nombre maximale d'acteurs en favoris est : {FPreferencesAbonne.NB_ACTEUR_MAX}" +
                        $"\n\n Veuillez en supprimer pour en rajouter", "Ajout d'acteur préféré", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                
                bool acteurIsToAdd = !(_acteursPref.Where(a => a.Id == acteur.Id).ToList().Count > 0);
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

        private void lstRealisateurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRealisateurs.SelectedIndex == -1)
            {
                return;
            }
            
            Realisateur realisateur = lstRealisateurs.SelectedItem as Realisateur;
            if (realisateur!= null)
            {
                if (lstRealisateursPref.Items.Count >= FPreferencesAbonne.NB_REALISATEUR_MAX)
                {
                    MessageBox.Show(
                        $"Le nombre maximale de réalisateurs en favoris est : {FPreferencesAbonne.NB_REALISATEUR_MAX}" +
                        $"\n\n Veuillez en supprimer pour en rajouter", "Ajout de réalisateur préféré", MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }


                bool realisateurIsToAdd = !(_realisateursPref.Where(r => r.Id == realisateur.Id).ToList().Count > 0);
                if (realisateurIsToAdd)
                {
                    bool realisateurAdded = _abo.AjouterRealisateurFavori(realisateur) && _dalAbonne.UpdateItem(_abo);
                    if (realisateurAdded)
                    {
                        _realisateursPref.Add(realisateur);
                        lstRealisateursPref.Items.Add(realisateur);
                    }
                    else
                    {
                        MessageBox.Show("Erreur d'ajout de réalisateur !", "Erreur", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Le réslisateur \"{realisateur}\" existe déjà dans votre liste de préférences.",
                        "Ajout de réalisateur",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }



        private void lstRealisateursPref_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRealisateursPref.SelectedIndex == -1)
            {
                return;
            }

            Realisateur realisateur = lstRealisateursPref.SelectedItem as Realisateur;

            bool realisateurCanBeDeleted = _realisateursPref.Contains(realisateur);

            if (realisateurCanBeDeleted)
            {
                bool realisateurDeleted = _abo.SupprimerRealisateurFavori(realisateur) && _dalAbonne.UpdateItem(_abo);
                if (realisateurDeleted)
                {
                    _realisateursPref.Remove(realisateur);
                    lstRealisateursPref.Items.Remove(realisateur);
                }
                else
                {
                    MessageBox.Show("Erreur de suppression de réalisateur ! ", "Erreur", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show($"Le réalisateur \"{realisateur}\" n'existe pas dans votre liste de préférences.",
                    "Suppression de réalisateur",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

    }
}