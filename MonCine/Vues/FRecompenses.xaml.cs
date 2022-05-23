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
using MonCine.Data.Entitites;

namespace MonCine.Vues
{
    /// <summary>
    /// Interaction logic for FRecompenses.xaml
    /// </summary>
    public partial class FRecompenses : Page
    {
        private TypeRecompense _modeRecompense { get; set; }
        private DALRecompense _dalRecompense { get; set; }
        private DALFilm _dalFilm { get; set; }
        private DALAbonne _dalAbonne { get; set; }
        private DALProjection _dalProjection { get; set; }
        List<Abonne> abonnes { get; set; }
        List<Abonne> lstAbonnes { get; set; }
        private List<Abonne> lstAbonnesValide { get; set; }

        public FRecompenses(DALRecompense DalRecompense, DALFilm pDalFilm, DALAbonne pDalAbonne, DALProjection pDalProjection)
        {
            InitializeComponent();

            _dalFilm = pDalFilm;
            _dalAbonne = pDalAbonne;
            _dalRecompense = DalRecompense;
            _dalProjection = pDalProjection;

            abonnes = new List<Abonne>();
            lstAbonnes = new List<Abonne>();
            lstAbonnesValide = new List<Abonne>();


            InitialConfiguration();
        }

        private void InitialConfiguration()
        {
            List<Film> films = _dalFilm.ReadItems().ToList();
            films = films.Where(f => f.AdmissibleReprojection()).ToList();
            lstReprojections.ItemsSource = films;

            List<Film> filmsAvantPremiere = _dalFilm.ReadItems().ToList();
            filmsAvantPremiere = filmsAvantPremiere.Where(f => f.AdmissibleAvantPremiere()).ToList();
            lstAvantPremiere.ItemsSource = filmsAvantPremiere;

            lstAbonnes = _dalAbonne.ReadItems().ToList();

            lstAbonnesAvantPremiere.ItemsSource = lstAbonnesValide;
        }

        private void ModeUpdate()
        {
            grbReprojection.Visibility =
                _modeRecompense == TypeRecompense.Reprojection ? Visibility.Visible : Visibility.Hidden;
            grbAvantPremiere.Visibility = _modeRecompense == TypeRecompense.AvantPremiere
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void optReprojection_Checked(object sender, RoutedEventArgs e)
        {
            _modeRecompense = TypeRecompense.Reprojection;
            ModeUpdate();
        }

        private void optAvantPremiere_Checked(object sender, RoutedEventArgs e)
        {
            _modeRecompense = TypeRecompense.AvantPremiere;
            ModeUpdate();
        }

        private void lstReprojections_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstReprojections.SelectedIndex != -1)
            {
                Film film = lstReprojections.SelectedItem as Film;
                lstAbonnesReprojection.ItemsSource = getAbonnes(film);
            }

        }

        /// <summary>
        /// Récupère les abonnés concernés par le type de récompense
        /// </summary>
        /// <param name="pFilm">Le film en question</param>
        /// <returns>Liste des abonnés</returns>
        private List<Abonne> getAbonnes(Film pFilm)
        {
            abonnes = _dalAbonne.ReadItems();
            if (abonnes.Count < 1)
            {
                lstAbonnesReprojection.Visibility = Visibility.Hidden;
                txtAucunAbonneReprojection.Visibility = Visibility.Visible;
                txtAucunAbonneReprojection.Text = "Aucun abonné enregistré !";
                // Retourne une liste vide pour éviter un beug
                return new List<Abonne>();
            }

            List<Abonne> abonnesRecompenses = new List<Abonne>();
            switch (_modeRecompense)
            {
                case TypeRecompense.Reprojection:
                    abonnes.ForEach(abonne =>
                    {
                        if (pFilm.Recompenses_AbonneAdmissibleReProjection(abonne) && _dalRecompense.AbonneAdmissibleRecompense(_modeRecompense, abonne,pFilm))
                        {
                            abonnesRecompenses.Add(abonne);
                        }
                    });
                    break;
            }

            return abonnesRecompenses;
        }


        private void OffirRecompense_Click(object sender, RoutedEventArgs e)
        {
            Abonne abonne = lstAbonnesReprojection.SelectedItem as Abonne;
            if (abonne is null)
            {
                MessageBox.Show("Veuillez séléctionnez un abonné", "Récompense", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Film film = lstReprojections.SelectedItem as Film;
            if (film is null)
            {
                MessageBox.Show("Veuillez séléctionnez un film", "Récompense", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Recompense recompense = new Recompense(_modeRecompense);
            bool result = _dalRecompense.AssignerRecompense(recompense, abonne, film);

            if (result)
            {
                MessageBox.Show($"Récompense ajouté avec succès à l'abonné {abonne} !", "Ajout de récompense", MessageBoxButton.OK, MessageBoxImage.Information);
                lstAbonnesReprojection.ItemsSource = getAbonnes(film);
            }
            else
            {
                MessageBox.Show($"Erreur lors de l'attribution de récompense.",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void lstAvantPremiere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Film avantPremiere = lstAvantPremiere.SelectedItem as Film;
            Boolean estValide = false;

            lstAbonnesValide = lstAbonnes.Where(a => a.AdmissibleAbo(avantPremiere)).ToList();
            lstAbonnesAvantPremiere.ItemsSource = lstAbonnesValide;
            lstAbonnesAvantPremiere.Items.Refresh();
            List<Projection> lstProj = _dalProjection.ReadItems().ToList();
            lstProj = lstProj.Where(x => x.Film.Id == avantPremiere.Id).ToList();
            if (lstProj.Count > 0)
            {
                int nbPlace = lstProj[0].Salle.Places.Count();
                int index = 0;
                while (index < nbPlace && index < lstAbonnesValide.Count())
                {
                    lstAbonnesAvantPremiere.SelectedItems.Add(lstAbonnesAvantPremiere.Items[index]);
                    index++;
                }
            }
            


        }


        private void lstAbonnesAvantPremiere_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OffirRecompenseAvantPremiere_Click(object sender, RoutedEventArgs e)

        {
            lstAbonnes = lstAbonnesAvantPremiere.SelectedItems.OfType<Abonne>().ToList();
            //foreach (Abonne abonne in lstAbonnesAvantPremiere.SelectedItems.Cast<Abonne>.toList())
            //{
            //    lstAbonnes.Add(abonne);
            //}
            //lstAbonnes = lstAbonnesAvantPremiere.SelectedItems as List<Abonne>;
            if (lstAbonnes.Count == 0 )
            {
                MessageBox.Show("Veuillez séléctionnez un abonné", "Récompense", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Film film = lstAvantPremiere.SelectedItem as Film;
            if (film is null)
            {
                MessageBox.Show("Veuillez séléctionnez un film", "Récompense", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            Recompense recompense = new Recompense(_modeRecompense);
            bool result = _dalRecompense.AssignerRecompenseAPlusieursAbonnes(recompense, lstAbonnes, film);

            if (result)
            {
                MessageBox.Show($"Récompense ajouté avec succès des ab !", "Ajout de récompense", MessageBoxButton.OK, MessageBoxImage.Information);
                lstAbonnesReprojection.ItemsSource = getAbonnes(film);
            }
            else
            {
                MessageBox.Show($"Erreur lors de l'attribution de récompense.",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}