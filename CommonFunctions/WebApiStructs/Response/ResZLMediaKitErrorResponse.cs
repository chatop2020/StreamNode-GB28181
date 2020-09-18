using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitErrorResponse : ResZLMediaKitResponseBase
    {
        private string _msg;
        private int? _result;

        public string Msg
        {
            get => _msg;
            set => _msg = value;
        }

        public int? Result
        {
            get => _result;
            set => _result = value;
        }
    }
}