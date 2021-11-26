using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto1.Models
{
    public class Group
    {
        [BsonId]
        public string GroupID { set; get; }
        public List<string> Participants { set; get; }

        int[] keyprivate { get; set; }
        int[] keypublic { get; set; }

    }
}
