using System;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 裁剪合并任务结构体
    /// </summary>
    [Serializable]
    public class CutMergeStruct
    {
        private long? _dbId;
        private string? _filePath;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private long? _fileSize;
        private long? _duration;
        private string? _cutStartPos;
        private string? _cutEndPos;

        
        /// <summary>
        /// 数据库中的主键ID
        /// </summary>
        public long? DbId
        {
            get => _dbId;
            set => _dbId = value;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        /// <summary>
        /// 录制文件开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 录制文件结束时间
        /// </summary>

        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 文件大小(Byte)
        /// </summary>
        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        /// <summary>
        /// 音视频文件时长(秒)
        /// </summary>
        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// 裁剪开始位置标记
        /// </summary>
        public string? CutStartPos
        {
            get => _cutStartPos;
            set => _cutStartPos = value;
        }

        /// <summary>
        /// 裁剪结束位置标记
        /// </summary>
        public string? CutEndPos
        {
            get => _cutEndPos;
            set => _cutEndPos = value;
        }
    }
}