using APIProyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProyecto.Repositories
{
    interface IUsersCollection
    {
        // se trabaja de manera asincrona
        Task newUser(User user);
        Task editUser(User user);
        Task<List<User>> GetUsers();
        Task<User> GetUserbyID(string userName);
    }
}
