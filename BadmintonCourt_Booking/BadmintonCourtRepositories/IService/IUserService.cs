using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IUserService
    {

        public List<User> GetAllUsers();

        public User GetUserById(string id);

        public User GetRecentAddedUser();

        public User GetUserByEmail(string email);

        public User GetUserByLogin(string username, string password);

		public List<User> GetUsersByRole(string id);

        public List<User> GetStaffsByBranch(string id);

        public void UpdateUser(User newUser, string id);

        public void AddUser(User user);

        public void DeleteUser(string id);

    }
}
