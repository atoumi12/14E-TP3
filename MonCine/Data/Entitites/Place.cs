using System;
using System.Collections.Generic;
using System.Text;

namespace MonCine.Data
{
    class Place
    {
        private int Numero { get; set; }
        private bool Disponible { get; set; }

        public Place(int pNumero)
        {
            Numero = pNumero;
            Disponible = true;
        }
    }
}