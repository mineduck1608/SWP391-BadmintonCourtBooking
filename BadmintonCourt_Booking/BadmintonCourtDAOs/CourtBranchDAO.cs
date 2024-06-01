using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class CourtBranchDAO
    {
        private readonly BadmintonCourtContext _dbContext = null;

        public CourtBranchDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<CourtBranch> GetAllCourtBranches() => _dbContext.CourtBranches.ToList();

        public CourtBranch GetBranchById(int id) => _dbContext.CourtBranches.FirstOrDefault(x => x.BranchId == id);

        public List<CourtBranch> GetBranchesByLocation(string location) => _dbContext.CourtBranches.Where(x => x.Location.ToLower().Contains(location.ToLower())).ToList();

        public List<CourtBranch> GetBranchesByPhone(string phone) => new Regex(@"\d{5,11}").IsMatch(phone) ? _dbContext.CourtBranches.Where(x => x.BranchPhone.Contains(phone)).ToList() : null;
        
        public List<CourtBranch> GetBranchesByName(string name) => _dbContext.CourtBranches.Where(x => x.BranchName.ToLower().Contains(name.ToLower())).ToList();

        public List<CourtBranch> GetBranchesBySearchResult(string search) => _dbContext.CourtBranches.Where(x => x.Location.ToLower().Contains(search.ToLower()) || x.BranchName.ToLower().Contains(search.ToLower()) || x.BranchPhone.Contains(search)).ToList();

        public void AddBranch(CourtBranch branch)
        {
            _dbContext.CourtBranches.Add(branch);
            _dbContext.SaveChanges();
        }

        public void UpdateBranch(CourtBranch newBranch, int id)
        {
            CourtBranch tmp = GetBranchById(id);
            if (tmp != null)
            {
                tmp.Location = newBranch.Location;
                tmp.BranchPhone = newBranch.BranchPhone;
                tmp.BranchImg = newBranch.BranchImg; 
                _dbContext.CourtBranches.Update(tmp);
                _dbContext.SaveChanges();
            }
        }
        
        public void DeleteBranch(int id)
        {
            _dbContext.CourtBranches.Remove(GetBranchById(id));
            _dbContext.SaveChanges();
        }
    }
}
