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
    /// Logique d'interaction pour FMenuAbonne.xaml
    /// </summary>
    public partial class FMenuAbonne : Page
    {
        Abonne CurrentUser;

        public FMenuAbonne(Abonne currentUser)
        {
            InitializeComponent();
            CurrentUser = currentUser;
            string UserName = CurrentUser.FirstName + " " + CurrentUser.LastName;
            userName.Text = UserName;
        }

        private void VoireFilm_Click(object sender, RoutedEventArgs e)
        {
            FAbonneFilms fAbonneFilms = new FAbonneFilms(new DALFilm (), CurrentUser, new DALProjection(), new DALAbonne());
           
            NavigationService?.Navigate(fAbonneFilms);
        }

        private void BtnPreference_Click(object sender, RoutedEventArgs e)
        {
            FPreferencesAbonne prf = new FPreferencesAbonne(CurrentUser ,new DALRealisateur(), new DALAbonne(), new DALActeur());
            prf.Show();
        }


        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void btn_noterFilm(object sender, RoutedEventArgs e)
        {
            FNoterFilm pnf = new FNoterFilm(CurrentUser, new DALAbonne());
            NavigationService?.Navigate(pnf);
        }
    }
}
