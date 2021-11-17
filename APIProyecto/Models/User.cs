using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProyecto.Models
{
    public class User
    {
        // el nombre de usuario debe ser único
        public string Username { set; get; }
        public string Name { set; get; } 
        public string Password { set; get; }
        public User[] Fiends { set; get; }
        // public bool State { set; get; } saber si el usuario está activo o inactivo
    }
}
