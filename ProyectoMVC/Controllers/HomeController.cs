using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto1.Models;
using Proyecto1.Extra;
using APIProyecto.Models;
using Microsoft.AspNetCore.Http;
using APIProyecto.Repositories;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Proyecto1.Controllers
{
    public class HomeController : Controller
    {
        APIProyecto.Repositories.IUsersCollection Friends = new APIProyecto.Repositories.UsersCollection();
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           

            return View();
        }
      

        [HttpPost]
        public IActionResult Index(string mensaje, IFormFile postedFile)
        {
            //solo es pruebas
            if (mensaje == null)
            {
                //se almacena el mensaje y todo lo que le corresponde
            }
            else
            {
                //archivos enviados
            }

            return View(Singleton.Instance.users);
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
