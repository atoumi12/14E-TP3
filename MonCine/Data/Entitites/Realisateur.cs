using System;
using System.Collections.Generic;
using System.Text;

namespace MonCine.Data
{
    public class Realisateur : Personne
    {
        public List<Film> films { get; set; }

        public Realisateur(string pFirstName, string pLastName) : base(pFirstName, pLastName)
        {
        }
    }
}