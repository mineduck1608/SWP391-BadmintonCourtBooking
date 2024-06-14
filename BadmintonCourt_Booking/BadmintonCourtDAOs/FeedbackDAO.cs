using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class FeedbackDAO
    {
        private readonly BadmintonCourtContext _dbContext = null;

        public FeedbackDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<Feedback> GetAllFeedbacks() => _dbContext.Feedbacks.ToList();

        public Feedback GetFeedbackByFeedbackId(string id) => _dbContext.Feedbacks.FirstOrDefault(x => x.FeedbackId == id);

        public List<Feedback> GetFeedbacksByContent(string content) => _dbContext.Feedbacks.Where(x => x.Content.ToLower().Contains(content.ToLower())).ToList();

        public List<Feedback> GetFeedbacksByRate(int rate) => _dbContext.Feedbacks.Where(x => x.Rate == rate).ToList();

        public List<Feedback> GetA_UserFeedbacks(string id) => _dbContext.Feedbacks.Where(x => x.UserId == id).ToList();

        public List<Feedback> GetBranchFeedbacks(string id) => _dbContext.Feedbacks.Where(x => x.BranchId == id).ToList();

        public void UpdateFeedback(Feedback newFeedback, string id)
        {
            Feedback tmp = GetFeedbackByFeedbackId(id);
            if (tmp != null)
            {
                tmp.Rate = newFeedback.Rate;
                tmp.Content = newFeedback.Content;
                _dbContext.Feedbacks.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddFeedback(Feedback feedback)
        {
            _dbContext.Feedbacks.Add(feedback);
            _dbContext.SaveChanges();
        }

        public void DeleteFeedback(string id)
        {
            _dbContext.Feedbacks.Remove(GetFeedbackByFeedbackId((id)));
            _dbContext.SaveChanges();
        }
    }
}
