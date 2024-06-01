using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IRoleService
    {
        public List<Role> GetAllRoles();

        public Role GetRoleById(int id);

        public List<Role> GetRolesByName(string name);

        public void UpdateRole(Role newRole, int id);

        public void AddRole(Role role);

        public void DeleteRole(int id);

    }
}
