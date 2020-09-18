using System;

namespace StreamMediaServerKeeper
{
    [Serializable]
    public class ReqCutOrMergeVideoFile
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private string? _mediaServerId;
        private string? _app;
        private string? _vhost;
        private string? _streamId;
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

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        public string? CallbackUrl
        {
            get => _callbackUrl;
            set => _callbackUrl = value;
        }
    }
}