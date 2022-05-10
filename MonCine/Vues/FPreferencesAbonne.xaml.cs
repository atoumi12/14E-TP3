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
using MonCine.Data;

namespace MonCine.Vues
{
    /// <summary>
    /// Interaction logic for FPreferencesAbonne.xaml
    /// </summary>
    public partial class FPreferencesAbonne : Window
    {
        private const string SALUTATIONS = "Bonjour, ";
        private DALFilm _dalFilm { get; set; }
        private DALRealisateur _dalRealisateur { get; set; }
        private DALAbonne _dalAbonne { get; set; }

        #region Lists

        private List<string> _categories { get; set; }
        private List<string> _categoriesPref { get; set; }
        private List<Film> _films { get; set; }
        private List<Film> _filmsPref { get; set; }
        private List<Realisateur> _realisateurs { get; set; }
        private List<Realisateur> _realisateursPref { get; set; }


        private Abonne _abo { get; set; }

        #endregion

        public FPreferencesAbonne(DALFilm pDalFilm, DALRealisateur pDalRealisateur, DALAbonne pDalAbonne)
        {
            InitializeComponent();

            _dalFilm = pDalFilm;
            _dalRealisateur = pDalRealisateur;
            _dalAbonne = pDalAbonne;
            
            List<Abonne> a = pDalAbonne.ReadItems();
            _abo = a[0];

            InstanceOfItems();
        }

        private void InstanceOfItems()
        {
            txtSalutations.Text = FPreferencesAbonne.SALUTATIONS + _abo.FirstName;

            _films = _dalFilm.ReadItems();
            _realisateurs = _dalRealisateur.ReadItems();


            // Categories
            _categories = typeof(Categorie).GetEnumNames().ToList();
            lstCategories.ItemsSource = _categories;
            
            _categoriesPref ??= new List<string>();
            //lstCategoriesPref.ItemsSource = _categoriesPref;

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
                string categorie = lstCategories.SelectedItem.ToString()?.Trim() ?? "";

                bool catToAdd = !string.IsNullOrWhiteSpace(categorie) && !_categoriesPref.Contains(categorie);
                if (catToAdd)
                {
                    _categoriesPref.Add(categorie);
                    lstCategoriesPref.Items.Add(categorie);

                    //Categorie cat = (Categorie) Enum.Parse(typeof(Categorie), categorie, true);

                    bool catAjoutee = _abo.AimeCategorie(categorie) && _dalAbonne.UpdateItem(_abo);
                    if (catAjoutee)
                    {
                        MessageBox.Show("XXX");
                    }

                    //lstCategories.SelectedIndex = -1;

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

            if (!string.IsNullOrWhiteSpace(categorie) && _categoriesPref.Contains(categorie))
            {
                _categoriesPref.Remove(categorie);
                lstCategoriesPref.Items.Remove(categorie);
                lstCategoriesPref.SelectedIndex = -1;
            }
        }
    }

    #endregion
}