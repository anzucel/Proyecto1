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

        //obtener toda la lista de usuarios 

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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
            // se debe agregar a la base de datos 
            db.newUser(user);
            //db.GetUsers();
            return Ok();
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

                return NoContent();
            }
        }

        /*[HttpPost]
        public void Post([FromBody] string value)
        {
        }*/



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
