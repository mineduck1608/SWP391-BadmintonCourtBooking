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

        private readonly PaymentDAO _paymentDAO = null;

        public PaymentService()
        {
            if (_paymentDAO == null)
            {
                _paymentDAO = new PaymentDAO();
            }
        }

        public void AddPayment(Payment payment) => _paymentDAO.AddPayment(payment);

		public void DeletePayment(string id) => _paymentDAO.DeletePayment(id);

        public List<Payment> GetAllPayments() => _paymentDAO.GetAllPayments();

        public List<Payment> GetPaymentsBySearch(string? id, string? search) => _paymentDAO.GetPaymentsBySearch(id, search);

		public Payment GetPaymentByBookingId(string id) => _paymentDAO.GetPaymentByBookingId(id);

        public Payment GetPaymentByPaymentId(string id) => _paymentDAO.GetPaymentByPaymentId(id);

        public List<Payment> GetPaymentsByDate(DateTime date) => _paymentDAO.GetPaymentsByDate(date);

        public List<Payment> GetPaymentsByUserId(string id) => _paymentDAO.GetPaymentsByUserId(id);

        public void UpdatePayment(Payment newPayment, string id) =>    _paymentDAO.UpdatePayment(newPayment, id);

    }
}
