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
    public class RoleService : IRoleService
    {

        private readonly RoleDAO roleDAO = null;

        public RoleService()
        {
            if (roleDAO == null)
            {
                roleDAO = new RoleDAO();
            }
        }

        public void AddRole(Role role) => roleDAO.AddRole(role);

        public void DeleteRole(string id) => roleDAO.DeleteRole(id);

        public List<Role> GetAllRoles() =>  roleDAO.GetAllRoles();

        public Role GetRoleById(string id) => roleDAO.GetRoleById(id);

        public List<Role> GetRolesByName(string name) => roleDAO.GetRolesByName(name);

        public void UpdateRole(Role newRole, string id) => roleDAO.UpdateRole(newRole, id);

    }
}
