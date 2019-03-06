using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserService
    {
        List<User> GetUsers();
        User GetUserById(string Id);
        string GetUserIdAfterAuthentification(string Login, string Mdp);
    }
}
