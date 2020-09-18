using System;
using CommonFunctions.ManageStructs;
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

        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        public string? CameraDeptId
        {
            get => _cameraDeptId;
            set => _cameraDeptId = value;
        }

        public string? CameraDeptName
        {
            get => _cameraDeptName;
            set => _cameraDeptName = value;
        }

        public string? CameraPDeptId
        {
            get => _cameraPDeptId;
            set => _cameraPDeptId = value;
        }

        public string? CameraPDeptName
        {
            get => _cameraPDeptName;
            set => _cameraPDeptName = value;
        }

        public CameraType CameraType
        {
            get => _cameraType;
            set => _cameraType = value;
        }

        public ClientType ClientType
        {
            get => _clientType;
            set => _clientType = value;
        }

        public CameraEx? CameraEx
        {
            get => _cameraEx;
            set => _cameraEx = value;
        }

        public bool? IsOnline
        {
            get => _isOnline;
            set => _isOnline = value;
        }

        public bool? IsRecord
        {
            get => _isRecord;
            set => _isRecord = value;
        }

        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }

        public string? CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
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
        
    }
}