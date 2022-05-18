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
using System.Windows.Shapes;
using MonCine.Data;


namespace MonCine.Vues
{
    /// <summary>
    /// Logique d'interaction pour Projections.xaml
    /// </summary>
    public partial class FProjections : Page
    {
        private DALProjection DalProjection { get; set; }
        private DALFilm DalFilm { get; set; }
        private DALSalle DalSalle { get; set; }

        private List<Film> Films = new List<Film>();
        private List<Salle> Salles = new List<Salle>();


        public FProjections(DALFilm pDalFilm, DALSalle pDalSalle, DALProjection pDalProjection)
        {
            InitializeComponent();
            DalProjection = pDalProjection;
            DalFilm = pDalFilm;
            DalSalle = pDalSalle;
            InitialConfiguration();
        }

        /// <summary>
        /// Définit l'état inital du form
        /// </summary>
        private void InitialConfiguration()
        {
            Films = DalFilm.ReadItems();
            Salles = DalSalle.ReadItems();
            PopulateComboBoxes();
        }

        private void PopulateComboBoxes()
        {
            foreach (Film film in Films)
            {
                FilmCombobox.Items.Add(film);
            }

            foreach (Salle salle in Salles)
            {
                SalleCombobox.Items.Add(salle);
            }
        }

        private Projection CreateProjectionToAdd()
        {
            Salle salle = (Salle)SalleCombobox.SelectedItem;
            Film film = (Film)FilmCombobox.SelectedItem;
            DateTime date = DatePickerProjection.SelectedDate.Value;

            film.SurAffiche = true;
            FilmCombobox.SelectedIndex = -1;
            SalleCombobox.SelectedIndex = -1;
            DatePickerProjection.SelectedDate = null;

            Projection projection = new Projection(salle, film, date);

            return projection;
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool champsRemplis = FilmCombobox.SelectedIndex != -1 && SalleCombobox.SelectedIndex != -1 &&
                                 DatePickerProjection.SelectedDate != null;
            bool dateAnterieure = DatePickerProjection.SelectedDate < DateTime.Now;

            if (!champsRemplis)
            {
                MessageBox.Show("Veuillez remplir les champs nécéssaires pour créer la projection",
                    "Ajout de Projection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (dateAnterieure)
            {
                MessageBox.Show(
                    $"Veuillez choisir une date dans le futur pour la projection. \n La date saisie : {DatePickerProjection.SelectedDate.Value.ToShortDateString()} n'est pas valide",
                    "Ajout de Projection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Projection projection = CreateProjectionToAdd();

                // Affecter la date de la projection au film concernée
                var resultFilm = DalFilm.AddProjectionDate(projection);
                bool resultProjection = false;
                if (resultFilm)
                {
                    // Créer la projection
                    resultProjection = DalProjection.AddItem(projection);
                }

                if (resultProjection && resultFilm)
                {
                    MessageBox.Show($"La projection a été crée avec succès !", "Création de projection",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(
                        "Erreur lors de la création de la projection.\nVeuillez programmer une reprojection si admissible",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}