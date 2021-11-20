using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProyecto.Helper
{
    public class MongoDBRepository
    {
        public MongoClient client;
        public IMongoDatabase db;
        public MongoDBRepository()
        {
            client = new MongoClient("mongodb://127.0.0.1:27017"); // se conecta a mongo
            db = client.GetDatabase("ChatDB"); // conexión con la bd
        }
    }
}
