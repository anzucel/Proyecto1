using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Proyecto1.Helper;
using Cifrado;
using System.Text;
using Proyecto1.Models;
using System.Security.Cryptography;

namespace WritingU.Controllers
{
    public class AccoutController : Controller
    {

        Cifrado.ISdes cipher = new Cifrado.Sdes();
        Cifrado.ISdes cesar =  new Cifrado.Cesar();
        UserAPI Api = new UserAPI();    // se inicializa clase 
        
        // GET: AccoutController1
        public ActionResult Index()
        {
            return View();
        }

        // GET: AccoutController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccoutController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccoutController1/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: AccoutController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AccoutController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(User user)
        {
            try
            {
                user.Password = cesar.CifrarCesar (user.Password, 4);
  
                //API - MVC
                HttpClient client = Api.Initial();
           
                //Post-instancia a la api
                var Data = client.PostAsJsonAsync<User>("api/user/signin", user);         
                Data.Wait();

                var result = Data.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index","Login"); ;//si los datos son correctos al crear nueva cuenta retorna a LogIn
                }


                return View(); // muestra alerta de usuario inválido porque ya existe  
            }
            catch
            {
                return View();
            }
        }

        // GET: AccoutController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccoutController1/Delete/5
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
