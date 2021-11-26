using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIProyecto.Models;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        // GET: api/<GroupController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreateGroup([FromBody] Group group)
        {
            try
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbgrupos = database.GetCollection<Group>("Groups");

                Group newGroup = new Group();

                newGroup.GroupID = group.GroupID;
                newGroup.Participants = group.Participants;

                dbgrupos.InsertOne(newGroup);

                return Ok();
            }
            catch
            {
                return BadRequest("Error");
            }

        }


        [HttpGet]
        [Route("getgroups/{username}")]
        public List<string> GetGroups([FromRoute] string username)
        {
            try
            {
                List<string> users = new List<string>();
                List<string> ListGroups = new List<string>();
                // retorna todos los grupos a los que pertenece el usuario
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");
                var dbgrupos = database.GetCollection<Group>("Groups");
                var buscarGrupos = dbgrupos.AsQueryable<Group>();

                var result = from a in buscarGrupos
                             select a;

                foreach (Group group in result)
                {
                    for (int i = 0; i < group.Participants.Count; i++)
                    {
                        users.Add(group.Participants.ElementAt(i));
                    }

                    if (users.Contains(username))
                    {
                        ListGroups.Add(group.GroupID);
                    }
                }

                return ListGroups;
            }
            catch
            {
                return null;
            }
        }

        [Route("keys")]
        [HttpGet]
        public IActionResult obtenerLlaves()
        {
            Cifrado.ISdes RSA = new Cifrado.CifradoRSA();
            int[] values = generate_primos();
            List<string> keys = RSA.generadorLlaves(values[0],values[1]);
            return Ok();
        }


        int[] generate_primos()
        {
            //número a evaluar
            Random r = new Random();
            bool num = false;
            int p = 0;
            while (num == false)
            {
                p = r.Next(10, 50);
                num = primo(p);
            }

            bool num2 = false;
            int q = 0;
            while (num2 == false)
            {
                q = r.Next(10, 100);
                num2 = primo(q);
                if (p == q && (p * q <= 255))
                {
                    num2 = false;
                }
            }

            int[] value = new int[2] { p, q };
            return value;



            bool primo(int n)
            {
                bool esPrimo = true;
                for (int i = 2; i < n; i++)
                {
                    if (n % i == 0)
                    {
                        esPrimo = false;
                        break;
                    }
                }

                if (esPrimo)
                { return true; }
                else
                { return false; }

            }
        }


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

                //var dbusers = database.GetCollection<User>("User");
                //var buscarUsuario = dbusers.AsQueryable<User>();
                //var result = from a in buscarUsuario
                //             where (message.UsuarioEmisor == a.Username || message.UsuarioReceptor == a.Username)
                //             select a;


                //foreach (User users in result)
                //{
                //    if (users.Username == message.UsuarioEmisor) { keyEmisor = users.Key; }
                //    if (users.Username == message.UsuarioReceptor) { keyReceptor = users.Key; }
                //}

                
                Messages newMessage = new Messages();
                newMessage.UsuarioEmisor = message.UsuarioEmisor;
                newMessage.UsuarioReceptor = message.UsuarioReceptor;
                newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                //key = GenerateKey(keyEmisor, keyReceptor);
                newMessage.Texto = message.Texto; // se debe cifrar con RSA
                newMessage.DeleteAll = false;
                newMessage.DeleteForMe = false;

                dbmessages.InsertOne(newMessage);

                return Ok();
            }
        }

        [HttpGet]
        [Route("download/{emisor}/{receptor}")]
        public List<StringMessage> GetMessages([FromRoute] string emisor, [FromRoute] string receptor)
        {
            List<StringMessage> ListMessages = new List<StringMessage>();

            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("ChatDB");

            //bd mensajes
            var dbmessages = database.GetCollection<Messages>("Messages");
            var buscarMensaje = dbmessages.AsQueryable<Messages>(); //comentado
            var result = from a in buscarMensaje
                         where ((a.UsuarioReceptor == receptor))
                         select a;

            // Descifrar mensajes 
            foreach (Messages mess in result)
            {
                // mostrar el mensaje
                if ((mess.DeleteForMe == false || (mess.DeleteForMe == true && emisor != mess.UsuarioEmisor)) && mess.DeleteAll == false)
                {
                    StringMessage message = new StringMessage();
                    if (mess.FilePath != null)
                    {
                        string[] splt = mess.FilePath.Split('.');
                        string filename = splt[0] + "." + splt[1];
                        message.Texto = filename;
                        message.FilePath = filename;
                    }
                    else
                    {
                        byte[] DesMessages = mess.Texto; // Descifrar con RSA
                        char[] chars = new char[DesMessages.Length / sizeof(char)];
                        Buffer.BlockCopy(DesMessages, 0, chars, 0, DesMessages.Length);
                        message.Texto = new string(chars);
                    }
                    message.Fecha_envio = mess.Fecha_envio;
                    message.UsuarioEmisor = mess.UsuarioEmisor;
                    message.UsuarioReceptor = mess.UsuarioReceptor;

                    ListMessages.Add(message);
                }
            }

            return ListMessages;

        }
    }
}
