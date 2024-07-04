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

        private readonly RoleDAO _roleDAO = null;

        public RoleService()
        {
            if (_roleDAO == null)
            {
                _roleDAO = new RoleDAO();
            }
        }

        public RoleService(RoleDAO dao)
        {
            _roleDAO = dao;
        }

        public void AddRole(Role role) => _roleDAO.AddRole(role);

        public void DeleteRole(string id) => _roleDAO.DeleteRole(id);

        public List<Role> GetAllRoles() =>  _roleDAO.GetAllRoles();

        public Role GetRoleById(string id) => _roleDAO.GetRoleById(id);

        public List<Role> GetRolesByName(string name) => _roleDAO.GetRolesByName(name);

        public void UpdateRole(Role newRole, string id) => _roleDAO.UpdateRole(newRole, id);

    }
}
