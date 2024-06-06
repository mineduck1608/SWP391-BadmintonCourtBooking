using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class UserDAO
    {

        private readonly BadmintonCourtContext _dbContext = null;

        public UserDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<User> GetAllUsers() => _dbContext.Users.ToList();

        public User GetUserById(int id) => _dbContext.Users.FirstOrDefault(x => x.UserId == id);

        public List<User> GetUsersByRole(int id) => _dbContext.Users.Where(x => x.RoleId == id).ToList();

        public List<User> GetStaffsByBranch(int id) => _dbContext.Users.Where(x => x.BranchId == id).ToList();

        public User GetUserByEmail(string email) => _dbContext.Users.FirstOrDefault(x => x.UserDetail.Email == email);

        public User GetRecentAddedUser() => _dbContext.Users.OrderBy(x => x.UserId).LastOrDefault();

        public User GetUserByLogin(string username, string password) => _dbContext.Users.FirstOrDefault(x => x.UserName == username && x.Password == password);

        public void UpdateUser(User newUser, int id)
        {
            User tmp = GetUserById(id);
            if (tmp != null)
            {
                tmp.RoleId = newUser.RoleId;
                tmp.UserName = newUser.UserName;
                tmp.Password = newUser.Password;
                tmp.BranchId = newUser.BranchId;
                tmp.Balance = newUser.Balance;
                tmp.AccessFail = newUser.AccessFail;
                tmp.LastFail = newUser.LastFail;
                _dbContext.Users.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void DeleteUser(int uId)
        {
            _dbContext.Users.Remove(GetUserById((uId)));
            _dbContext.SaveChanges();
        }
    }
}
