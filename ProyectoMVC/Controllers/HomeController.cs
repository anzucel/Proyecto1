﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto1.Models;
using Proyecto1.Extra;
using Microsoft.AspNetCore.Http;
using APIProyecto.Repositories;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Proyecto1.Helper;
using System.Net.Http;

namespace Proyecto1.Controllers
{
    public class HomeController : Controller
    {
        APIProyecto.Repositories.IUsersCollection Friends = new APIProyecto.Repositories.UsersCollection();
        private readonly ILogger<HomeController> _logger;
        UserAPI Api = new UserAPI();    // se inicializa clase 
        Metodos metodos = new Metodos();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string amigo)
        {

            User user = new User();
            user = Singleton.Instance.user;
            metodos.GetFriends(user.Username);
            metodos.GetFriendRequest(user.Username);
            metodos.GetUsers(user.Username);

            ViewBag.chatamigo = Singleton.Instance.Amigo_Chat;
            ViewBag.chatamigo = Singleton.Instance.Amigo_Chat;
            ViewBag.usuarios = Singleton.Instance.ListUsers;
            ViewBag.Friends = Singleton.Instance.List;
            ViewBag.FriendsRequest = Singleton.Instance.ListRequests;

           
             
            
            return View();
        }

        [HttpPost]
        public IActionResult SendRequest(string usernameToAdd)
        {
           // usernameToAdd = "CarolV"; // temporal
            string user = HttpContext.Session.GetString("userLogged");
            HttpClient client = Api.Initial();
            try {
                //HttpResponseMessage res = null;
                //while (res == null)
                //{
                //    HttpClient client = Api.Initial();
                //    res = await client.GetAsync("api/message/getusers");
                //}

                var data = client.PostAsJsonAsync<string>($"api/user/sendrequest/{usernameToAdd}/{user}", usernameToAdd);
                data.Wait();

                var result = data.Result;

                if (result.IsSuccessStatusCode)
                {
                    // retorna mensaje indicando que se envió la solicitud
                    return Redirect("index");
                }
                else
                {
                    // retorna alerta que no se pudo realizar la peticion
                    return Redirect("index");
                }
                //}
                //if (res.IsSuccessStatusCode)
                //{
                //    var results = res.Content.ReadAsStringAsync().Result;
                //    Singleton.Instance.ListUsers = JsonConvert.DeserializeObject<List<string>>(results);
                //    Singleton.Instance.List = Singleton.Instance.ListUsers;
                //}
            }
            catch 
            {
                return Redirect("index");
            }
           
        }


        //Post  para aceptar/rechazar solicitud de amistad
        [HttpPost]
        public IActionResult solicitudes(string info)
        {
            //se trae en la variable info el usuario que envía la solicitud y si se acepta/recahaza
            //se hace split para separar los parametros
            string[] split = info.Split(",");
            string user = split[0];
            bool estatus = Convert.ToBoolean( split[1]);
            return Redirect("/home");
        }

        [HttpPost]
        public IActionResult Index(string mensaje, IFormFile postedFile, string amigo)
        {
            //solo es pruebas
            Singleton.Instance.Amigo_Chat = amigo;
            if (mensaje != null)
            {
                try
                {
                    Message message = new Message();
                    
                    byte[] byteM = new byte[mensaje.Length * sizeof(char)];
                    Buffer.BlockCopy(mensaje.ToCharArray(), 0, byteM, 0, byteM.Length);

                    message.Texto = byteM;
                    //API - MVC
                    HttpClient client = Api.Initial();

                    //Post-instancia a la api
                    var Data = client.PostAsJsonAsync<Message>("api/message/send", message);
                    Data.Wait();

                    var result = Data.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        //GetUsers();
                        return Redirect("home/");//si los datos son correctos al crear nueva cuenta retorna a LogIn
                    }
                    return View();
                }
                catch
                {
                    return View();
                }

            }
            else
            {
                //archivos enviados
            }

            return Redirect("home/");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
