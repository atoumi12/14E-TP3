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
    /// Logique d'interaction pour FNoterFilm.xaml
    /// </summary>
    public partial class FNoterFilm : Page
    {
        private List<Film> filmsVueParAbo;
        private Abonne currentUser;
        private DALAbonne DALabonne;
        private DALProjection DALprojection;
        private Film filmNoter;
        private int note;


        public FNoterFilm(Abonne user, DALAbonne dalAbonne, DALProjection dalProjection)
        {
            InitializeComponent();
            currentUser = user;
            filmsVueParAbo = new List<Film>();
        
            DALabonne = dalAbonne;
            DALprojection = dalProjection;


            initList();
        }

        private void initList()
        {
            foreach (Projection reservation in currentUser.Reservations)
            {
                if (reservation.DateDebut < DateTime.Now && reservation.Note == false)
                {
                    filmsVueParAbo.Add(reservation.Film);
                }
            }

            LstFilms.ItemsSource = filmsVueParAbo;
        }

        private void BtnReturn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void LstFilms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(LstFilms.SelectedItem != null)
            {
                filmNoter = (Film)LstFilms.SelectedItem;
                sectionNote.Visibility = Visibility.Visible;
                note = 0;

            }
            else
            {
                sectionNote.Visibility = Visibility.Hidden;
            }

          
        }

        private void btn_noterFilm_Click(object sender, RoutedEventArgs e)
        {
            DALFilm dalFilm = new DALFilm();

            filmNoter.Notes ??= new List<int>();
            filmNoter.Notes.Add(note);
            filmNoter.NoteMoyenne = filmNoter.CalculerMoyennesNotes();
       
            bool res = dalFilm.UpdateItem(filmNoter);
            if (res)
            {
                currentUser.Reservations.ForEach(r => {
                    if (r.Film == filmNoter)
                    {
                        r.Note = true;
                        DALabonne.UpdateItem(currentUser);
                    }
                });


                MessageBox.Show("La note a été attribuée avec succès !" , "Noter un film", MessageBoxButton.OK, MessageBoxImage.Information);
                LstFilms.SelectedIndex = -1;

                filmsVueParAbo.Remove(filmNoter);
                LstFilms.Items.Refresh();
            }

        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            note = (int)slider_note.Value;
            string strNote = note.ToString();
            txt_note.Text = "Note : " + strNote;
            btn_noterFilm.IsEnabled = true;
        }
    }
}
