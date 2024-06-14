using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IUserDetailService
    {

        public List<UserDetail> GetAllUserDetails();

        public UserDetail GetUserDetailById(string id);

        public List<UserDetail> GetUserDetailsByName(string name);

        public UserDetail GetUserDetailByMail(string mail);

        public List<UserDetail> GetUserDetailsBySearchResult(string search);

        public void UpdateUserDetail(UserDetail newUserDetail, string id);

        public void AddUserDetail(UserDetail userDetail);

        public void DeleteUserDetail(string id);

    }
}
