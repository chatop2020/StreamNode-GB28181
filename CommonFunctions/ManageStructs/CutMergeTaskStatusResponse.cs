using System;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class CutMergeTaskStatusResponse
    {
        private string? _taskId;
        private string? _callbakUrl;
        private DateTime _createTime;
        private TaskStatus? _taskStatus;
        private string? _playUrl;

        /// <summary>
        /// Create=0%
        /// Packageing=45%
        /// Cutting=15%
        /// Mergeing=40%
        /// </summary>
        private double? _processPercentage = 0f;

        /// <summary>
        /// 裁剪合并任务id
        /// </summary>
        public string? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }

        /// <summary>
        /// 任务结束后回调http地址（http webapi）
        /// </summary>
        public string? CallbakUrl
        {
            get => _callbakUrl;
            set => _callbakUrl = value;
        }

        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 裁剪合并任务状态
        /// Create=任务创建
        /// Packaging=解包成ts流
        /// Cutting=对前后进行裁剪
        /// Mergeing=文件合并
        /// Closed=任务结束
        /// </summary>
        public TaskStatus? TaskStatus
        {
            get => _taskStatus;
            set => _taskStatus = value;
        }

        /// <summary>
        /// 任务处理百分比
        /// </summary>
        public double? ProcessPercentage
        {
            get => _processPercentage;
            set => _processPercentage = value;
        }

        /// <summary>
        /// 播放及下载地址
        /// </summary>
        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }
    }
}