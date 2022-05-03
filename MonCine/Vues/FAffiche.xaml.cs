using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for FAffiche.xaml
    /// </summary>
    public partial class FAffiche : Page
    {
        private DALProjection dalProjection { get; set; }
        private DALSalle dalSalle { get; set; }
        private List<Salle> Salles { get; set; }
        private DateTime DateRecherche { get; set; }

        public FAffiche(DALProjection pDalProjection, DALSalle pDalSalle)
        {
            InitializeComponent();

            dalProjection = pDalProjection;
            dalSalle = pDalSalle;

            // Date d'ajourd'hui au premier affichage du formulaire
            DateRecherche = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            InitialConfiguration(DateRecherche);
        }


        private void InitialConfiguration(DateTime pDate)
        {
            GroupBoxInformationsProjection.Visibility = Visibility.Hidden;

            DatePickerRecherche.SelectedDate = pDate;
            // Projection
            List<Projection> Projections = dalProjection.GetProjectionsByDate(pDate);
            lstProjectionsAffiche.ItemsSource = Projections;
            if (Projections.Count > 0)
            {
                StackPanelAffiche.Visibility = Visibility.Visible;
                txtAcuneProjection.Visibility = Visibility.Hidden;
            }
            else
            {
                txtAcuneProjection.Visibility = Visibility.Visible;
                StackPanelAffiche.Visibility = Visibility.Hidden;
            }

            // Salles
            Salles = dalSalle.ReadItems();
            if (ComboBoxSalles.Items.Count <= 0)
            {
                Salles.ForEach(salle => ComboBoxSalles.Items.Add(salle));
            }
        }


        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }


        private void btnRetirerProjection_Click(object sender, RoutedEventArgs e)
        {
            Projection projection = lstProjectionsAffiche.SelectedItem as Projection;

            if (projection is null)
            {
                MessageBox.Show("Veuillez sélectionner une projection programmée pour la retirer",
                    "Suppression de projection", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (MessageBox.Show("Êtes-vous sur de vouloir retirer la projection ?", "Suppression de la projection",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    bool result = dalProjection.DeleteItem(projection);
                    if (result)
                    {
                        lstProjectionsAffiche.SelectedIndex = -1;
                        MessageBox.Show("La projection a été supprimée avec succès!", "Suppression de projection",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        InitialConfiguration(DateRecherche);
                    }
                }
            }
        }


        private void btnModifierProjection_Click(object sender, RoutedEventArgs e)
        {
            var (valide, erreurs) = ValidateProjection();

            Projection projection = lstProjectionsAffiche.SelectedItem as Projection;

            if (projection != null && valide)
            {
                projection.Salle = ComboBoxSalles.SelectedItem as Salle;
                projection.DateDebut = DatePickerDateProjectionFilm.SelectedDate.Value;

                bool result = dalProjection.UpdateItem(projection);
                if (result)
                {
                    MessageBox.Show($"La projection a été modifié avec succès", "Modification", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    InitialConfiguration(DateRecherche);
                }
            }
            else if (!valide)
            {
                MessageBox.Show($"Veuillez corriger le(s) erreur(s) suivante(s) : \n\n {erreurs}", "Erreur de modification de projection", MessageBoxButton.OK, MessageBoxImage.Error );
            }
        }


        private void lstProjectionsAffiche_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Projection projection = lstProjectionsAffiche.SelectedItem as Projection;

            if (projection != null)
            {
                GroupBoxInformationsProjection.Visibility = Visibility.Visible;

                txtNomFilm.Text = projection.Film.Name;
                DatePickerDateProjectionFilm.SelectedDate = projection.DateDebut;

                Salle salle = Salles.Where(s => s.Id == projection.Salle.Id).ToList()[0];
                ComboBoxSalles.SelectedIndex = Salles.IndexOf(salle);
            }
            else
            {
                GroupBoxInformationsProjection.Visibility = Visibility.Hidden;
            }
        }

        private void DatePickerRecherche_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateRecherche = DatePickerRecherche.SelectedDate.Value;
            InitialConfiguration(DateRecherche);
        }



        /// <summary>
        /// Permet de valider les données contenues dans les champs de la projection
        /// </summary>
        /// <returns>True si le format de données est correct, sinon False</returns>
        private (bool, string) ValidateProjection()
        {
            bool valide = true;
            string erreurs = "";

            if (ComboBoxSalles.SelectedIndex == -1)
            {
                erreurs += " - Veuillez choisir une salle pour la projection \n";
            }

            if (DatePickerDateProjectionFilm.SelectedDate == null)
            {
                erreurs += " - Veuillez choisir une date de projection valide \n";
            }


            if (!string.IsNullOrWhiteSpace(erreurs))
            {
                valide = false;
            }

            return (valide, erreurs);
        }
    }
}