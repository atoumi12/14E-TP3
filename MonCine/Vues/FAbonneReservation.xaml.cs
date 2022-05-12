using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MonCine.Data;

namespace MonCine.Vues
{
    /// <summary>
    /// Logique d'interaction pour FAbonneReservation.xaml
    /// </summary>
    public partial class FAbonneReservation : Page
    {

        private DALProjection dalProjection { get; set; }
        private DALSalle dalSalle { get; set; }
        private List<Salle> Salles { get; set; }
        private DateTime DateRecherche { get; set; }
        private Film unFilm { get; set; }
        private List<Projection> Projections { get; set; }
        private ObservableCollection<Projection> ProjectionsVoulue { get; set; }
        private Projection uneProjection { get; set; }
        private Salle uneSalle { get; set; }
        private int nbPlaceRestante { get; set; }


        public FAbonneReservation(DALProjection pDalProjection, DALSalle pDalSalle, Film unFilmChoisie)
        {
            InitializeComponent();
            unFilm = unFilmChoisie;
            dalProjection = pDalProjection;
            dalSalle = pDalSalle;
            ProjectionsVoulue = new ObservableCollection<Projection>();
            DateRecherche = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            lstProjections.ItemsSource = ProjectionsVoulue;
            InitialConfiguration(DateRecherche);
            
        }

        public string changeTextPlace_restante()
        {
            place_restante.Text = "TEST";
            return null;
        }

        private void InitialConfiguration(DateTime pDate)
        {
            DatePickerRecherche.SelectedDate = pDate;

            if(ProjectionsVoulue != null)
            {
                ProjectionsVoulue.Clear();
            }

            Projections = dalProjection.GetProjectionsByDate(pDate);
            
            foreach(Projection projection in Projections)
            {
                if( projection.Film.Id == unFilm.Id)
                {
                    ProjectionsVoulue.Add(projection);
                }
            }
            // verifier pour quel film

            
        }





        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void DatePickerRecherche_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateRecherche = DatePickerRecherche.SelectedDate.Value;
            InitialConfiguration(DateRecherche);
        }

        private void LstProjection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            uneProjection = (Projection)lstProjections.SelectedItem;
            if (uneProjection != null)
            {
                if (uneProjection.Salle.Places != null)
                {
                    nbPlaceRestante = uneProjection.Salle.Places.Count;
                    place_restante.Text = "Il reste " + nbPlaceRestante.ToString() + " places dispoonible pour la seance.";
                }
                else
                {
                    place_restante.Text = "Il n'y a pas de place disponible pour cette sceance.";
                }

            }

            //int nbPlaceRestante = uneProjection.Salle.Places.Count;
            //place_restante.Text = projection.;
        }
    }
}
