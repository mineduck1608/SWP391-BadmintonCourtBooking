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

        public CourtBranch GetBranchById(string id) => _dbContext.CourtBranches.FirstOrDefault(x => x.BranchId == id);

        public List<CourtBranch> GetBranchesByLocation(string location) => _dbContext.CourtBranches.Where(x => x.Location.ToLower().Contains(location.ToLower())).ToList();

        public List<CourtBranch> GetBranchesByPhone(string phone) => new Regex(@"\d{5,11}").IsMatch(phone) ? _dbContext.CourtBranches.Where(x => x.BranchPhone.Contains(phone)).ToList() : null;
        
        public List<CourtBranch> GetBranchesByName(string name) => _dbContext.CourtBranches.Where(x => x.BranchName.ToLower().Contains(name.ToLower())).ToList();

        public List<CourtBranch> GetBranchesBySearchResult(string search) => _dbContext.CourtBranches.Where(x => x.Location.ToLower().Contains(search.ToLower()) || x.BranchName.ToLower().Contains(search.ToLower()) || x.BranchPhone.Contains(search)).ToList();

        public List<CourtBranch> GetMaintainingBranches() => _dbContext.CourtBranches.Where(x => x.BranchStatus == -1).ToList();

        public void AddBranch(CourtBranch branch)
        {
            _dbContext.CourtBranches.Add(branch);
            _dbContext.SaveChanges();
        }

        public void UpdateBranch(CourtBranch newBranch, string id)
        {
            CourtBranch tmp = GetBranchById(id);
            if (tmp != null)
            {
                tmp.Location = newBranch.Location;
                tmp.BranchPhone = newBranch.BranchPhone;
                tmp.BranchImg = newBranch.BranchImg; 
                tmp.BranchName = newBranch.BranchName;
                tmp.BranchStatus = newBranch.BranchStatus;
                if (newBranch.BranchStatus == 0 || newBranch.BranchStatus == -1)
                {
                    List<Court> courts = _dbContext.Courts.Where(x => x.BranchId == id).ToList();
                    foreach (var item in courts)
                    {
                        item.CourtStatus = false;
                        _dbContext.Courts.Update(item);
                        _dbContext.SaveChanges();
                    }
                }
                _dbContext.CourtBranches.Update(tmp);
                _dbContext.SaveChanges();
            }
        }
        
        public void DeleteBranch(string id)
        {
            CourtBranch branch = GetBranchById(id);
            branch.BranchStatus = -1;
            List<Court> courts = _dbContext.Courts.Where(x => x.BranchId == id).ToList();
            foreach (var item in courts)
            {
                item.CourtStatus = false;
                _dbContext.Courts.Update(item);
                _dbContext.SaveChanges();
            }
            UpdateBranch(branch, id);
        }


    }
}
