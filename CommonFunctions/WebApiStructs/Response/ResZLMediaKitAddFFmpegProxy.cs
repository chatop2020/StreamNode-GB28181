using System;
using System.Collections.Generic;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class Data
    {
        private string? _key;

        public string? Key
        {
            get => _key;
            set => _key = value;
        }
    }

    [Serializable]
    public class ResZLMediaKitAddFFmpegProxy : ResZLMediaKitResponseBase
    {
        private string? _app;
        private string? _serverip;
        private ushort? _httpserverport;
        private string? _streamId;
        private string? _src_url;
        private string? _vhost;
        private DateTime? _updateTime;
        private Data _data;
        private List<MediaDataItem> _mediaInfo = new List<MediaDataItem>();


        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Serverip
        {
            get => _serverip;
            set => _serverip = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public ushort? Httpserverport
        {
            get => _httpserverport;
            set => _httpserverport = value;
        }


        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        public string? Src_Url
        {
            get => _src_url;
            set => _src_url = value;
        }

        public string Play_Url
        {
            get { return "http://" + _serverip + ":" + _httpserverport + "/" + _app + "/" + _streamId + ".flv"; }
        }

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        public Data Data
        {
            get => _data;
            set => _data = value;
        }

        public List<MediaDataItem> MediaInfo
        {
            get => _mediaInfo;
            set => _mediaInfo = value;
        }
    }
}