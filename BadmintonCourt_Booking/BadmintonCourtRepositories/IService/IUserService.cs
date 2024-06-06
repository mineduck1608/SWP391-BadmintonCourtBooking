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

        public User GetUserById(int id);

        public User GetRecentAddedUser();

        public User GetUserByEmail(string email);

        public User GetUserByLogin(string username, string password);

		public List<User> GetUsersByRole(int id);

        public List<User> GetStaffsByBranch(int id);

        public void UpdateUser(User newUser, int id);

        public void AddUser(User user);

        public void DeleteUser(int id);

    }
}
