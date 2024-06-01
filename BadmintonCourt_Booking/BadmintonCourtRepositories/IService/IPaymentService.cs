using BadmintonCourtBusinessObjects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
    public interface IPaymentService
    {

        public List<Payment> GetAllPayments();

        public Payment GetPaymentByPaymentId(int id);

        public List<Payment> GetPaymentsByStatus(bool status);

        public List<Payment> GetPaymentsByDate(DateTime date);

        public List<Payment> GetPaymentsByUserId(int id);

        public Payment GetPaymentByBookingId(int id);

        public void UpdatePayment(Payment newPayment, int id);

        public void AddPayment(Payment payment);

        public void DeletePayment(int id);

    }
}
