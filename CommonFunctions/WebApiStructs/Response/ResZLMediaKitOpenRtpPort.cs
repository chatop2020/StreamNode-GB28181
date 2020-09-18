using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitOpenRtpPort : ResZLMediaKitResponseBase
    {
        private ushort? _port;


        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }
    }
}