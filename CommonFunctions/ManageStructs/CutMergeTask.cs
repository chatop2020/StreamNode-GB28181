using System;
using System.Collections.Generic;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 裁剪合并任务的状态
    /// Create=任务创建
    /// Packaging=解包成ts流
    /// Cutting=对前后进行裁剪
    /// Mergeing=文件合并
    /// Closed=任务结束
    /// </summary>
    [Serializable]
    public enum TaskStatus
    {
        Create,
        Packaging,
        Cutting,
        Mergeing,
        Closed,
    }

    [Serializable]
    public class CutMergeTask
    {
        private List<CutMergeStruct>? _cutMergeFileList;
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
        /// 裁剪合并结构列表（任务中所包含的所有文件列表）
        /// </summary>
        public List<CutMergeStruct>? CutMergeFileList
        {
            get => _cutMergeFileList;
            set => _cutMergeFileList = value;
        }

        /// <summary>
        /// 裁剪合并任务ID
        /// </summary>
        public string? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }

        /// <summary>
        /// 裁剪合并任务结束后的回调地址（http webapi地址）
        /// </summary>
        public string? CallbakUrl
        {
            get => _callbakUrl;
            set => _callbakUrl = value;
        }

        /// <summary>
        /// 裁剪合并任务的创建时间
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
        /// 下载与播放地址
        /// </summary>
        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }
    }
}