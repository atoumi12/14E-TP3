using MonCine.Data;
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

namespace MonCine.Vues
{
    /// <summary>
    /// Logique d'interaction pour FChoixAbonne.xaml
    /// </summary>
    public partial class FChoixAbonne : Page
    {
        DALAbonne dalAbonne { get; set; }
        List<Abonne> lstAboneesSource { get; set; }
        Abonne currentUser { get; set; }

        public FChoixAbonne(DALAbonne dalabo)
        {
            dalAbonne = dalabo;
            InitializeComponent();
            lstAboneesSource = dalAbonne.ReadItems();
            LstAbonnes.ItemsSource = lstAboneesSource;
        }

        private void LstAbonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentUser = (Abonne)LstAbonnes.SelectedItem;
            FMenuAbonne famenu = new FMenuAbonne(currentUser);
            NavigationService?.Navigate(famenu);
        }

        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
