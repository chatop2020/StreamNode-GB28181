using System;
using System.Collections.Generic;

namespace GB28181.Sys.Model
{
    [Serializable]
    public enum SipDeviceStatus
    {
        Register, //注册完成
        UnRegister, //注销了
        GetDeviceList, //获取设备列表中
        LooksLikeOffline, //看起来像离线了
    }

    [Serializable]
    public enum PushStreamSocketType
    {
        UDP,
        TCP
    }


    [Serializable]
    public enum SipCameraStatus
    {
        RealVideo,
        Idle,
    }

    [Serializable]
    public class CameraEx
    {
        private string? _mediaServerId;
        private string? _vhost;
        private string? _app;
        private uint? _streamId;
        private string? _streamServerIp;
        private int? _streamServerPort;
        private SipCameraStatus? _sipCameraStatus;
        private CameraType _ctype;
        private PushStreamSocketType? _pushStreamSocketType;
        private Camera _camera;
        private string? _inputUrl;


        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public CameraType Ctype
        {
            get => _ctype;
            set => _ctype = value;
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

        public uint? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        public string? StreamServerIp
        {
            get => _streamServerIp;
            set => _streamServerIp = value;
        }

        public int? StreamServerPort
        {
            get => _streamServerPort;
            set => _streamServerPort = value;
        }


        public SipCameraStatus? SipCameraStatus
        {
            get => _sipCameraStatus;
            set => _sipCameraStatus = value;
        }

        public PushStreamSocketType? PushStreamSocketType
        {
            get => _pushStreamSocketType;
            set => _pushStreamSocketType = value;
        }


        public Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        public string? InputUrl
        {
            get => _inputUrl;
            set => _inputUrl = value;
        }

        public override string ToString()
        {
            return
                $"{nameof(Camera)}: {Camera}, {nameof(Vhost)}: {Vhost}, {nameof(App)}: {App}, {nameof(StreamId)}: {StreamId}, {nameof(StreamServerIp)}: {StreamServerIp}, {nameof(StreamServerPort)}: {StreamServerPort}, {nameof(SipCameraStatus)}: {SipCameraStatus}";
        }
    }

    [Serializable]
    public class SipDevice
    {
        private string _crc32;
        private string _device_id;
        private string _ipAddress;
        private int _sipPort;
        private SIPRequest _lastSipRequest;
        private List<CameraEx> _CameraExList = new List<CameraEx>();
        private DateTime _lastKeepAliveTime;
        private SipDeviceStatus _sipDeviceStatus;
        private Dictionary<string, DateTime> _alarmList = new Dictionary<string, DateTime>();
        private DateTime _lastUpdateTime;


        public string CRC32
        {
            get => _crc32;
            set => _crc32 = value;
        }

        public string DeviceId
        {
            get => _device_id;
            set => _device_id = value;
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public int SipPort
        {
            get => _sipPort;
            set => _sipPort = value;
        }

        public SIPRequest LastSipRequest
        {
            get => _lastSipRequest;
            set => _lastSipRequest = value;
        }

        public List<CameraEx> CameraExList
        {
            get => _CameraExList;
            set => _CameraExList = value;
        }

        public DateTime LastKeepAliveTime
        {
            get => _lastKeepAliveTime;
            set => _lastKeepAliveTime = value;
        }

        public SipDeviceStatus SipDeviceStatus
        {
            get => _sipDeviceStatus;
            set => _sipDeviceStatus = value;
        }

        public Dictionary<string, DateTime> AlarmList
        {
            get => _alarmList;
            set => _alarmList = value;
        }

        public DateTime LastUpdateTime
        {
            get => _lastUpdateTime;
            set => _lastUpdateTime = value;
        }


        public override string ToString()
        {
            return
                $"{nameof(CRC32)}: {CRC32}, {nameof(DeviceId)}: {DeviceId}, {nameof(IpAddress)}: {IpAddress}, {nameof(SipPort)}: {SipPort}, {nameof(LastSipRequest)}: {LastSipRequest}, {nameof(CameraExList)}: {CameraExList}, {nameof(LastKeepAliveTime)}: {LastKeepAliveTime}, {nameof(SipDeviceStatus)}: {SipDeviceStatus}, {nameof(AlarmList)}: {AlarmList}";
        }
    }
}