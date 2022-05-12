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
        public List<Place> Places { get; set; }


        public Salle(int pnumSale, List<Place> pPlaces)
        {
            Places = pPlaces;
            NumSale = pnumSale;
        }

        public override string ToString()
        {
            return $"Salle (N°{NumSale})";
        }
    }
}