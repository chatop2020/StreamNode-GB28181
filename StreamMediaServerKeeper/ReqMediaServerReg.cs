using System;
using LibSystemInfo;

namespace StreamMediaServerKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ReqMediaServerReg
    {
        private ushort? _mediaServerHttpPort;
        private ushort? _webApiServerhttpPort;
        private string? _ipaddress;
        private string? _mediaServerId;
        private string? _secret;
        private string? _recordFilePath;
        private GlobalSystemInfo? _mediaServerSystemInfo;

        /// <summary>
        /// 
        /// </summary>
        public ushort? MediaServerHttpPort
        {
            get => _mediaServerHttpPort;
            set => _mediaServerHttpPort = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public ushort? WebApiServerhttpPort
        {
            get => _webApiServerhttpPort;
            set => _webApiServerhttpPort = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value;
        }


        /// <summary>
        /// 
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? Secret
        {
            get => _secret;
            set => _secret = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string? RecordFilePath
        {
            get => _recordFilePath;
            set => _recordFilePath = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public GlobalSystemInfo? MediaServerSystemInfo
        {
            get => _mediaServerSystemInfo;
            set => _mediaServerSystemInfo = value;
        }
    }
}