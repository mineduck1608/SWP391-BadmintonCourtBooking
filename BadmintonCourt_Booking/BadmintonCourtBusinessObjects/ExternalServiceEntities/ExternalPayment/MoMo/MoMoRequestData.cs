using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.Momo
{
	public class MoMoRequestData
	{
		public string Endpoint { get; set; }
		public string OrderID { get; set; }
		public string OrderInfo { get; set; }
		public string AccessKey { get; set; }
		public string SecretKey { get; set; }
		public string PartnerCode { get; set; }
		public string RequestType { get; set; }
		public string ReturnUrl { get; set; }
		public string NotifyUrl { get; set; }
		public string Amount { get; set; }
		public string ExtraData { get; set; }


		public override string ToString()
		{
			return Endpoint + "|" + OrderID + "|" + OrderInfo + "|" + AccessKey + "|" + SecretKey + "|" + PartnerCode + "|" + RequestType + "|" + ReturnUrl + "|" + Amount + "|" + ExtraData;
		}
	}
}
