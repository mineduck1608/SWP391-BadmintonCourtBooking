using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.VnPay;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface IVnPayService
	{
		public string CreatePaymentUrl(HttpContext context, VnPayRequestDTO vnPayRequestDTO);

		public VnPayResponseDTO PaymentExecute(IQueryCollection collection);

	}
}
