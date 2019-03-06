using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TunisianApp.Models
{
    public class ClientModel
    {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Age { get; set; }
    }
    public class ListClientsModel
    {
        public List<ClientModel> AllClients { get; set; }
    }
}