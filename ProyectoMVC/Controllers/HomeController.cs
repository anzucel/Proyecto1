﻿using System;
using System.IO;
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
using Cifrado;

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
            //usuario logiado
            ViewBag.userLogin = HttpContext.Session.GetString("userLogged");
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
                var data = client.PostAsJsonAsync<string>($"api/user/sendrequest/{usernameToAdd}/{user}", usernameToAdd);
                data.Wait();

                var result = data.Result;

                if (result.IsSuccessStatusCode)
                {
                    metodos.GetFriends(user);
                    metodos.GetFriendRequest(user);
                    metodos.GetUsers(user);
                    // retorna mensaje indicando que se envió la solicitud
                    return Redirect("index");
                }
                else
                {
                    // retorna alerta que no se pudo realizar la peticion
                    return Redirect("index");
                }
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
            string userToAdd = split[0];
            bool estatus = Convert.ToBoolean( split[1]);

            string user = HttpContext.Session.GetString("userLogged");

            try
            {
                HttpClient client = Api.Initial();
                var data = client.PostAsJsonAsync<string>($"api/user/acceptrequest/{userToAdd}/{user}/{estatus}", userToAdd);

                data.Wait();

                var result = data.Result;

                if (result.IsSuccessStatusCode)
                {
                    metodos.GetFriends(user);
                    metodos.GetFriendRequest(user);
                    metodos.GetUsers(user);
                    // muestra mensaje de solicitud aceptada
                    return Redirect("index");
                }
                else
                {
                    //muestra mensaje de solicitud rechazada
                    return Redirect("index");
                }
            }
            catch 
            {
                return Redirect("/home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadMessages(string receptor)
        {
            try
            {
                receptor = "ZucelyH";//"MariaG";
                string emisor = HttpContext.Session.GetString("userLogged");
                Message mensajes = new Message();
                mensajes.UsuarioEmisor = emisor;
                mensajes.UsuarioReceptor = receptor;

                HttpClient client = Api.Initial();

                HttpResponseMessage res = await client.GetAsync($"api/message/download/{emisor}/{receptor}");

                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    Singleton.Instance.ListMessages = JsonConvert.DeserializeObject<List<StringMessage>>(results); //  guarda todos los mensajes
                }
                
                return Redirect("index");
            }
            catch 
            {
                return Redirect("/home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string Filename, string receptor)
        {
            try
            {
                receptor = null;//"MariaG";
                string emisor = HttpContext.Session.GetString("userLogged");

                HttpClient client = Api.Initial();

                HttpResponseMessage res = await client.GetAsync($"api/lzwcompress/downloadFile/{emisor}/{receptor}/{Filename}");

                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    Message message = JsonConvert.DeserializeObject<Message>(results); //  guarda todos los mensajes


                    // en message.FilePath se encuentra el nombre del archivo guardado en la carpeta \\Files\\
                }

                return Redirect("index");
            }
            catch
            {
                return Redirect("/home");
            }
        }

        [HttpPost]
        public IActionResult creatgroup(string[] members, string name)
        {
            return Redirect("index");
        }

        [HttpPost]
        public IActionResult Index(string mensaje, IFormFile files, string amigo)
        {
            if(amigo != null)
            {
                Singleton.Instance.Amigo_Chat = amigo;
            }
            //solo es pruebas

            if (mensaje != null)
            {
                try
                {
                    amigo = Singleton.Instance.Amigo_Chat; // se debe leer desde el parámetro
                    string emisor = HttpContext.Session.GetString("userLogged");

                    Message message = new Message();

                    byte[] byteM = new byte[mensaje.Length * sizeof(char)];
                    Buffer.BlockCopy(mensaje.ToCharArray(), 0, byteM, 0, byteM.Length);
                    message.Texto = byteM;
                    message.UsuarioEmisor = emisor;
                    message.UsuarioReceptor = amigo;

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

            if(files!= null)
            {
                amigo = Singleton.Instance.Amigo_Chat; // se debe leer desde el parámetro
                Message message = new Message();

                //archivos enviados
                byte[] readText = null;
                using(var ms = new MemoryStream())
                {
                    files.CopyTo(ms);
                    readText = ms.ToArray();
                }

                message.Texto = readText;
                message.UsuarioEmisor = HttpContext.Session.GetString("userLogged");
                message.UsuarioReceptor = amigo;
                message.FilePath = files.FileName.ToString();

                //API - MVC
                HttpClient client = Api.Initial();
                //Post-instancia a la api
                var Data = client.PostAsJsonAsync<Message>("api/lzwcompress/sendfile", message);
                Data.Wait();

                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    //GetUsers();
                    return Redirect("home/");//si los datos son correctos al crear nueva cuenta retorna a LogIn
                }
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
