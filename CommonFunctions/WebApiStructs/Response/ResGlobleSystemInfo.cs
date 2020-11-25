using System;
using System.Collections.Generic;
using CommonFunctions.WebApiStructs.Request;
using LibSystemInfo;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class MediaServerInfomation
    {
        private string? _mediaSeverId;
        private GlobalSystemInfo? _mediaServerSystemInfo;

        public string? MediaSeverId
        {
            get => _mediaSeverId;
            set => _mediaSeverId = value;
        }

        public GlobalSystemInfo? MediaServerSystemInfo
        {
            get => _mediaServerSystemInfo;
            set => _mediaServerSystemInfo = value;
        }
    }

    [Serializable]
    public class ResGlobleSystemInfo
    {
        private GlobalSystemInfo _StreamCtrlSystemInfo;
        private List<MediaServerInfomation> _mediaServerSystemInfos;
        private DateTime _updateTime;

        public GlobalSystemInfo StreamCtrlSystemInfo
        {
            get => _StreamCtrlSystemInfo;
            set => _StreamCtrlSystemInfo = value;
        }

        public List<MediaServerInfomation> MediaServerSystemInfos
        {
            get => _mediaServerSystemInfos;
            set => _mediaServerSystemInfos = value;
        }

        public DateTime UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }
    }
}