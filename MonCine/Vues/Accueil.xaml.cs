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
    /// Interaction logic for Accueil.xaml
    /// </summary>
    public partial class Accueil : Page
    {
        public Accueil()
        {
            InitializeComponent();
        }

        private void btnAdmin_Click(object sender, RoutedEventArgs e)
        {
            FAdministrateur fadmin = new FAdministrateur();
            NavigationService?.Navigate(fadmin);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FPreferencesAbonne prf = new FPreferencesAbonne(new DALRealisateur(), new DALAbonne(), new DALActeur());
            prf.Show();
        }
    }
}
