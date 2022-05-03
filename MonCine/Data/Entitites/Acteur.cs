using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace MonCine.Data
{
    public class Acteur : Personne
    {
        public List<Film> films { get; set; }

        public Acteur(string pFirstName, string pLastName):base(pFirstName, pLastName)
        {
            
        }
    }
}
