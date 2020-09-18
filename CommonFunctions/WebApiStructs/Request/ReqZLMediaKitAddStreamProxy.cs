using System;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqZLMediaKitAddStreamProxy:ReqZLMediaKitRequestBase
    {
        private string _vhost;
        private string _app;
        private string _stream;
        private string _url;
        private bool _enable_rtsp;
        private bool _enable_rtmp;
        private bool _enable_hls;
        private bool _enable_mp4;
        private int _rtp_type;

        public string Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string App
        {
            get => _app;
            set => _app = value;
        }

        public string Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string Url
        {
            get => _url;
            set => _url = value;
        }

        public bool Enable_Rtsp
        {
            get => _enable_rtsp;
            set => _enable_rtsp = value;
        }

        public bool Enable_Rtmp
        {
            get => _enable_rtmp;
            set => _enable_rtmp = value;
        }

        public bool Enable_Hls
        {
            get => _enable_hls;
            set => _enable_hls = value;
        }

        public bool Enable_Mp4
        {
            get => _enable_mp4;
            set => _enable_mp4 = value;
        }

        public int Rtp_Type
        {
            get => _rtp_type;
            set => _rtp_type = value;
        }
       
    }
}