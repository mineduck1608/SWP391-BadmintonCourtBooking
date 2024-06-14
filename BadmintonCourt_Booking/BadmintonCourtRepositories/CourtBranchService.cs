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

        private readonly CourtBranchDAO courtBranchDAO = null;

        public CourtBranchService()
        {
            if (courtBranchDAO == null)
                courtBranchDAO= new CourtBranchDAO();
        }

        public void AddBranch(CourtBranch branch) => courtBranchDAO.AddBranch(branch);

        public void DeleteBranch(string id) => courtBranchDAO.DeleteBranch(id);

        public List<CourtBranch> GetAllCourtBranches() => courtBranchDAO.GetAllCourtBranches();

        public CourtBranch GetBranchById(string id) => courtBranchDAO.GetBranchById(id);

        public List<CourtBranch> GetBranchesByLocation(string location) => courtBranchDAO.GetBranchesByLocation(location);

        public List<CourtBranch> GetBranchesByName(string name) => courtBranchDAO.GetBranchesByName(name);

        public List<CourtBranch> GetBranchesByPhone(string phone) => courtBranchDAO.GetBranchesByPhone(phone);

        public List<CourtBranch> GetBranchesBySearchResult(string search) => courtBranchDAO.GetBranchesBySearchResult(search);

        public void UpdateBranch(CourtBranch newBranch, string id) =>  courtBranchDAO.UpdateBranch(newBranch, id);

    }
}
