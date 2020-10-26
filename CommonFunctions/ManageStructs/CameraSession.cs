using System;
using GB28181.Sys.Model;

namespace CommonFunction.ManageStructs
{
    [Serializable]
    public enum ClientType
    {
        Camera,
        Player,
        Livecast,
    }

    [Serializable]
    public class CameraSession
    {
        private string? _cameraId;
        private string? _mediaServerId;
        private string? _cameraName;
        private string? _cameraDeptId;
        private string? _cameraDeptName;
        private string? _cameraPDeptId;
        private string? _cameraPDeptName;
        private CameraType _cameraType;
        private ClientType _clientType;
        private CameraEx? _cameraEx = null;
        private bool? _isOnline;
        private bool? _isRecord;
        private string? _playUrl;
        private string? _cameraIpAddress;
        private double? _upTime;
        private DateTime _OnlineTime;
        private string? _vhost;
        private string? _app;
        private string? _streamId;
        private string? _mediaServerIp;
        private bool? _forceOffline = false;


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
        /// 摄像头实例名称
        /// </summary>
        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        /// <summary>
        /// 摄像头部门代码
        /// </summary>
        public string? CameraDeptId
        {
            get => _cameraDeptId;
            set => _cameraDeptId = value;
        }

        /// <summary>
        /// 摄像头部门名称
        /// </summary>
        public string? CameraDeptName
        {
            get => _cameraDeptName;
            set => _cameraDeptName = value;
        }

        /// <summary>
        /// 摄像头父部门代码
        /// </summary>
        public string? CameraPDeptId
        {
            get => _cameraPDeptId;
            set => _cameraPDeptId = value;
        }

        /// <summary>
        /// 摄像头父部门名称
        /// </summary>
        public string? CameraPDeptName
        {
            get => _cameraPDeptName;
            set => _cameraPDeptName = value;
        }

        /// <summary>
        /// 摄像头类型
        /// GB28181=GB28181设备
        /// Rtsp=RTSP设备
        /// LiveCast=直播设备
        /// None=未知设备
        /// </summary>
        public CameraType CameraType
        {
            get => _cameraType;
            set => _cameraType = value;
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
        /// 摄像头详细信息
        /// </summary>
        public CameraEx? CameraEx
        {
            get => _cameraEx;
            set => _cameraEx = value;
        }

        /// <summary>
        /// 摄像头是否在线
        /// </summary>
        public bool? IsOnline
        {
            get => _isOnline;
            set => _isOnline = value;
        }

        /// <summary>
        /// 摄像头是否正在录制音视频文件
        /// </summary>
        public bool? IsRecord
        {
            get => _isRecord;
            set => _isRecord = value;
        }

        /// <summary>
        /// 摄像头的音视频流播放地址
        /// </summary>
        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }

        /// <summary>
        /// 摄像头IP地址
        /// </summary>
        public string? CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

        /// <summary>
        /// 摄像头的在线时长（秒）
        /// </summary>
        public double? UpTime
        {
            get => _upTime;
            set => _upTime = value;
        }

        /// <summary>
        /// 摄像头最近的上线时间
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
        /// 强制下线，此参数为true时，cameraKeeper中不再使这个摄像头上线，否则导致重复上线的问题（暂时无用）
        /// </summary>
        public bool? ForceOffline
        {
            get => _forceOffline;
            set => _forceOffline = value;
        }
    }
}