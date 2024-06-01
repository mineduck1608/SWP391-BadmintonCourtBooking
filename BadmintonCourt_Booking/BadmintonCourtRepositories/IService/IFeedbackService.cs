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

        public Feedback GetFeedbackByFeedbackId(int id);

        public List<Feedback> GetFeedbacksByContent(string content);

        public List<Feedback> GetFeedbacksByRate(int rate);

        public List<Feedback> GetA_UserFeedbacks(int id);

        public List<Feedback> GetBranchFeedbacks(int id);

        public void UpdateFeedback(Feedback newFeedback, int id);

        public void AddFeedback(Feedback feedback);

        public void DeleteFeedback(int id);
    }
}
