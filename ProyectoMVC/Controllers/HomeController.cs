using System;
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
using System.Text;
using System.Net;
using System.Web;
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

        [HttpGet]
        public IActionResult Index(string amigo)
        {
            //string userSession = HttpContext.Session.GetString("userLogged");
            Singleton.Instance.usuario = HttpContext.Session.GetString("userLogged");

            User user = new User();
            user = Singleton.Instance.user;
            metodos.GetFriends(Singleton.Instance.usuario);
            metodos.GetFriendRequest(Singleton.Instance.usuario);
            metodos.GetUsers(Singleton.Instance.usuario);
            //usuario logiado
            ViewBag.userLogin = Singleton.Instance.usuario;//HttpContext.Session.GetString("userLogged");
            ViewBag.chatamigo = Singleton.Instance.Amigo_Chat;
            // DownloadMessages(Singleton.Instance.Amigo_Chat);

            ViewBag.usuarios = Singleton.Instance.ListUsers;
            ViewBag.Friends = Singleton.Instance.List;
            ViewBag.FriendsRequest = Singleton.Instance.ListRequests;
            ViewBag.chat = Singleton.Instance.ListMessages;
            return View();
        }

        [HttpPost]
        public IActionResult SendRequest(string usernameToAdd)
        {
            // usernameToAdd = "CarolV"; // temporal
            string user = HttpContext.Session.GetString("userLogged");
            HttpClient client = Api.Initial();
            try
            {
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
            bool estatus = Convert.ToBoolean(split[1]);

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
                // receptor = receptor;//"MariaG";
                Singleton.Instance.ListMessages = null;
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

                ViewBag.chat = Singleton.Instance.ListMessages;
                return RedirectToAction(nameof(Index));

            }
            catch
            {
                ViewBag.chat = Singleton.Instance.ListMessages;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(string Filename, string receptor)
        {
            try
            {
                receptor = "andreaxd";//"MariaG";
                string emisor = HttpContext.Session.GetString("userLogged");
                emisor = "jasonxd";
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
            if (amigo != null)
            {
                Singleton.Instance.Amigo_Chat = amigo;
                DownloadMessages(amigo);
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
                        DownloadMessages(amigo);
                        return RedirectToAction(nameof(Index));//si los datos son correctos al crear nueva cuenta retorna a LogIn
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return RedirectToAction(nameof(Index));
                }

            }

            if (files != null)
            {

                amigo = Singleton.Instance.Amigo_Chat; // se debe leer desde el parámetro

                Message message = new Message();

                //archivos enviados
                byte[] readText = null;
                using (var ms = new MemoryStream())
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
                    return RedirectToAction("Index", "Home");//si los datos son correctos al crear nueva cuenta retorna a LogIn

                }
            }

            return RedirectToAction("Index", "Home");
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


        //PUREBA
        public ActionResult GetFile(string name)
        {

            //if ()
            //{
            name = "usuario2.jpg";
            var filePath = "files\\" + name;
            var fileName = string.Empty;
            for (int i = filePath.Length - 1; i > -1; i--)
            {
                if (filePath[i] == '\\')
                {
                    i++;
                    fileName = filePath.Substring(i, filePath.Length - i);
                    break;
                }
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            //System.IO.File.Delete(filePath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            // }
            //else
            //{
            //    return RedirectToAction("Error");
            //}


        }


        //delete message
        public IActionResult DeleteMessage(string deleteM, string Texto, string receptor, string emisor)
        {
            //deleteM = me-> eliminar para mi || all->eliminar para todos
            //Texto = mensaje que se quiere eliminar
            //usuario_delete = el usuario que esta haciendo la acción de eliminar
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }     
}
