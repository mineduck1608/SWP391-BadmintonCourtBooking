using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class UserDetailDAO
    {
        private readonly BadmintonCourtContext _dbContext = null;

        public UserDetailDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<UserDetail> GetAllUserDetails() => _dbContext.UserDetails.ToList();

        public UserDetail GetUserDetailById(int id) => _dbContext.UserDetails.FirstOrDefault(x => x.UserId == id);

        public List<UserDetail> GetUserDetailsByName(string name) => _dbContext.UserDetails.Where(x => x.LastName.ToLower().Contains(name.ToLower()) || x.FirstName.ToLower().Contains(name.ToLower())).ToList();

		//public UserDetail GetUserDetailByMail(string mail) => _dbContext.UserDetails.FirstOrDefault(x => x.Email.ToLower().Contains(mail.ToLower()));

		public UserDetail GetUserDetailByMail(string mail) => _dbContext.UserDetails.FirstOrDefault(x => x.Email.Equals(mail));

		public List<UserDetail> GetUserDetailsBySearchResult(string search) => _dbContext.UserDetails.Where(x => x.Email.ToLower().Contains(search.ToLower()) || x.LastName.ToLower().Contains(search.ToLower()) || x.FirstName.ToLower().Contains(search.ToLower())).ToList();

        public void UpdateUserDetail(UserDetail newUserDetail, int id)
        {
            UserDetail tmp = GetUserDetailById(id);
            if (tmp != null)
            {
                tmp.FirstName = newUserDetail.FirstName;
                tmp.LastName = newUserDetail.LastName;
                tmp.Email = newUserDetail.Email;
                tmp.Phone = newUserDetail.Phone;
                _dbContext.UserDetails.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddUserDetail(UserDetail userDetail)
        {
            _dbContext.UserDetails.Add(userDetail);
            _dbContext.SaveChanges();
        }

        public void DeleteUserDetail(int id)
        {
            _dbContext.UserDetails.Remove(GetUserDetailById((id)));
            _dbContext.SaveChanges();
        }
    }
}

