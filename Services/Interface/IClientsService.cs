﻿using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IClientsService
    {
        List<Clients> GetAllClients();
        Clients GetClientById(string Id);

    }
    
}
