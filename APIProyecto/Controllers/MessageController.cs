using System;
using System.Text;
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

        [HttpGet]
        [Route("download/{emisor}/{receptor}")]
        public List<StringMessage> GetMessages([FromRoute] string emisor, [FromRoute] string receptor)
        {

            int keyEmisor = 0, keyReceptor = 0, key;
            List<StringMessage> ListMessages = new List<StringMessage>();

            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("ChatDB");

            //bd mensajes
            var dbmessages = database.GetCollection<Messages>("Messages");
            var buscarMensaje = dbmessages.AsQueryable<Messages>(); //comentado
            var result = from a in buscarMensaje
                          where ((a.UsuarioEmisor == emisor && a.UsuarioReceptor == receptor) || 
                                 (a.UsuarioEmisor == receptor && a.UsuarioReceptor == emisor))
                          select a;

            //bd usuarios 
            var dbusers = database.GetCollection<User>("User");
            var buscarUsuario = dbusers.AsQueryable<User>();
            var resultusers = from a in buscarUsuario
                         where (a.Username == emisor || a.Username == receptor)
                         select a;

            // Buscar llave entre usuarios 
            foreach (User users in resultusers)
            {
                if (users.Username == emisor) { keyEmisor = users.Key; }
                if (users.Username == receptor) { keyReceptor = users.Key; }
            }

            key = GenerateKey(keyEmisor, keyReceptor);

            // Descifrar mensajes 
            foreach (Messages mess in result)
            {
                StringMessage message = new StringMessage();
                byte[] DesMessages = sdes.Descifrar(mess.Texto, key); // se guarda el texto descifrado
                char[] chars = new char[DesMessages.Length / sizeof(char)];
                Buffer.BlockCopy(DesMessages, 0, chars, 0, DesMessages.Length);
                message.Texto = new string(chars);
                message.Fecha_envio = mess.Fecha_envio;
                message.UsuarioEmisor = mess.UsuarioEmisor;
                message.UsuarioReceptor = mess.UsuarioReceptor;

                ListMessages.Add(message);
            }

            // retorna la lista de mensajes
            return ListMessages;
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
                //Messages descifrado = new Messages();
                //foreach (Messages mess in result1)
                //{
                //    descifrado.Texto = mess.Texto;
                //}
                //=============================

                Messages newMessage = new Messages();
                newMessage.UsuarioEmisor = message.UsuarioEmisor;
                newMessage.UsuarioReceptor = message.UsuarioReceptor;
                newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                key = GenerateKey(keyEmisor, keyReceptor);
                newMessage.Texto = sdes.Cifrar(message.Texto, key);

                // extra ======================
                // descifrado.Texto = sdes.Descifrar( newMessage.Texto, key);
                //=============================

                dbmessages.InsertOne(newMessage);

                return Ok();
            }
        }

        // Metodo obtener llave secreta entre emisor y receptor
        public int GenerateKey(int KEmisor, int KReceptor)
        {
            int g = 11, p = 23;
            BigInteger B = BigInteger.ModPow(g, KReceptor, p);
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
