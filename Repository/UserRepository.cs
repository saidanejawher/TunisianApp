using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository
    {
        public List<User> GetUsers()
        {
            using (var context = new TunisianAppEntities())
            {
                return context.User.ToList();
            }
        }
        public User GetUserById(string Id)
        {
            using (var context = new TunisianAppEntities())
            {
                var User = context.User.Where(c => c.Id == Id).FirstOrDefault();
                return User;
            }
        }
        public string GetUserIdAfterAuthentification(string Login, string Mdp)
        {
            var UserId = "";
            if (Authentification(Login, Mdp))
            {
                using (var context = new TunisianAppEntities())
                {
                    UserId = context.User.Where(c => c.Login == Login && c.Mdp == Mdp).FirstOrDefault().Id;
                }
            }
            return UserId;
        }
        public bool Authentification(string Login, string Mdp)
        {
            using (var context = new TunisianAppEntities())
            {
                var User = context.User.Where(c => c.Login == Login && c.Mdp == Mdp).FirstOrDefault();
                var Succes = User == null ? false : true;
                return Succes;
            }
        }
    }
}
