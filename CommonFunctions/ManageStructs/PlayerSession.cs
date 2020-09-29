using System;

namespace CommonFunction.ManageStructs
{
    [Serializable]
    public class PlayerSession
    {
        private string? _cameraId;
        private string? _mediaServerId;
        private ClientType _clientType;
        private string? _playUrl;
        private string? _playerIp;
        private double? _upTime;
        private DateTime _OnlineTime;
        private string? _vhost;
        private string? _app;
        private string? _streamId;
        private string? _mediaServerIp;
        private string? _sessionId;

        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public ClientType ClientType
        {
            get => _clientType;
            set => _clientType = value;
        }


        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }

        public string? PlayerIp
        {
            get => _playerIp;
            set => _playerIp = value;
        }

        public double? UpTime
        {
            get => _upTime;
            set => _upTime = value;
        }

        public DateTime OnlineTime
        {
            get => _OnlineTime;
            set => _OnlineTime = value;
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

        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        public string? MediaServerIp
        {
            get => _mediaServerIp;
            set => _mediaServerIp = value;
        }

        public string? SessionId
        {
            get => _sessionId;
            set => _sessionId = value;
        }
    }
}