using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIProyecto.Models;
using MongoDB.Driver;
using Cifrado;
using System.Numerics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        // GET: api/<MessageController>
        Sdes sdes = new Sdes();

        // obtiene la lista de usuarios
        [HttpGet]
        [Route("getusers/{username}")]
        public List<string> GetUsers([FromRoute] string username)
        {
            // obtiene el listado de usuarios existentes
            if (!ModelState.IsValid)
            {
                return null;
            }
            else
            {
                List<string> ListUsers = new List<string>();
                List<string> activeUsers = new List<string>();
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbmessages = database.GetCollection<User>("User");
                var buscarUsuario = dbmessages.AsQueryable<User>();
                var result = from a in buscarUsuario
                             where (a.Username == username)
                             select a;
                var result2 = from a in buscarUsuario
                              where (a.Username != username)
                              select a;

                foreach (User users in result)
                {
                    if(users.Fiends != null && users.FriendsRequest != null)
                    {
                        for (int i = 0; i < users.Fiends.Count; i++)
                        {
                            ListUsers.Add(users.Fiends.ElementAt(i).ToString());
                        }

                        for (int j = 0; j < users.FriendsRequest.Count; j++)
                        {
                            ListUsers.Add(users.FriendsRequest.ElementAt(j).ToString());
                        }
                    }
                }

                foreach (User users in result2)
                {
                    string uname = users.Username;
                    bool exist = ListUsers.Contains(uname);
                    if (exist == false)
                    {
                        activeUsers.Add(uname);
                    }
                }
                return activeUsers;
            }
        }

        [HttpGet]
        [Route("getfriends/{username}")]
        public List<string> GetFriends([FromRoute] string username)
        {
            // obtiene el listado de amigos
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
                             where (a.Username == username)
                             select a;

                foreach (User users in result)
                {
                    if(users.Fiends != null)
                    {
                        for (int i = 0; i < users.Fiends.Count; i++)
                        {
                            ListUsers.Add(users.Fiends.ElementAt(i).ToString());
                        }
                    }
                }
                return ListUsers;
            }
        }

        [HttpGet]
        [Route("getrequests/{username}")]
        public List<string> GetRequest([FromRoute] string username)
        {
            // obtiene el listado de solicitudes
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
                             where (a.Username == username)
                             select a;

                foreach (User users in result)
                {
                    if(users.FriendsRequest != null)
                    {
                        for (int i = 0; i < users.FriendsRequest.Count; i++)
                        {
                            ListUsers.Add(users.FriendsRequest.ElementAt(i).ToString());
                        }
                    }
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
                int keyEmisor = 0, keyReceptor = 0, key;
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbmessages = database.GetCollection<Messages>("Messages");
                
                //extra===========================
                var buscarMensaje = dbmessages.AsQueryable<Messages>(); //comentado
                var result1 = from a in buscarMensaje
                             where (a.UsuarioEmisor == message.UsuarioEmisor && message.UsuarioReceptor == a.UsuarioReceptor)
                             select a;
                //================================

                var dbusers = database.GetCollection<User>("User");
                var buscarUsuario = dbusers.AsQueryable<User>();
                var result = from a in buscarUsuario
                             where (message.UsuarioEmisor == a.Username || message.UsuarioReceptor == a.Username)
                             select a;


                foreach (User users in result)
                {
                    if(users.Username == message.UsuarioEmisor) { keyEmisor = users.Key; }
                    if(users.Username == message.UsuarioReceptor) { keyReceptor = users.Key; }
                }

                // extra ======================
                Messages descifrado = new Messages();
                foreach (Messages mess in result1)
                {
                    descifrado.Texto = mess.Texto;
                }
                //=============================

                Messages newMessage = new Messages();
                newMessage.UsuarioEmisor = message.UsuarioEmisor;
                newMessage.UsuarioReceptor = message.UsuarioReceptor;
                newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                key = GenerateKey(keyEmisor, keyReceptor);

                // extra ======================
                newMessage.Texto = sdes.Cifrar(message.Texto, key);
                descifrado.Texto = sdes.Descifrar(descifrado.Texto, key);
                //=============================

                dbmessages.InsertOne(newMessage);

                return Ok();
            }
        }

        // Metodo obtener llave secreta entre emisor y receptor
        public int GenerateKey(int KEmisor, int KReceptor)
        {
            int g = 11, p = 23;
            //double B = Math.Pow(g, KReceptor);
            BigInteger B = BigInteger.ModPow(g, KReceptor, p);
            //double piv = Math.Pow(B, KEmisor);
            //double K = piv % p;
            BigInteger K = BigInteger.ModPow(B, KEmisor, p);

            object obj = K;
            BigInteger big = (BigInteger)obj;
            int key = (int)big;

            return key;
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
