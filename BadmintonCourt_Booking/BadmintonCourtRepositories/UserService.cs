using BadmintonCourtBusinessObjects.Entities;
using BadmintonCourtDAOs;
using BadmintonCourtServices.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices
{
    public class UserService : IUserService
    {

        private readonly UserDAO userDAO = null;

        public UserService()
        {
            if (userDAO == null)
                userDAO = new UserDAO();
        }

        public void AddUser(User user) => userDAO.AddUser(user);

        public void DeleteUser(int id) => userDAO.DeleteUser(id);

        public List<User> GetAllUsers() => userDAO.GetAllUsers();

        public List<User> GetStaffsByBranch(int id) => userDAO.GetStaffsByBranch(id);

        public User GetUserById(int id) => userDAO.GetUserById(id);

        public User GetRecentAddedUser() => userDAO.GetRecentAddedUser();

		public User GetUserByEmail(string email) => userDAO.GetUserByEmail(email);

		public User GetUserByLogin(string username, string password) => userDAO.GetUserByLogin(username, password);


		public List<User> GetUsersByRole(int id) => userDAO.GetUsersByRole(id);

        public void UpdateUser(User newUser, int id) => userDAO.UpdateUser(newUser, id);

    }
}
