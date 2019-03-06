using Repository;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserServices : IUserService
    {
        public UserRepository _userRepository=> new UserRepository();

        public List<User> GetUsers()
        {
            return _userRepository.GetUsers();
        }
        public User GetUserById(string Id)
        {
            return _userRepository.GetUserById(Id);
        }
        public string GetUserIdAfterAuthentification(string Login, string Mdp)
        {
            return _userRepository.GetUserIdAfterAuthentification(Login, Mdp);
        }
    }
}
