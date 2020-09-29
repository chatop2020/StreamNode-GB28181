using System;

namespace StreamMediaServerKeeper
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