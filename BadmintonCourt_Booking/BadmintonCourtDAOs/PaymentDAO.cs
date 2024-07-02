using BadmintonCourtBusinessDAOs;
using BadmintonCourtBusinessObjects.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
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

        public PaymentDAO(BadmintonCourtContext context)
        {
            _dbContext = context;
        }

        public List<Payment> GetAllPayments() => _dbContext.Payments.ToList();

        public Payment GetPaymentByPaymentId(string id) => _dbContext.Payments.FirstOrDefault(x => x.PaymentId == id);

        public List<Payment> GetPaymentsByDate(DateTime date) => _dbContext.Payments.Where(x => x.Date == date).ToList();
        
        public List<Payment> GetPaymentsByUserId(string id) => _dbContext.Payments.Where(x => x.UserId == id).ToList();

        public List<Payment> GetPaymentsBySearch(string? id, string? search)
        {
            if (!id.IsNullOrEmpty()) // user tìm 
            {
                if (!search.IsNullOrEmpty())  
                    return GetPaymentsByUserId(id).Where(x => x.TransactionId == search || x.PaymentId == search).ToList();
                return GetPaymentsByUserId(id);
			}

            if (!search.IsNullOrEmpty()) // Admin, staff tìm
                return GetAllPayments().Where(x => x.TransactionId == search || x.PaymentId == search).ToList();
            return GetAllPayments();
		}

        public Payment GetPaymentByBookingId(string id) => _dbContext.Payments.FirstOrDefault(x => x.BookingId == id);

        public void UpdatePayment(Payment newPayment, string id)
        {
            Payment tmp = GetPaymentByPaymentId(id);
            if (tmp != null)
            {
                tmp.Method = newPayment.Method;
                tmp.TransactionId = newPayment.TransactionId;
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

        public void DeletePayment(string id)
        {
            _dbContext.Payments.Remove(GetPaymentByPaymentId(id));
            _dbContext.SaveChanges();
        }
    }
}

