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

        Cifrado.CifradoRSA RSA = new Cifrado.CifradoRSA();

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

                List<string> participantes = group.Participants;
                List<string> info = new List<string>();


                for (int i = 0; i < participantes.Count; i++)
                {
                    List<string> llaves = obtenerLlaves();
                    string usuario = participantes.ElementAt(i) + "|" + llaves.ElementAt(0) + "|" + llaves.ElementAt(1);
                    info.Add(usuario);
                }

                newGroup.Participants = info;

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


                List<string> participantes = new List<string>();
                foreach (Group group in result)
                {
                    for (int i = 0; i < group.Participants.Count; i++)
                    {
                        string participante = group.Participants.ElementAt(i);
                        string[] piv = participante.Split('|');
                        string user = piv[0];

                        users.Add(user);
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


        public List<string> obtenerLlaves()
        {
            Cifrado.ISdes RSA = new Cifrado.CifradoRSA();
            int[] values = generate_primos();
            List<string> keys = RSA.generadorLlaves(values[0],values[1]); // 0 publica, 1 privada
            return keys;
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

                var dbGroup = database.GetCollection<Group>("Groups");
                var buscarUsuario = dbGroup.AsQueryable<Group>();
                var result = from a in buscarUsuario
                             where (a.GroupID == message.UsuarioReceptor)
                             select a;

                List<string> participants = new List<string>();
                string keys = "";
                int privateK = 0;
                int n = 0;
                foreach (Group group in result)
                {
                    participants = group.Participants;
                }


                for (int i = 0; i < participants.Count; i++)
                {

                    string info = participants.ElementAt(i);
                    string[] piv = info.Split('|');

                    if (piv[0] == message.UsuarioEmisor)
                    {
                        keys = piv[1];
                        string[] piv2 = keys.Split(',');
                        privateK = Convert.ToInt32(piv2[0]);
                        n = Convert.ToInt32(piv2[1]);
                    }
                }

                Messages newMessage = new Messages();
                newMessage.UsuarioEmisor = message.UsuarioEmisor;
                newMessage.UsuarioReceptor = message.UsuarioReceptor;
                newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                //key = GenerateKey(keyEmisor, keyReceptor);
                newMessage.Texto = RSA.CifrarRSA(message.Texto, privateK, n); //message.Texto; // se debe cifrar con RSA
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

            var dbGroup = database.GetCollection<Group>("Groups");
            var buscarUsuario = dbGroup.AsQueryable<Group>();
            var result2 = from a in buscarUsuario
                         where (a.GroupID == receptor)
                         select a;

            List<string> participants = new List<string>();
            string keys = "";
            int publicK = 0;
            int n = 0;

            foreach (Group group in result2)
            {
                participants = group.Participants;
            }

            // Descifrar mensajes 
            foreach (Messages mess in result)
            {
                // mostrar el mensaje
                if ((mess.DeleteForMe == false || (mess.DeleteForMe == true && emisor != mess.UsuarioEmisor)) && mess.DeleteAll == false)
                {
                    for (int i = 0; i < participants.Count; i++)
                    {

                        string info = participants.ElementAt(i);
                        string[] piv = info.Split('|');

                        if (piv[0] == mess.UsuarioEmisor)
                        {
                            keys = piv[2];
                            string[] piv2 = keys.Split(',');
                            publicK = Convert.ToInt32(piv2[0]);
                            n = Convert.ToInt32(piv2[1]);
                        }
                    }

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
                        byte[] DesMessages = RSA.DescifrarRSA(mess.Texto, publicK, n); //mess.Texto; // Descifrar con RSA
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

        [HttpPost]
        [Route("deleteMessage")]
        public IActionResult DeleteMessage([FromBody] StringMessage message)
        {
            try
            {
                int keyEmisor = 0, keyReceptor = 0, key;

                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var database = client.GetDatabase("ChatDB");

                //bd mensajes
                var dbmessages = database.GetCollection<Messages>("Messages");
                var buscarMensaje = dbmessages.AsQueryable<Messages>(); //comentado
                var result = from a in buscarMensaje
                             where ((a.UsuarioEmisor == message.UsuarioEmisor && a.UsuarioReceptor == message.UsuarioReceptor))
                             select a;

                var dbGroup = database.GetCollection<Group>("Groups");
                var buscarUsuario = dbGroup.AsQueryable<Group>();
                var result2 = from a in buscarUsuario
                              where (a.GroupID == message.UsuarioReceptor)
                              select a;

                List<string> participants = new List<string>();
                string keys = "";
                int publicK = 0;
                int n = 0;

                foreach (Group group in result2)
                {
                    participants = group.Participants;
                }


                // Descifrar mensajes 
                foreach (Messages mess in result)
                {
                    Messages updateMessage = new Messages();

                    string filepath = "";
                    string mensaje = "";
                    if (mess.FilePath != null)
                    {
                        string[] splt = mess.FilePath.Split('.');
                        string type = "." + splt[1];
                        filepath = splt[0] + type;
                    }
                    else
                    {
                        for (int i = 0; i < participants.Count; i++)
                        {

                            string info = participants.ElementAt(i);
                            string[] piv = info.Split('|');

                            if (piv[0] == mess.UsuarioEmisor)
                            {
                                keys = piv[2];
                                string[] piv2 = keys.Split(',');
                                publicK = Convert.ToInt32(piv2[0]);
                                n = Convert.ToInt32(piv2[1]);
                            }
                        }

                        byte[] DesMessages = RSA.DescifrarRSA(mess.Texto, publicK, n); //mess.Texto; // descifrar con RSA
                        char[] chars = new char[DesMessages.Length / sizeof(char)];
                        Buffer.BlockCopy(DesMessages, 0, chars, 0, DesMessages.Length);
                        mensaje = new string(chars);
                    }

                    if (mensaje == message.Texto || filepath == message.Texto)
                    {
                        if (message.DeleteForMe)
                        {
                            updateMessage.Id = mess.Id;
                            updateMessage.UsuarioEmisor = mess.UsuarioEmisor;
                            updateMessage.UsuarioReceptor = mess.UsuarioReceptor;
                            updateMessage.Texto = mess.Texto;
                            updateMessage.FilePath = mess.FilePath;
                            updateMessage.Fecha_envio = mess.Fecha_envio;
                            updateMessage.SalaID = mess.SalaID;
                            updateMessage.DeleteForMe = true;
                            updateMessage.DeleteAll = false;
                        }
                        if (message.DeleteAll)
                        {
                            updateMessage.Id = mess.Id;
                            updateMessage.UsuarioEmisor = mess.UsuarioEmisor;
                            updateMessage.UsuarioReceptor = mess.UsuarioReceptor;
                            updateMessage.Texto = mess.Texto;
                            updateMessage.FilePath = mess.FilePath;
                            updateMessage.Fecha_envio = mess.Fecha_envio;
                            updateMessage.SalaID = mess.SalaID;
                            updateMessage.DeleteForMe = false;
                            updateMessage.DeleteAll = true;
                        }

                        dbmessages.ReplaceOne(u => u.Id == mess.Id, updateMessage);
                    }
                }

                return Ok();
            }
            catch
            {
                return BadRequest("Error");
            }
        }

    }
}
