using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface IVnPayService
	{
		public string CreatePaymentUrl(HttpContent content);


	}
}
