using System;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqForWebHookOnPublish
    {
        private string? _app;
        private string? _id;
        private string? _ip;
        private string? _mediaserverid;
        private string? _params;
        private ushort? _port;
        private string? _schema;
        private string? _stream;
        private string? _vhost;

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Id
        {
            get => _id;
            set => _id = value;
        }


        public string? Ip
        {
            get => _ip;
            set => _ip = value;
        }

        public string? Mediaserverid
        {
            get => _mediaserverid;
            set => _mediaserverid = value;
        }

        public string? Params
        {
            get => _params;
            set => _params = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }
    }
}