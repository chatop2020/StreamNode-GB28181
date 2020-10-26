using System;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 请求结构-获取录制计划
    /// </summary>
    [Serializable]
    public class ReqGetDvrPlan
    {
        private string? _mediaServerId;
        private string? _cameraId;

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 摄像头实例ID
        /// </summary>
        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }
    }
}