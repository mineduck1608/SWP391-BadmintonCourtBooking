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
        private readonly CourtDAO courtDAO = null;
        
        public CourtService()
        {
            if (courtDAO == null)
            {
                courtDAO = new CourtDAO();
            }
        }
        public List<Court> GetAllCourts() => courtDAO.GetAllCourts();

        public Court GetCourtByCourtId(string id) => courtDAO.GetCourtByCourtId(id);

        public Court GetRecentAddedCourt() => courtDAO.GetRecentAddedCourt();

        public List<Court> GetCourtsByBranchId(string id) => courtDAO.GetCourtsByBranchId(id);

        public List<Court> GetCourtsByPriceInterval(float min, float max) => courtDAO.GetCourtsByPriceInterval(min, max);

        public List<Court> GetCourtsBySearchResult(string search) => courtDAO.GetCourtsBySearchResult(search);

        public void UpdateCourt(Court newCourt, string id) => courtDAO.UpdateCourt(newCourt, id);

        public void DeleteCourt(string id) => courtDAO.DeleteCourt(id);

        public void AddCourt(Court court) => courtDAO.AddCourt(court);

    }
}
