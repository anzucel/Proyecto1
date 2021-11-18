using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Proyecto1.Helper
{
    public class UserAPI
    {
        public HttpClient Initial()
        {
            //http accede a la información de la API
            var User = new HttpClient();
            User.BaseAddress = new Uri("https://localhost:44355/");
            return User;
        }
    }
}
