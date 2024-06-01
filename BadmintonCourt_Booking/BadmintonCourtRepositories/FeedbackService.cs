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

        private readonly FeedbackDAO feedbackDAO = null;

        public FeedbackService()
        {
            if (feedbackDAO == null)
            {
                feedbackDAO = new FeedbackDAO();
            }
        }

        public void AddFeedback(Feedback feedback) => feedbackDAO.AddFeedback(feedback);

        public void DeleteFeedback(int id) => feedbackDAO.DeleteFeedback(id);

        public List<Feedback> GetAllFeedbacks() => feedbackDAO.GetAllFeedbacks();

        public List<Feedback> GetA_UserFeedbacks(int id) => feedbackDAO.GetA_UserFeedbacks(id);

        public List<Feedback> GetBranchFeedbacks(int id) => feedbackDAO.GetBranchFeedbacks(id);

        public Feedback GetFeedbackByFeedbackId(int id) => feedbackDAO.GetFeedbackByFeedbackId(id);

        public List<Feedback> GetFeedbacksByContent(string content) => feedbackDAO.GetFeedbacksByContent(content);

        public List<Feedback> GetFeedbacksByRate(int rate) => feedbackDAO.GetFeedbacksByRate(rate);

        public void UpdateFeedback(Feedback newFeedback, int id) =>     feedbackDAO.UpdateFeedback(newFeedback, id);

    }
}
