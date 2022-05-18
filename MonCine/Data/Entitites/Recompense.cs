using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MonCine.Data.Entitites
{
    public class Recompense
    {
        [BsonId] public ObjectId Id { get; set; }
        public TypeRecompense Type { get; set; }

        public Film Film { get; set; }
        public Abonne Abonne { get; set; }

        public Recompense(TypeRecompense pType , Film pFilm = null, Abonne abonne = null)
        {
            Type = pType;
            Film = pFilm;
            Abonne = abonne;
        }

        public override string ToString()
        {
            // Fonction crée dans le fichier EnumHelper.cs
            return $"{Type.GetDesc()}";
        }
    }
}
