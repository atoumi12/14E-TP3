﻿using System;
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

        public FRecompenses(DALRecompense DalRecompense, DALFilm pDalFilm, DALAbonne pDalAbonne)
        {
            InitializeComponent();

            _dalFilm = pDalFilm;
            _dalAbonne = pDalAbonne;
            _dalRecompense = DalRecompense;

            InitialConfiguration();
        }

        private void InitialConfiguration()
        {
            List<Film> films = _dalFilm.ReadItems().ToList();
            films = films.Where(f => f.AdmissibleReprojection()).ToList();
            lstReprojections.ItemsSource = films;
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
            List<Abonne> abonnes = _dalAbonne.ReadItems();
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
    }
}