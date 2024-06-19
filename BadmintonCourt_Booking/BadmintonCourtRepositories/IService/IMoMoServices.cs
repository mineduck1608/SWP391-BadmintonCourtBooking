using BadmintonCourtBusinessObjects.ExternalServiceEntities.ExternalPayment.MoMo;

namespace BadmintonCourtServices.IService
{
    public interface IMoMoServices
    {
        public static abstract string CreateRawData(MoMoRequestData request);
        public static abstract MoMoRequestData CreateRequestData(string orderInfo, string amount, string returnUrl);
        public static abstract string CreateRequestBody(MoMoRequestData request);
        public static abstract Task<MoMoResponse> SendMoMoRequest(MoMoRequestData request);
        public static abstract string SignSHA256(string msg, string secretKey);
    }
}