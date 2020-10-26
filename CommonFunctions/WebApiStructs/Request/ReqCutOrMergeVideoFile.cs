using System;

namespace CommonFunctions.WebApiStructs.Request
{
    /// <summary>
    /// 请求结构-裁剪合并文件结构
    /// </summary>
    [Serializable]
    public class ReqCutOrMergeVideoFile
    {
        private DateTime _startTime;
        private DateTime _endTime;
        private string? _mediaServerId;
        private string? _cameraId;
        private string? _callbackUrl;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get => _endTime;
            set => _endTime = value;
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
        /// 摄像头实例ID
        /// </summary>
        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }


        /// <summary>
        /// 任务结束后的回调地址（http webapi）
        /// </summary>
        public string? CallbackUrl
        {
            get => _callbackUrl;
            set => _callbackUrl = value;
        }
    }
}