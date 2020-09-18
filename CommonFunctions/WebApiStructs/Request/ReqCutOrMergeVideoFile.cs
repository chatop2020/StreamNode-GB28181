using System;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqCutOrMergeVideoFile
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private string? _mediaServerId;
        private string? _cameraId;
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

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }


        public string? CallbackUrl
        {
            get => _callbackUrl;
            set => _callbackUrl = value;
        }
    }
}