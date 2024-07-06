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
    public class CourtService : ICourtService
    {
        private readonly CourtDAO _courtDAO = null;
        
        public CourtService()
        {
            if (_courtDAO == null)
            {
                _courtDAO = new CourtDAO();
            }
        }

        public CourtService(CourtDAO dao)
        {
            _courtDAO = dao;
        }
        public List<Court> GetAllCourts() => _courtDAO.GetAllCourts();

        public Court GetCourtByCourtId(string id) => _courtDAO.GetCourtByCourtId(id);

        public Court GetRecentAddedCourt() => _courtDAO.GetRecentAddedCourt();

        public List<Court> GetCourtsByBranchId(string id) => _courtDAO.GetCourtsByBranchId(id);

		public List<Court> GetCourtsByStatus(bool status) => _courtDAO.GetCourtsByStatus(status);

		public List<Court> GetCourtsByPriceInterval(double min, double max) => _courtDAO.GetCourtsByPriceInterval(min, max);

        public List<Court> GetCourtsBySearchResult(string search) => _courtDAO.GetCourtsBySearchResult(search);

        public void UpdateCourt(Court newCourt, string id) => _courtDAO.UpdateCourt(newCourt, id);

        public void DeleteCourt(string id) => _courtDAO.DeleteCourt(id);

        public void AddCourt(Court court) => _courtDAO.AddCourt(court);

    }
}
