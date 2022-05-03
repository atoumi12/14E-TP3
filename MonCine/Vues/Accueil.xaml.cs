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
    /// Logique d'interaction pour Accueil.xaml
    /// </summary>
    public partial class Accueil : Page
    {
        public Accueil()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FAbonnes frmAbonnes = new FAbonnes(new DALAbonne());

            NavigationService?.Navigate(frmAbonnes);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FProjections frmProjections = new FProjections(new DALFilm(), new DALSalle(), new DALProjection());

            NavigationService?.Navigate(frmProjections);
        }

        private void BtnFilm_Click(object sender, RoutedEventArgs e)
        {
            FFilms frmFilms = new FFilms(new DALFilm(),  new DALActeur(), new DALRealisateur(), new DALProjection());
            NavigationService?.Navigate(frmFilms);
        }

        private void BtnAffiche_Click(object sender, RoutedEventArgs e)
        {
            FAffiche frmAffiche = new FAffiche(new DALProjection(), new DALSalle());
            NavigationService?.Navigate(frmAffiche);
        }
    }
}
