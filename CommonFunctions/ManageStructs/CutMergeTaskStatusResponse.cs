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

        public string? TaskId
        {
            get => _taskId;
            set => _taskId = value;
        }

        public string? CallbakUrl
        {
            get => _callbakUrl;
            set => _callbakUrl = value;
        }

        public DateTime CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        public TaskStatus? TaskStatus
        {
            get => _taskStatus;
            set => _taskStatus = value;
        }

        public double? ProcessPercentage
        {
            get => _processPercentage;
            set => _processPercentage = value;
        }

        public string? PlayUrl
        {
            get => _playUrl;
            set => _playUrl = value;
        }
    }
}