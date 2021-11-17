using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritingU.Controllers
{
    public class AccoutController : Controller
    {
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
        public ActionResult Index(string Usuario, string Correo,string Contraseña,string Verificacion_C)
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
