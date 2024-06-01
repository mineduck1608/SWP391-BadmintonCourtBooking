using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtDAOs
{
    public class PaymentDAO
    {

        private readonly BadmintonCourtContext _dbContext = null;

        public PaymentDAO()
        {
            if (_dbContext == null)
            {
                _dbContext = new BadmintonCourtContext();
            }
        }

        public List<Payment> GetAllPayments() => _dbContext.Payments.ToList();

        public Payment GetPaymentByPaymentId(int id) => _dbContext.Payments.FirstOrDefault(x => x.PaymentId == id);

        public List<Payment> GetPaymentsByStatus(bool status) => _dbContext.Payments.Where(x => x.Status == status).ToList();

        public List<Payment> GetPaymentsByDate(DateTime date) => _dbContext.Payments.Where(x => x.Date == date).ToList();
        
        public List<Payment> GetPaymentsByUserId(int id) => _dbContext.Payments.Where(x => x.UserId == id).ToList();

        public Payment GetPaymentByBookingId(int id) => _dbContext.Payments.FirstOrDefault(x => x.BookingId == id);

        public void UpdatePayment(Payment newPayment, int id)
        {
            Payment tmp = GetPaymentByPaymentId(id);
            if (tmp != null)
            {
                tmp.Status = newPayment.Status;
                tmp.Date = newPayment.Date;
                _dbContext.Payments.Update(tmp);
                _dbContext.SaveChanges();
            }
        }

        public void AddPayment(Payment payment)
        {
            _dbContext.Payments.Add(payment);
            _dbContext.SaveChanges();
        }

        public void DeletePayment(int pId)
        {
            _dbContext.Payments.Remove(GetPaymentByPaymentId((pId)));
            _dbContext.SaveChanges();
        }
    }
}

