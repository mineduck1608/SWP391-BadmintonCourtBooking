using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.Momo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadmintonCourtServices.IService
{
	public interface IMoMoService
	{
		public string CreateRawData(MoMoRequestData request);
		public MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl);
		public string CreateRequestBody(MoMoRequestData request);
		public Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request);
		public string SignSHA256(string msg, string secretKey);
	}
}
