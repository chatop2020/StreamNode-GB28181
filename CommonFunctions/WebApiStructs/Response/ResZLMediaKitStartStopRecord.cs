using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitStartStopRecord : ResZLMediaKitResponseBase
    {
        private string? _msg;
        private bool? _result;

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }

        public bool? Result
        {
            get => _result;
            set => _result = value;
        }
    }
}