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
        private Film filmNoter;
        private int note;


        public FNoterFilm(Abonne user, DALAbonne dalAbonne)
        {
            InitializeComponent();
            currentUser = user;
            filmsVueParAbo = new List<Film>();
            slider_note.IsEnabled = false;
            btn_noterFilm.IsEnabled = false;
            DALabonne = dalAbonne;
            note = 0;
            
            initList();
        }

        private void initList()
        {
            foreach (Projection reservation in currentUser.Reservations)
            {
                if (reservation.DateDebut < DateTime.Now)
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
                slider_note.IsEnabled = true;
            }
            else
            {
                slider_note.IsEnabled = false;
                btn_noterFilm.IsEnabled = false;
            }

            
        }

        private void btn_noterFilm_Click(object sender, RoutedEventArgs e)
        {
            DALFilm dalFilm = new DALFilm();
            filmNoter.Notes.Add(note);
            filmNoter.NoteMoyenne = filmNoter.CalculerMoyennesNotes();
            dalFilm.UpdateItem(filmNoter);
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
