using System;

namespace CommonFunction.ManageStructs
{
    /// <summary>
    /// 播放者结构
    /// </summary>
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

        /// <summary>
        /// 摄像头实例ID
        /// </summary>
        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 客户端类型
        ///  Camera =摄像头
        ///  Player =播放者
        /// Livecast=直播者
        /// </summary>
        public ClientType ClientType
        {
            get => _clientType;
            set => _clientType = value;
        }


        /// <summary>
        /// 播放的URL地址
        /// </summary>
        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }

        /// <summary>
        /// 播放者ip地址
        /// </summary>
        public string? PlayerIp
        {
            get => _playerIp;
            set => _playerIp = value;
        }

        /// <summary>
        /// 在线时长（秒）
        /// </summary>
        public double? UpTime
        {
            get => _upTime;
            set => _upTime = value;
        }

        /// <summary>
        /// 最后一次在线时间
        /// </summary>
        public DateTime OnlineTime
        {
            get => _OnlineTime;
            set => _OnlineTime = value;
        }

        /// <summary>
        /// 流媒体Vhost标记
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// 流媒体APP标记
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 流媒体StreamId标记
        /// </summary>
        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        /// <summary>
        /// 流媒体服务器IP地址
        /// </summary>
        public string? MediaServerIp
        {
            get => _mediaServerIp;
            set => _mediaServerIp = value;
        }

        /// <summary>
        /// 客户端在流媒体服务器中的SessionId
        /// </summary>
        public string? SessionId
        {
            get => _sessionId;
            set => _sessionId = value;
        }
    }
}