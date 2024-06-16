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
    public class FeedbackService : IFeedbackService
    {

        private readonly FeedbackDAO _feedbackDAO = null;

        public FeedbackService()
        {
            if (_feedbackDAO == null)
            {
                _feedbackDAO = new FeedbackDAO();
            }
        }

        public void AddFeedback(Feedback feedback) => _feedbackDAO.AddFeedback(feedback);

        public void DeleteFeedback(string id) => _feedbackDAO.DeleteFeedback(id);

        public List<Feedback> GetAllFeedbacks() => _feedbackDAO.GetAllFeedbacks();

        public List<Feedback> GetA_UserFeedbacks(string id) => _feedbackDAO.GetA_UserFeedbacks(id);

        public List<Feedback> GetBranchFeedbacks(string id) => _feedbackDAO.GetBranchFeedbacks(id);

        public Feedback GetFeedbackByFeedbackId(string id) => _feedbackDAO.GetFeedbackByFeedbackId(id);

        public List<Feedback> GetFeedbacksByContent(string content) => _feedbackDAO.GetFeedbacksByContent(content);

        public List<Feedback> GetFeedbacksByRate(int rate) => _feedbackDAO.GetFeedbacksByRate(rate);

        public void UpdateFeedback(Feedback newFeedback, string id) =>     _feedbackDAO.UpdateFeedback(newFeedback, id);

    }
}
