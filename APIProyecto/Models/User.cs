using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace APIProyecto.Models
{
    public class User
    {
        // el nombre de usuario debe ser único
        [Required]
        [StringLength(10)]
        public string Username { set; get; }
        [Required]
        public string Name { set; get; }
        [Required]

        [StringLength(8, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Password { set; get; }
        public User[] Fiends { set; get; }
        // public bool State { set; get; } saber si el usuario está activo o inactivo
    }
}
