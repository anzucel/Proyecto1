using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIProyecto.Models;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        List<User> listUser = new List<User>();

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
        public IActionResult PostSignIn([FromBody] User user)   // recibe info para registrar usuario
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model");
            }
            // se debe agregar a la base de datos
           listUser.Add(user);
            return Ok();
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
