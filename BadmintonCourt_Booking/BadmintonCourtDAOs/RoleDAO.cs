using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public  class RoleDAO
    {

        private readonly BadmintonCourtContext _dbContext = null;

        public RoleDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<Role> GetAllRoles() => _dbContext.Roles.ToList();

        public Role GetRoleById(int id) => _dbContext.Roles.FirstOrDefault(x => x.RoleId == id);

        public List<Role> GetRolesByName(string name) => _dbContext.Roles.Where(x => x.RoleName.ToLower().Contains(name.ToLower())).ToList();

        public void UpdateRole(Role newRole, int id)
        {
            Role tmp = GetRoleById(id);
            if (tmp != null)
            {
                tmp.RoleName = newRole.RoleName;
                _dbContext.Roles.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddRole(Role role)
        {
            _dbContext.Roles.Add(role);
            _dbContext.SaveChanges();
        }

        public void DeleteRole(int id)
        {
            _dbContext.Roles.Remove(GetRoleById((id)));
            _dbContext.SaveChanges();
        }
    }
}

