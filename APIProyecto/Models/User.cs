using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APIProyecto.Models
{
    public class User
    {
        // el nombre de usuario debe ser único, primary key 
        [BsonId]
        //[BsonRepresentation(BsonType.ObjectId)]
        [Required]
        [StringLength(10)]
        public string Username { set; get; }
       // [BsonElement("name")]
        [Required]
        public string Name { set; get; }
        //[BsonElement("password")]
        [Required]
        [StringLength(8, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 6)]
        public string Password { set; get; }
        //[BsonRepresentation(BsonType.Array)]
        //[BsonElement("friends")]
        //public String[] Fiends { set; get; }
        public int Key { set; get; }
        public List<String> Fiends { set; get; }
        //[BsonElement("friendsRequest")]
        //public String[] FriendsRequest { set; get; }
        public List<String> FriendsRequest { set; get; }
        // public bool Status { set; get; } //saber si el usuario está activo o inactivo
        public List<String> GroupID { set; get; }

    }
}
