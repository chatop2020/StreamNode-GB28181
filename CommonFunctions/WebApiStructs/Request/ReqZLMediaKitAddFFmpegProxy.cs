using System;
using System.Text.Json.Serialization;
using GB28181.Servers.SIPMonitor;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqZLMediaKitAddFFmpegProxy : ReqZLMediaKitRequestBase
    {
        private string? _src_url;
        private string? _serverip;
        private string? _app;
        private string? _streamId;
        private string? _dst_url;
        private int? _timeout_ms;

        public string? Src_Url
        {
            get => _src_url;
            set => _src_url = value;
        }

        [JsonIgnore]
        public string? Serverip
        {
            get => _serverip;
            set => _serverip = value;
        }

        [JsonIgnore]
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        [JsonIgnore]
        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        public string? Dst_Url
        {
            get => _dst_url;
            set => _dst_url = value;
        }

        public int? Timeout_Ms
        {
            get => _timeout_ms;
            set => _timeout_ms = value;
        }

        public ReqZLMediaKitAddFFmpegProxy(string? srcUrl, string serverip)
        {
            _src_url = srcUrl;
            _serverip = serverip;
            _app = "rtsp_proxy";
            _streamId = string.Format("{0:X8}", CRC32Cls.GetCRC32(_src_url));
            _dst_url = "rtmp://" + _serverip + "/" + _app + "/" + _streamId;
            _timeout_ms = 20000;
        }
    }
}