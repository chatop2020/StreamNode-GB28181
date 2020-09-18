using System;
using CommonFunctions.ManageStructs;
using GB28181.Sys.Model;

namespace CommonFunctions.WebApiStructs.Response
{
    /// <summary>
    /// 用于返回实时视频预览请求的结果
    /// </summary>
    [Serializable]
    public class ResLiveVideoResponse
    {
        private string? _deviceIpaddress;
        private string? _deivceid;
        private string? _cameraid;
        private string? _mediaServerId;
        private string? _mediaServerIpaddress;
        private int? _mediaServerPort;
        private string? _mediaId;
        private string? _vhost;
        private string? _app;
        private string? _play_Url;
        private PushStreamSocketType? _pushStreamSocketType;

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? DeviceIpaddress
        {
            get => _deviceIpaddress;
            set => _deviceIpaddress = value;
        }

        public string? Deivceid
        {
            get => _deivceid;
            set => _deivceid = value;
        }

        public string? Cameraid
        {
            get => _cameraid;
            set => _cameraid = value;
        }

        public string? MediaServerIpaddress
        {
            get => _mediaServerIpaddress;
            set => _mediaServerIpaddress = value;
        }

        public int? MediaServerPort
        {
            get => _mediaServerPort;
            set => _mediaServerPort = value;
        }

        public string? MediaId
        {
            get => _mediaId;
            set => _mediaId = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Play_Url
        {
            get => _play_Url;
            set => _play_Url = value;
        }

        public PushStreamSocketType? PushStreamSocketType
        {
            get => _pushStreamSocketType;
            set => _pushStreamSocketType = value;
        }
    }
}