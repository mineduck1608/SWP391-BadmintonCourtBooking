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

        private readonly UserDAO _userDAO = null;

        public UserService()
        {
            if (_userDAO == null)
                _userDAO = new UserDAO();
        }

        public void AddUser(User user) => _userDAO.AddUser(user);

        public void DeleteUser(string id) => _userDAO.DeleteUser(id);

        public List<User> GetAllUsers() => _userDAO.GetAllUsers();

        public List<User> GetStaffsByBranch(string id) => _userDAO.GetStaffsByBranch(id);

        public User GetUserById(string id) => _userDAO.GetUserById(id);

        public User GetRecentAddedUser() => _userDAO.GetRecentAddedUser();

		public User GetUserByEmail(string email) => _userDAO.GetUserByEmail(email);

		public User GetUserByLogin(string username, string password) => _userDAO.GetUserByLogin(username, password);


		public List<User> GetUsersByRole(string id) => _userDAO.GetUsersByRole(id);

        public void UpdateUser(User newUser, string id) => _userDAO.UpdateUser(newUser, id);

    }
}
