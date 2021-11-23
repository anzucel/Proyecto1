using System;
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
        

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string amigo)
        {

            //List<string> ListaAmigo = new List<string>();
            // ListaAmigo.Add("Martin Bech");
            // ListaAmigo.Add("Pablo godoy");
            // ListaAmigo.Add("zucely Raxón");
            // ListaAmigo.Add("Marta Raxón");
            // ListaAmigo.Add("Lupe Gomez");
            // ListaAmigo.Add("Jason Giron");
            // ListaAmigo.Add("Maryorie Sosa");
            // ListaAmigo.Add("Lucas Perez");
            // ListaAmigo.Add("Miguel Lopez");
            // ListaAmigo.Add("Carlos Gonzalez");
                GetUsers();
                ViewBag.chatamigo = Singleton.Instance.Amigo_Chat;
                ViewBag.LA = Singleton.Instance.List;
            
                     

            return View();
        }

        [HttpPost]
        public IActionResult Añadir_Amigo(string usernameToAdd)
        {
<<<<<<< HEAD
            usernameToAdd = "CarolV"; // temporal
            string user = HttpContext.Session.GetString("userLogged");
            HttpClient client = Api.Initial();
=======
            try {
                HttpResponseMessage res = null;
                while (res == null)
                {
                    HttpClient client = Api.Initial();
                    res = await client.GetAsync("api/message/getusers");
                }

>>>>>>> b3efdf7ba6e8bba713f92d72b6df8d0fb25baaa1

            var data = client.PostAsJsonAsync<string>($"api/user/sendrequest/{usernameToAdd}/{user}", usernameToAdd);
            data.Wait();

<<<<<<< HEAD
            var result = data.Result;

            if (result.IsSuccessStatusCode) {
                // retorna mensaje indicando que se envió la solicitud
                return Redirect("home/");
            }
            else
            {
                // retorna alerta que no se pudo realizar la peticion
                return Redirect("home/");
            }
=======
                if (res.IsSuccessStatusCode)
                {
                    var results = res.Content.ReadAsStringAsync().Result;
                    Singleton.Instance.ListUsers = JsonConvert.DeserializeObject<List<string>>(results);
                    Singleton.Instance.List = Singleton.Instance.ListUsers;
                }
            }
            catch 
            {
                GetUsers();
            }
           
        }
        //Post  para enviar solicitud de amistad
        [HttpPost]
        public IActionResult Añadir_Amigo(string añadiramigo)
        {

            return Redirect("home/");
        }

        //Post  para aceptar/rechazar solicitud de amistad
        [HttpPost]
        public IActionResult solicitudes(bool estatus)
        {

            return Redirect("/home");
>>>>>>> b3efdf7ba6e8bba713f92d72b6df8d0fb25baaa1
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
