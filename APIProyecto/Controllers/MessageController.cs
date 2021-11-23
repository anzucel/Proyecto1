using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIProyecto.Models;
using MongoDB.Driver;
using Cifrado;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        // GET: api/<MessageController>
        Sdes sdes = new Sdes();

        [HttpGet]
        [Route("getusers")]
        public List<string> Get()
        {
            //return new string[] { "value1", "value2" };
            if (!ModelState.IsValid)
            {
                return null;
            }
            else
            {
                List<string> ListUsers = new List<string>();
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbmessages = database.GetCollection<User>("User");
                var buscarUsuario = dbmessages.AsQueryable<User>();
                var result = from a in buscarUsuario
                             select a;

                foreach (User users in result)
                {
                    string username = users.Username;
                    ListUsers.Add(username);
                }
                return ListUsers;
            }
        }

        // GET api/<MessageController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MessageController>
        [HttpPost]
        [Route("send")]
        public IActionResult SendMessage([FromBody] Messages message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            else
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbmessages = database.GetCollection<Messages>("Messages");
                var buscarUsuario = dbmessages.AsQueryable<Messages>();
                var result = from a in buscarUsuario
                             where (message.UsuarioEmisor == a.UsuarioEmisor || message.UsuarioReceptor == a.UsuarioReceptor)
                             select a;

                //int id = result.Count() + 2; // empieza en 0

                Messages newMessage = new Messages();
                newMessage.UsuarioEmisor = "";
                newMessage.UsuarioReceptor = "";
                newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                //newMessage.Id = id;
                newMessage.Texto = sdes.Cifrar(message.Texto, 192);
                dbmessages.InsertOne(newMessage);

                return Ok();
            }
        }

        // PUT api/<MessageController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
