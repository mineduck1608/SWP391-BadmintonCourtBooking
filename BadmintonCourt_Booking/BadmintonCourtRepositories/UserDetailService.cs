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
    public class UserDetailService : IUserDetailService
    {

        private readonly UserDetailDAO _userDetailDAO = null;

        public UserDetailService()
        {
            if (_userDetailDAO == null)
            {
                _userDetailDAO = new UserDetailDAO();
            }
        }

        public void AddUserDetail(UserDetail userDetail) => _userDetailDAO.AddUserDetail(userDetail);

        public void DeleteUserDetail(string id) => _userDetailDAO.DeleteUserDetail(id); 

        public List<UserDetail> GetAllUserDetails() =>  _userDetailDAO.GetAllUserDetails();

        public UserDetail GetUserDetailById(string id) => _userDetailDAO.GetUserDetailById(id);

        public UserDetail GetUserDetailByMail(string mail) => _userDetailDAO.GetUserDetailByMail(mail);

        public List<UserDetail> GetUserDetailsByName(string name) => _userDetailDAO.GetUserDetailsByName(name);

        public List<UserDetail> GetUserDetailsBySearchResult(string search) => _userDetailDAO.GetUserDetailsBySearchResult(search);

        public void UpdateUserDetail(UserDetail newUserDetail, string id) => _userDetailDAO.UpdateUserDetail(newUserDetail, id);

    }
}
