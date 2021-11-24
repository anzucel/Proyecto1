using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto1;
using Proyecto1.Extra;
using Proyecto1.Helper;
using Proyecto1.Models;

namespace Proyecto1.Extra
{
    public sealed class Singleton
    {
        private readonly static Singleton instance = new Singleton();
        
        //public List<> name_Original = new List<string>();
        public List<string> name_cipher = new List<string>();
        public List<User >users ;
        public String Amigo_Chat="";
        public List<string> ListUsers; // lista de usuarios existentes
        public List<string> ListFriends; // lista de amigos
        public List<string> ListRequests; // lista de solicitudes
        public User user = new User();
        public User userSesion = new User();
        public string AmigoChatear;
        public List<string> List = new List<string>();
        private Singleton()
        {
            
        }
      
        public static Singleton Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
