using System;

namespace AntaresClientApi.Domain.Models.Exceptions
{
    public class MatchingEngineException : Exception
    {
        public string MeStatusCode { get; set; }
        public string MeReason { get; set; }

        public string MeMethod { get; set; }

        public string MeService { get; set; }

        public object MeRequest { get; set; }

        public object MeResponse { get; set; }

        public MatchingEngineException(
            string meStatusCode, 
            string meReason, 
            string meMethod,
            string meService,
            object meRequest,
            object meResponse) : base($"ME Error. Method: {meService}.{meMethod}; Code: {meStatusCode}; Reason: {meReason};")
        {
            MeStatusCode = meStatusCode;
            MeReason = meReason;
            MeMethod = meMethod;
            MeService = meService;
            MeRequest = meRequest;
            MeResponse = meResponse;
        }
    }
}
