using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace MonCine.Data
{
    public class Projection
    {
        public ObjectId Id { get; set; }
        public Salle Salle { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public Film Film { get; set; }


        public Projection(Salle salle, Film film, DateTime pDate)
        {
            Salle = salle;
            Film = film;
            DateDebut = pDate;
        }

        public override string ToString()
        {
            return $"{Film.Name} - {Salle} - {DateDebut.ToShortDateString()}";
        }
    }
}
