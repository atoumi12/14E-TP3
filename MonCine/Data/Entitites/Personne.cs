using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MonCine.Data
{
    public class Personne
    {
        [BsonId] public ObjectId Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }


        public Personne()
        {
        }

        public Personne(string pFirstName, string pLastName)
        {
            FirstName = pFirstName;
            LastName = pLastName;
        }

        public Personne(string pFirstName, string pLastName, int pAge)
        {
            FirstName = pFirstName;
            LastName = pLastName;
            Age = pAge;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}