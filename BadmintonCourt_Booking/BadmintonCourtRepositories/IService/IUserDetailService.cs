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

        public UserDetail GetUserDetailById(int id);

        public List<UserDetail> GetUserDetailsByName(string name);

        public UserDetail GetUserDetailByMail(string mail);

        public List<UserDetail> GetUserDetailsBySearchResult(string search);

        public void UpdateUserDetail(UserDetail newUserDetail, int id);

        public void AddUserDetail(UserDetail userDetail);

        public void DeleteUserDetail(int id);

    }
}
