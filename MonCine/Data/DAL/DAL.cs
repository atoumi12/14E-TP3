using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using System.Windows;
using System.Xaml;

namespace MonCine.Data
{
    public abstract class DAL
    {
        protected IMongoClient mongoDBClient;
        protected IMongoDatabase database;

        public DAL(IMongoClient client = null)
        {
            mongoDBClient = client ?? OuvrirConnexion();
            database = ConnectDatabase();
        }

        private IMongoClient OuvrirConnexion()
        {
            MongoClient dbClient = null;
            try
            {
                dbClient = new MongoClient("mongodb://localhost:27017/");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible de se connecter à la base de données " + ex.Message, "Erreur");
            }

            return dbClient;
        }

        private IMongoDatabase ConnectDatabase()
        {
            IMongoDatabase db = null;
            try
            {
                db = mongoDBClient.GetDatabase("TP2DB");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Impossible de se connecter à la base de données " + ex.Message, "Erreur");
            }

            return db;
        }
    }
}