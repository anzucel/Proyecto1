using Newtonsoft.Json;
using Proyecto1.Extra;
using Proyecto1.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proyecto1.Models
{
    public class Metodos
    {
        UserAPI Api = new UserAPI();

        // se obtiene la lista de todos los usuarios registrados
        public async void GetUsers(string username)
        {
            HttpClient client = Api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/message/getusers/{username}");


            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                Singleton.Instance.ListUsers = JsonConvert.DeserializeObject<List<string>>(results);
            }
        }

        // listado de amigos
        public async void GetFriends(string username)
        {
            HttpClient client = Api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/message/getfriends/{username}");


            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                Singleton.Instance.ListFriends = JsonConvert.DeserializeObject<List<string>>(results);
                Singleton.Instance.List = Singleton.Instance.ListFriends;
            }
        }

        // listado de solicitudes
        public async void GetFriendRequest(string username)
        {
            HttpClient client = Api.Initial();
            HttpResponseMessage res = await client.GetAsync($"api/message/getrequests/{username}");

            if (res.IsSuccessStatusCode)
            {
                var results = res.Content.ReadAsStringAsync().Result;
                Singleton.Instance.ListRequests = JsonConvert.DeserializeObject<List<string>>(results);
            }
        }
    }
}
