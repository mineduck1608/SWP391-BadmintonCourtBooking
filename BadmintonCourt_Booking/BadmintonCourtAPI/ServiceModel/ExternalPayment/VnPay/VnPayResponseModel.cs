namespace BadmintonCourtAPI.ServiceModel.ExternalPayment.VNPay
{
	public class VNPayResponseModel
	{
		public bool Success { get; set; }
		public string PaymentMethod { get; set; }
		public string OrderDescription { get; set; }
		public string OrderId { get; set; }
		public string PaymentId { get; set; }
		public string TransactionId { get; set; }
		public string Token { get; set; }
		public string VnPayResponseCode { get; set; }

		public VNPayResponseModel(bool success, string paymentMethod, string orderDescription, string orderId, string paymentId, string transactionId, string token, string vnPayResponseCode)
		{
			Success = success;
			PaymentMethod = paymentMethod;
			OrderDescription = orderDescription;
			OrderId = orderId;
			PaymentId = paymentId;
			TransactionId = transactionId;
			Token = token;
			VnPayResponseCode = vnPayResponseCode;
		}

	}
}
