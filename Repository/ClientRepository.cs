using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class ClientRepository
    {
        public List<Clients> GetClients()
        {
            using (var context = new TunisianAppEntities())
            {
                return context.Clients.ToList();
            }
        }
        public Clients GetClientsById(string Id)
        {
            using (var context = new TunisianAppEntities())
            {
                var Client = context.Clients.Where(c => c.Id == Id).FirstOrDefault();
                return Client;
            }
        }
        //public bool createClients
    }
}
