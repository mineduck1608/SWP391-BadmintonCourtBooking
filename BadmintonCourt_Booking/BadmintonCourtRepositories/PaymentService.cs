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
    public class PaymentService : IPaymentService
    {

        private readonly PaymentDAO paymentDAO = null;

        public PaymentService()
        {
            if (paymentDAO == null)
            {
                paymentDAO = new PaymentDAO();
            }
        }

        public void AddPayment(Payment payment) => paymentDAO.AddPayment(payment);

		public void DeletePayment(int id) => paymentDAO.DeletePayment(id);

        public List<Payment> GetAllPayments() => paymentDAO.GetAllPayments();   

        public Payment GetPaymentByBookingId(int id) => paymentDAO.GetPaymentByBookingId(id);

        public Payment GetPaymentByPaymentId(int id) => paymentDAO.GetPaymentByPaymentId(id);

        public List<Payment> GetPaymentsByDate(DateTime date) => paymentDAO.GetPaymentsByDate(date);

        public List<Payment> GetPaymentsByStatus(bool status) => paymentDAO.GetPaymentsByStatus(status);

        public List<Payment> GetPaymentsByUserId(int id) => paymentDAO.GetPaymentsByUserId(id);

        public void UpdatePayment(Payment newPayment, int id) =>    paymentDAO.UpdatePayment(newPayment, id);

    }
}
