using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TunisianApp.Models;

namespace TunisianApp.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientsService _clientServices;


        public ClientController(IClientsService clientServices)
        {
            _clientServices = clientServices;
        }
     
        public ActionResult Index()
        {
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

        public ActionResult EditClient(Guid idClient, ClientModel Client)
        {

            return null;

        }
    }
}