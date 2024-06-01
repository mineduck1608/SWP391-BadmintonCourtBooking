using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface ICourtBranchService
    {
        public List<CourtBranch> GetAllCourtBranches();

        public CourtBranch GetBranchById(int id);

        public List<CourtBranch> GetBranchesByLocation(string location);

        public List<CourtBranch> GetBranchesByPhone(string phone);

        public List<CourtBranch> GetBranchesByName(string name);

        public List<CourtBranch> GetBranchesBySearchResult(string search);

        public void AddBranch(CourtBranch branch);

        public void UpdateBranch(CourtBranch newBranch, int id);

        public void DeleteBranch(int id);
    }
}
