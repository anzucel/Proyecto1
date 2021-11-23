using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIProyecto.Helper;
using APIProyecto.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using APIProyecto.Repositories;
using Microsoft.AspNetCore.Http;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private IUsersCollection db = new UsersCollection();
        Cifrado.ISdes cipher = new Cifrado.Sdes();
        Cifrado.ISdes cesar = new Cifrado.Cesar();
        //List<User> listUser = new List<User>();
       
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<UserController>
        [HttpPost]
        [Route("signin")]
        public IActionResult PostSignIn([FromBody] User user)   // recibe info para registrar usuario
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            else
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var users = database.GetCollection<User>("User");
                var buscarUsuario = users.AsQueryable<User>();
                var result = from a in buscarUsuario
                             where (a.Username.ToLower() == user.Username.ToLower())
                             select a;

                foreach(User usuarios in result)
                {
                    if (usuarios.Username == user.Username)
                    {
                        return BadRequest("Usuario ya existente"); 
                    }
                }

                db.newUser(user);
                return Ok();
            }
        }

        [HttpPost]
        [Route("login")]
        public IActionResult PostLogIn(User user)   // recibe info para registrar usuario
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            else
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("ChatDB");
                var users = db.GetCollection<User>("User");
                var buscarUsuario = users.AsQueryable<User>();
                var result = from a in buscarUsuario
                             where (a.Username == user.Username)
                             select a;

                foreach(User buscar in result)
                {
                    string passdb = buscar.Password;
                    passdb = cesar.DesifrarCesar(passdb, 4);
                    if(passdb == user.Password)
                    {
                        return Ok();
                    }
                }

                return BadRequest("No existe el usuario");
            }
        }


        [HttpPost]
        [Route("sendrequest/{user}/{usernameToAdd}")]
        public IActionResult SendRequest([FromRoute] string user, [FromRoute] string usernameToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            else
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbUsers = database.GetCollection<User>("User");
                var buscarUsuario = dbUsers.AsQueryable<User>();
                var result = from a in buscarUsuario
                             where (a.Username == user)
                             select a;

                User dataUser = new User();
                foreach (User data in result)
                {
                    dataUser.Username = data.Username;
                    dataUser.Name = data.Name;
                    dataUser.Password = data.Password;
                    dataUser.Fiends = data.Fiends;
                    dataUser.FriendsRequest = data.FriendsRequest;

                    List<string> Requests = dataUser.FriendsRequest;
                    if(Requests == null)
                    {
                        Requests = new List<string>();
                    }
                    Requests.Add(usernameToAdd);

                    dataUser.FriendsRequest = Requests;
                }

                dbUsers.ReplaceOne(u => u.Username == user, dataUser);

                return Ok();
            }

        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
