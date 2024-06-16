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

        public List<Court> GetCourtsByBranchId(string id) => _dbContext.Courts.Where(x => x.BranchId == id).ToList();

        public List<Court> GetCourtsByPriceInterval(float min, float max) =>  _dbContext.Courts.Where(x => x.Price >= min && x.Price <= max).ToList();
        
        public List<Court> GetCourtsBySearchResult(string search) => _dbContext.Courts.Where(x => x.Description.ToLower().Contains(search.ToLower()) || x.Branch.BranchName.ToLower().Contains(search.ToLower()) || x.Branch.Location.ToLower().Contains(search.ToLower())).ToList();

        public Court GetCourtByCourtId(string id) => _dbContext.Courts.FirstOrDefault(x => x.CourtId == id);

        public void UpdateCourt(Court newCourt, string id)
        {
            Court tmp = GetCourtByCourtId(id);
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

        public void DeleteCourt(string id)
        {
            Court court = GetCourtByCourtId(id);
            court.CourtStatus = false;
            UpdateCourt(court, id);
        }
    }
}
