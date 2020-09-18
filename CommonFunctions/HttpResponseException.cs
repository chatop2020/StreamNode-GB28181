using System;
using CommonFunctions.ManageStructs;
using LibGB28181SipGate;

namespace CommonFunctions
{
    public class HttpResponseException : Exception
    {
        public ResponseStruct rs { get; set; }

        public HttpResponseException(string message) : base(message)
        {
            rs = JsonHelper.FromJson<ResponseStruct>(message);
        }
    }
}