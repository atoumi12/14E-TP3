using System;
using System.Collections.Generic;
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
    /// Logique d'interaction pour FAbonneFilms.xaml
    /// </summary>
    public partial class FAbonneFilms : Page
    {
        private List<Film> films;
        private List<Film> filmsAlAffiche = new List<Film>();
        private DALFilm Dal { get; set; }
        private Abonne currentUser { get; set; }


        public FAbonneFilms(DALFilm dal, Abonne currentUser)
        {
            InitializeComponent();
            Dal = dal;
            films = Dal.ReadItems();
            initList();
        }

        private void initList()
        {
            foreach(Film unFilm in films)
            {
                if(unFilm.SurAffiche == true)
                {
                    filmsAlAffiche.Add(unFilm);
                }
            }
            LstFilms.ItemsSource = filmsAlAffiche;
        }


        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }


        private void LstFilms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Film unFilm = LstFilms.SelectedItem as Film;

            FAbonneReservation freserv = new FAbonneReservation(new DALProjection(),new DALSalle(), unFilm);
            NavigationService?.Navigate(freserv);
        }
    }
}