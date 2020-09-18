using System;

namespace SRSManageCommon.ControllerStructs.RequestModules
{
    [Serializable]
    public class ReqCutOrMergeVideoFile
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private string? _deviceId;
        private string? _app;
        private string? _vhostDomain;
        private string? _stream;
        private string? _callbackUrl;

        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        public string? DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? VhostDomain
        {
            get => _vhostDomain;
            set => _vhostDomain = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        public string? CallbackUrl
        {
            get => _callbackUrl;
            set => _callbackUrl = value;
        }
    }
}