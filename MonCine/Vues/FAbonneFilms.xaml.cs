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
        private DALProjection DalProjection { get; set; }
        private DALAbonne DalAbonne { get; set; }
        private Abonne CurrentUser { get; set; }
        private List<Projection> projections;
        private Projection uneProjection { get; set; }
        private int nbPlaceRestante { get; set; }


        public FAbonneFilms(DALFilm dal, Abonne currentUser, DALProjection dalProjection, DALAbonne dalAbonne)
        {
            InitializeComponent();
            Dal = dal;
            DalProjection = dalProjection;
            DalAbonne = dalAbonne;

            films = Dal.ReadItems();

            projections = new List<Projection>();


            CurrentUser = currentUser;
            initList();
        }

        private void initList()
        {
            foreach (Film unFilm in films)
            {
                if (unFilm.SurAffiche)
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
            grpSeances.Visibility = Visibility.Visible;
            Film unFilm = LstFilms.SelectedItem as Film;

            projections = DalProjection.GetProjectionsOfFilm(unFilm);
            LstProjections.ItemsSource = projections;


        }

        private void LstProjections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uneProjection = (Projection)LstProjections.SelectedItem;

            AfficherPlaceRestante(uneProjection);

            if (uneProjection != null && uneProjection.Salle.Places.Count > 0)
            {
                btn_reservation.IsEnabled = true;
            }
        }
        public void GestionBtnReservation()
        {
            LstProjections.SelectedItem = null;
            btn_reservation.IsEnabled = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<Projection> reservationsDeLAbonne = new List<Projection>();
            if (CurrentUser.Reservations != null)
            {
                reservationsDeLAbonne = CurrentUser.Reservations;

            }
            if (!reservationsDeLAbonne.Contains(uneProjection))
            {
                CurrentUser.Reservations.Add(uneProjection);
                DalAbonne.UpdateItem(CurrentUser);


                Place unePlace = uneProjection.Salle.Places[0];
                uneProjection.Salle.Places.Remove(unePlace);
                AfficherPlaceRestante(uneProjection);

                DalProjection.UpdateItem(uneProjection);
                GestionBtnReservation();

                MessageBox.Show(
                "Votre reservation a bien été effectuée", "Réservation d'une séance", MessageBoxButton.OK,
                MessageBoxImage.Information);

                grpSeances.Visibility = Visibility.Hidden;

            }
            else
            {
                MessageBox.Show(
                "Vous avez déjà une réservation pour cette projection", "Réservation d'une séance", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            }


        }
        private void AfficherPlaceRestante(Projection uneProjection)
        {
            if (uneProjection != null)
            {
                if (uneProjection.Salle.Places != null)
                {
                    nbPlaceRestante = uneProjection.Salle.Places.Count;
                    place_restante.Text = "Il reste " + nbPlaceRestante + " place(s) disponible(s) pour la séance.";
                }
                else
                {
                    place_restante.Text = "Il n'y a pas de places disponibles pour cette séance.";
                }

            }
        }
    }
}