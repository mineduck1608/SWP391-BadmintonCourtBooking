using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class CourtDAO
    {
        private readonly BadmintonCourtContext _dbContext = null;

        public CourtDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public Court GetRecentAddedCourt() => _dbContext.Courts.LastOrDefault();

        public List<Court> GetAllCourts() => _dbContext.Courts.ToList();

        public List<Court> GetCourtsByBranchId(int id) => _dbContext.Courts.Where(x => x.BranchId == id).ToList();

        public List<Court> GetCourtsByPriceInterval(float? min, float? max)
        {
            if (min == null)
                min = 0;
            if (max == null) 
                max = float.MaxValue;
            return _dbContext.Courts.Where(x => x.Price >= min && x.Price <= max).ToList();
        }

        public List<Court> GetCourtsBySearchResult(string search) => _dbContext.Courts.Where(x => x.Description.ToLower().Contains(search.ToLower()) || x.Branch.BranchName.ToLower().Contains(search.ToLower()) || x.Branch.Location.ToLower().Contains(search.ToLower())).ToList();

        public Court GetCourtByCourtId(int id) => _dbContext.Courts.FirstOrDefault(x => x.CourtId == id);

        

        public void UpdateCourt(Court newCourt, int cId)
        {
            Court tmp = GetCourtByCourtId(cId);
            if (tmp != null)
            {
                tmp.CourtImg = newCourt.CourtImg;
                tmp.Price = newCourt.Price;
                tmp.Description = newCourt.Description;
                tmp.BranchId = newCourt.BranchId;
                _dbContext.Courts.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddCourt(Court court)
        {
            _dbContext.Courts.Add(court);
            _dbContext.SaveChanges();
        }

        public void DeleteCourt(int cId)
        {
            _dbContext.Courts.Remove(GetCourtByCourtId(cId));
            _dbContext.SaveChanges();
        }
    }
}
