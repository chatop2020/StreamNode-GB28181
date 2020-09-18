using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResToWebHookOnStreamChange : ResZLMediaKitResponseBase
    {
        private string? _msg;

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}