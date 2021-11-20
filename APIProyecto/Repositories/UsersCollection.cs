using APIProyecto.Helper;
using APIProyecto.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProyecto.Repositories
{
    public class UsersCollection : IUsersCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository(); // solo esta clase puede ver mongo 
        private IMongoCollection<User> Collection; // tendrá la coleccion

        public UsersCollection()
        {
            Collection = _repository.db.GetCollection<User>("User"); // trae la coleccion, la carga en collection
        }

        public async Task editUser(User user)
        {
            var filter = Builders<User>.Filter.Eq(us => us.Username, user.Username);
            await Collection.ReplaceOneAsync(filter, user); // reemplaza lo que encuentra en el filtro por lo del producto
        }

        public async Task<User> GetUserbyID(string userName)
        {
            return await Collection.FindAsync(
                new BsonDocument { { "_Username", new ObjectId(userName) } }).Result.FirstAsync();
        }

        public async Task<List<User>> GetUsers()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync(); // se le manda un documento vacío
        }

        public async Task newUser(User user)
        {
            await Collection.InsertOneAsync(user);
        }
    }
}
