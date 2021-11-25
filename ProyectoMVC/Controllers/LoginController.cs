using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto1.Helper;
using Proyecto1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Proyecto1.Extra;
using Proyecto1.Controllers;
using Newtonsoft.Json;

namespace WritingU.Controllers
{
    public class LoginController : Controller
    {
        UserAPI Api = new UserAPI();
        Metodos metodos = new Metodos();
        Cifrado.ISdes cipher = new Cifrado.Sdes();
        Cifrado.ISdes cesar = new Cifrado.Cesar();
        // GET: LoginController
        public ActionResult Index()
        {
            ViewBag.error = false;
             return View();
        }

        // GET: LoginController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string pass)
        {
            try
            {
                User user = new User();
                user.Name = username;
                user.Password = pass;
                user.Username = username;
                bool error = false;
                //Api-mv
                HttpClient client = Api.Initial();

                //Post- instancia a la API
                var data = client.PostAsJsonAsync<User>("api/user/login", user);
                data.Wait();

                var result = data.Result;

                if (result.IsSuccessStatusCode)
                {
                    HttpContext.Session.SetString("userLogged", user.Username); // se crea la sesión para el usuario que inició sesión

                    string userSession = HttpContext.Session.GetString("userLogged");

                    Singleton.Instance.ListUsers = new List<string>();
                    metodos.GetFriends(userSession);
                    metodos.GetFriendRequest(userSession);
                    metodos.GetUsers(userSession);


                    //resetear información de la peronsa logeada antes
                    Singleton.Instance.Amigo_Chat = null;
                    Singleton.Instance.ListUsers = null;
                    Singleton.Instance.List = null;
                    Singleton.Instance.ListRequests = null;
                    Singleton.Instance.ListMessages=null;


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Debería mostrar alerta contraseña inválida
                   error = true;
                    ViewBag.error = error;
                    return View();
                }                
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
