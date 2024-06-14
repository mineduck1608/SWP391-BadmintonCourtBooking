using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IFeedbackService
    {

        public List<Feedback> GetAllFeedbacks();

        public Feedback GetFeedbackByFeedbackId(string id);

        public List<Feedback> GetFeedbacksByContent(string content);

        public List<Feedback> GetFeedbacksByRate(int rate);

        public List<Feedback> GetA_UserFeedbacks(string id);

        public List<Feedback> GetBranchFeedbacks(string id);

        public void UpdateFeedback(Feedback newFeedback, string id);

        public void AddFeedback(Feedback feedback);

        public void DeleteFeedback(string id);
    }
}
