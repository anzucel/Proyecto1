using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto1.Models
{
    public class Message
	{
		[BsonId]
		[BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
		public string Id { get; set; }
		public String UsuarioEmisor { get; set; }
		public String UsuarioReceptor { get; set; }
		public byte[] Texto { get; set; }
		public string Fecha_envio { get; set; }
		public int SalaID { get; set; }
	}
}
