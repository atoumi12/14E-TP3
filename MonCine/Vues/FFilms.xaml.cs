using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour Films.xaml
    /// </summary>
    public partial class FFilms : Page
    {
        private List<Film> Films { get; set; }
        private List<Acteur> Acteurs { get; set; }
        private List<Realisateur> Realisateurs { get; set; }
        private List<String> Categories { get; set; }

        public ObservableCollection<Acteur> ActeursDansLeFilm { get; set; }
        public ObservableCollection<Realisateur> RealisateursDansLeFilm { get; set; }
        public ObservableCollection<String> CategoriesDansLeFilm { get; set; }


        private DALFilm _dalFilm { get; set; }
        private DALActeur _dalActeur { get; set; }
        private DALRealisateur _dalRealisateur { get; set; }
        private DALProjection _dalProjection { get; set; }


        int indexOfMovieSelected;




        public FFilms(DALFilm pDalFilm, DALActeur pDalActeur, DALRealisateur pDalRealisateur,
            DALProjection pDalProjection)
        {
            _dalFilm = pDalFilm;
            _dalActeur = pDalActeur;
            _dalRealisateur = pDalRealisateur;
            _dalProjection = pDalProjection;

            InitializeComponent();

            InitialItemConfiguration();
        }

        /// <summary>
        /// Définit l'état inital du form
        /// </summary>
        private void InitialItemConfiguration()
        {
            InitialiseListView();

            BtnDelete.IsEnabled = false;
            BtnUpdate.IsEnabled = false;
            BtnAfficherProjections.IsEnabled = false;
        }


        private void InitialiseListView()
        {
            Films = _dalFilm.ReadItems();
            Acteurs = _dalActeur.ReadItems();
            Realisateurs = _dalRealisateur.ReadItems();
            Categories = typeof(Categorie).GetEnumNames().ToList();

            LstFilms.ItemsSource = Films;
            LstActeurs.ItemsSource = Acteurs;
            LstRealisateurs.ItemsSource = Realisateurs;
            LstCategories.ItemsSource = Categories;

            ActeursDansLeFilm = new ObservableCollection<Acteur>();
            RealisateursDansLeFilm = new ObservableCollection<Realisateur>();
            CategoriesDansLeFilm = new ObservableCollection<String>();

            LstActeursInTheMovie.ItemsSource = ActeursDansLeFilm;
            LstRealisateursInTheMovie.ItemsSource = RealisateursDansLeFilm;
            LstCategorieInTheMovie.ItemsSource = CategoriesDansLeFilm;

            LstFilms.SelectedIndex = -1;

        }




        // GESTION DES BTN ////////////////


        /// <summary>
        /// Permet de retourn à l'accueil.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new Accueil());
        }


        /// <summary>
        /// Permet la création d'un film
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var (valide, erreurs) = FilmIsValid();

            if (!valide)
            {
                MessageBox.Show($"Veuillez remplir les champs nécéssaires \n\n\n{erreurs}", "Création d'un film",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                Film film = GetFilmFromValues();
                var result = _dalFilm.AddItem(film);
                if (result)
                {
                    MessageBox.Show($"Le film '{film.Name}' a été crée avec succès !", "Création de film",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            ClearLisView();
            InitialItemConfiguration();
        }


        /// <summary>
        /// Permet la mise à jour d'un film
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (LstFilms.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez choisir un film pour le modifier", "Modification",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                var (valide, erreurs) = FilmIsValid();

                if (!valide)
                {
                    MessageBox.Show($"Veuillez remplir les champs nécéssaires \n\n\n{erreurs}",
                        "Modification d'un film", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                {
                    Film selectedFilm = (Film)LstFilms.SelectedItem;
                    Film updatedFilm = GetFilmFromValues();
                    updatedFilm.Id = selectedFilm.Id;

                    var result = _dalFilm.UpdateItem(updatedFilm);

                    if (result)
                    {
                        MessageBox.Show($"Le film '{updatedFilm.Name}' a été mis à jour avec succès !", "Modification",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            InitialItemConfiguration();
        }


        /// <summary>
        /// Permet la suppression d'un film
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (LstFilms.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez choisir un film pour le supprimer", "Suppression",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Film film = (Film)LstFilms.SelectedItem;
                var result = _dalFilm.DeleteItem(film);

                if (result)
                {
                    NameField.Text = "";
                    //RefreshItems();
                    MessageBox.Show($"Le film '{film.Name}' a été supprimé avec succès !", "Suppression",
                        MessageBoxButton.OK, MessageBoxImage.None);
                }
            }
            ClearLisView();
            InitialItemConfiguration();
        }


        /// <summary>
        /// Accède à la fenêtre des projections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAfficherProjections_Click(object sender, RoutedEventArgs e)
        {
            Film film = LstFilms.SelectedItem as Film;
            if (film is null)
            {
                MessageBox.Show("Veuillez sélectionnez un film", "Affichage de projection", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                FProjectionFilm projectionFilm = new FProjectionFilm(_dalProjection, film);
                projectionFilm.Show();
            }
        }




        // GESTION DES LISTES ////////////////


        private void LstFilms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstFilms.SelectedIndex == -1)
            {
                ClearLisView();
            }

            Film film = (Film)LstFilms.SelectedItem;

            if (film != null)
            {
                NameField.Text = film.Name;
                ChargementDataDuFilm(film);
            }

            if (LstFilms.SelectedIndex != -1)
            {
                indexOfMovieSelected = LstFilms.SelectedIndex;
            }

            BtnDelete.IsEnabled = film != null;
            BtnUpdate.IsEnabled = film != null;
            BtnAfficherProjections.IsEnabled = film != null;
        }


        private void LstRealisateurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Realisateur realisateur = (Realisateur)LstRealisateurs.SelectedItem;
            if (!RealisateursDansLeFilm.Contains(realisateur) && realisateur != null)
            {
                RealisateursDansLeFilm.Add(realisateur);
            }
            LstRealisateurs.SelectedIndex = -1;
        }

        private void LstActeurs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Acteur acteur = (Acteur)LstActeurs.SelectedItem;
            if (!ActeursDansLeFilm.Contains(acteur) && acteur != null)
            {
                ActeursDansLeFilm.Add(acteur);
            }
           
            LstActeurs.SelectedIndex = -1;
        }

        private void LstCategorie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String categorie = (String)LstCategories.SelectedItem;
            if (!CategoriesDansLeFilm.Contains(categorie) && categorie != null)
            {
                CategoriesDansLeFilm.Add(categorie);
            }
            
            LstCategories.SelectedIndex = -1;
        }

        private void LstCategorieInTheMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String categorie = (String)LstCategorieInTheMovie.SelectedItem;
            CategoriesDansLeFilm.Remove(categorie);
        }

        private void LstActeursInTheMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Acteur acteur = (Acteur)LstActeursInTheMovie.SelectedItem;
            ActeursDansLeFilm.Remove(acteur);
        }

        private void LstRealisateursInTheMovie_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Realisateur realisateur = (Realisateur)LstRealisateursInTheMovie.SelectedItem;
            RealisateursDansLeFilm.Remove(realisateur);
        }




        // UTILITAIIRE ////////////////

        private void ChargementDataDuFilm(Film _film)
        {
            ClearLisView();
            foreach (Acteur acteur in _film.Acteurs)
            {
                ActeursDansLeFilm.Add(acteur);
            }

            foreach (Realisateur realisateur in _film.Realisateurs)
            {
                RealisateursDansLeFilm.Add(realisateur);
            }

            foreach (Categorie categorie in _film.Categories)
            {
                CategoriesDansLeFilm.Add(categorie.ToString());
            }

        }



        private void ClearLisView()
        {
            ActeursDansLeFilm.Clear();
            RealisateursDansLeFilm.Clear();
            CategoriesDansLeFilm.Clear();

        }




        /// <summary>
        /// Permet de créer un objet film à partir des valeurs saisies dans les champs
        /// </summary>
        /// <returns>Film avec les valeurs saisies</returns>
        private Film GetFilmFromValues()
        {
            // Recuperer les champs
            string nom = NameField.Text;


            List<Categorie> categories = TranformeEnumToList(CategoriesDansLeFilm);

            //ClearFields();

            Film film = new Film(nom,
                categories,
                ActeursDansLeFilm.ToList(),
                RealisateursDansLeFilm.ToList());

            return film;
        }



        private List<Categorie> TranformeEnumToList(ObservableCollection<String> _CategoriesDansLeFilm)
        {
            List<Categorie> categories = new List<Categorie>();
            foreach (String cat in _CategoriesDansLeFilm)
            {
                Categorie uneCat;
                if (Enum.TryParse<Categorie>(cat, true, out uneCat))
                {
                    categories.Add(uneCat);
                }
            }
            return categories;
        }



        /// <summary>
        /// Permet de valider les données insérées dans les champs d'un film
        /// </summary>
        /// <returns></returns>
        private (bool, string) FilmIsValid()
        {
            bool valide = true;
            string erreurs = "";

            // Nom
            if (NameField.Text.Length == 0)
            {
                erreurs += " - Veuillez remplir le nom du film \n";
            }

            // CATEGORIES
            List<Categorie> categories = TranformeEnumToList(CategoriesDansLeFilm);
            if (categories.Count <= 0)
            {
                erreurs += " - Veuillez choisir au moins une catégorie du film \n";
            }



            // ACTEURS
            if (ActeursDansLeFilm.Count <= 0)
            {
                erreurs += " - Veuillez choisir au moins un acteur pour le film \n";
            }


            // REALISATEUR
            if (RealisateursDansLeFilm.Count <= 0)
            {
                erreurs += " - Veuillez choisir au moins un réalisateur pour le film \n";
            }

            if (!string.IsNullOrWhiteSpace(erreurs))
            {
                valide = false;
            }

            return (valide, erreurs);
        }
    }
}