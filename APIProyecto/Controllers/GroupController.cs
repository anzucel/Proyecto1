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
    }
}
