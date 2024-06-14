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

        private readonly UserDetailDAO userDetailDAO = null;

        public UserDetailService()
        {
            if (userDetailDAO == null)
            {
                userDetailDAO = new UserDetailDAO();
            }
        }

        public void AddUserDetail(UserDetail userDetail) => userDetailDAO.AddUserDetail(userDetail);

        public void DeleteUserDetail(string id) => userDetailDAO.DeleteUserDetail(id); 

        public List<UserDetail> GetAllUserDetails() =>  userDetailDAO.GetAllUserDetails();

        public UserDetail GetUserDetailById(string id) => userDetailDAO.GetUserDetailById(id);

        public UserDetail GetUserDetailByMail(string mail) => userDetailDAO.GetUserDetailByMail(mail);

        public List<UserDetail> GetUserDetailsByName(string name) => userDetailDAO.GetUserDetailsByName(name);

        public List<UserDetail> GetUserDetailsBySearchResult(string search) => userDetailDAO.GetUserDetailsBySearchResult(search);

        public void UpdateUserDetail(UserDetail newUserDetail, string id) => userDetailDAO.UpdateUserDetail(newUserDetail, id);

    }
}
