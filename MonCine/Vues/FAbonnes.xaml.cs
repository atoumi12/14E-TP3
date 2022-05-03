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
    /// Logique d'interaction pour FAbonnes.xaml
    /// </summary>
    public partial class FAbonnes : Page
    {
        private List<Abonne> abonnes;
        private DALAbonne Dal { get; set; }


        public FAbonnes(DALAbonne dal)
        {
            InitializeComponent();
            Dal = dal;
            abonnes = Dal.ReadItems();
            LstAbonnes.ItemsSource = abonnes;

        }


        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.Navigate(new Accueil());
        }


        private void LstAbonnes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Abonne unAbonne = LstAbonnes.SelectedItem as Abonne;

            FAbonne fabonne = new FAbonne(unAbonne);
            NavigationService?.Navigate(fabonne);
        }



    }
}