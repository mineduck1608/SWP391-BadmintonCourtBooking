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

        public Payment GetPaymentByPaymentId(string id);

        public List<Payment> GetPaymentsByDate(DateTime date);

        public List<Payment> GetPaymentsBySearch(string? id, string? search);


		public List<Payment> GetPaymentsByUserId(string id);

        public Payment GetPaymentByBookingId(string id);

		public void UpdatePayment(Payment newPayment, string id);

        public void AddPayment(Payment payment);

        public void DeletePayment(string id);

    }
}
