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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MonCine.Data;

namespace MonCine.Vues
{
    /// <summary>
    /// Interaction logic for FProjectionFilm.xaml
    /// </summary>
    public partial class FProjectionFilm : Window
    {
        private DALProjection dalProjection { get; set; }
        private List<Projection> projections { get; set; }
        private Film FilmChoisi { get; set; } 


        public FProjectionFilm(DALProjection pDalProjection, Film pFilm)
        {
            InitializeComponent();

            dalProjection = pDalProjection;
            FilmChoisi = pFilm;

            InitialConfiguration();
        }

        private void InitialConfiguration()
        {
            txtFilmName.Text = $"{FilmChoisi.Name}";

            // TODO: Appeler la methode du film directement
            projections = dalProjection.GetProjectionsOfFilm(FilmChoisi);

            if (projections.Count == 0)
            {
                txtLstProjectionsCount.Text = "Aucune projection programmée pour l'instant ! \nVeuillez en ajoutez.";
                ProjectionsListView.Visibility = Visibility.Hidden;
            }
            else
            {
                txtLstProjectionsCount.Text = "";
                ProjectionsListView.Visibility = Visibility.Visible;
                ProjectionsListView.ItemsSource = projections;
            }


        }
    }
}