using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonCine.Data
{
    public class Salle
    {
        [BsonId] public ObjectId Id { get; set; }
        public int NumSale { get; set; }
        private Place Place { get; set; }


        public Salle(int pnumSale)
        {
            NumSale = pnumSale;
        }

        public override string ToString()
        {
            return $"Salle (N°{NumSale})";
        }
    }
}