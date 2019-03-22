using Newtonsoft.Json;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TunisianApp.Models;

namespace TunisianApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClientsService _clientServices;
        public HomeController(IClientsService clientServices)
        {
            _clientServices = clientServices;
        }
       
        public ActionResult Index()
        {
            List<Repository.Clients> Clients = _clientServices.GetAllClients();
            //return View("Test", Clients);
            return View();

        }
        public ViewResult ListClients()
        {
            List<Repository.Clients> Clients = _clientServices.GetAllClients();
            List<ClientModel> ListClientsViewModel = new List<ClientModel>();
            foreach (var c in Clients)
            {
                ListClientsViewModel.Add(new ClientModel
                {
                    Id = c.Id,
                    Age = c.age,
                    Nom = c.Nom,
                    Prenom = c.Prenom
                });
            }
            return View(ListClientsViewModel);
        }
        public JsonResult ListClientsJson()
        {
            List<Repository.Clients> Clients = _clientServices.GetAllClients();
            return Json(Clients);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}