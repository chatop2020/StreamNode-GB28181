using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResMediaServerWebApiReg
    {
        private ushort _mediaServerHttpPort;
        private ushort _webApiServerhttpPort;
        private string? _ipaddress;
        private string _mediaServerId;
        private string _secret;


        public ushort MediaServerHttpPort
        {
            get => _mediaServerHttpPort;
            set => _mediaServerHttpPort = value;
        }

        public ushort WebApiServerhttpPort
        {
            get => _webApiServerhttpPort;
            set => _webApiServerhttpPort = value;
        }

        public string? Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value;
        }


        public string MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string Secret
        {
            get => _secret;
            set => _secret = value;
        }
    }
}