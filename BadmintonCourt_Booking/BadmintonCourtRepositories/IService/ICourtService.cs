using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface ICourtService
    {
        public List<Court> GetAllCourts();
        public List<Court> GetCourtsByBranchId(string id);
        public List<Court> GetCourtsByPriceInterval(double min, double max);
        public List<Court> GetCourtsBySearchResult(string search);

        public List<Court> GetCourtsByStatus(bool status);

		public Court GetCourtByCourtId(string id);
        public Court GetRecentAddedCourt();
        public void UpdateCourt(Court newCourt, string id);
        public void DeleteCourt(string id);
        public void AddCourt(Court court);
    }
}
