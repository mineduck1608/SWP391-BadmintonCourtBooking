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
    public class CourtBranchService : ICourtBranchService
    {

        private readonly CourtBranchDAO _branchDAO = null;

        public CourtBranchService()
        {
            if (_branchDAO == null)
                _branchDAO= new CourtBranchDAO();
        }

        public void AddBranch(CourtBranch branch) => _branchDAO.AddBranch(branch);

        public void DeleteBranch(string id) => _branchDAO.DeleteBranch(id);

        public List<CourtBranch> GetAllCourtBranches() => _branchDAO.GetAllCourtBranches();

        public CourtBranch GetBranchById(string id) => _branchDAO.GetBranchById(id);

        public List<CourtBranch> GetBranchesByLocation(string location) => _branchDAO.GetBranchesByLocation(location);

        public List<CourtBranch> GetBranchesByName(string name) => _branchDAO.GetBranchesByName(name);

        public List<CourtBranch> GetBranchesByPhone(string phone) => _branchDAO.GetBranchesByPhone(phone);

        public List<CourtBranch> GetBranchesBySearchResult(string search) => _branchDAO.GetBranchesBySearchResult(search);

        public void UpdateBranch(CourtBranch newBranch, string id) =>  _branchDAO.UpdateBranch(newBranch, id);

    }
}
