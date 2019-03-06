using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ClientsServices
    {
        public ClientRepository _clientRepository => new ClientRepository();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Clients> GetAllClients()
        {
            return _clientRepository.GetClients();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Clients GetClientById(string Id)
        {
            return _clientRepository.GetClientsById(Id);
        }
    }
}
