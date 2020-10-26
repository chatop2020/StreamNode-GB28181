using System;
using FreeSql.DataAnnotations;

namespace CommonFunctions.DBStructs
{
    [Table(Name = "RecordFile")]
    [Index("rf_cameraid", "CameraId", false)]
    [Index("rf_cameraname", "CameraName", false)]
    [Index("rf_ipaddress", "IpAddress", false)]
    [Index("rf_findgroup", "PushMediaServerId,CameraId", false)]
    [Index("rf_finddatetime", "PushMediaServerId,CameraId,StartTime,EndTime", false)]
    [Index("rf_finddate", "PushMediaServerId,CameraId,RecordDate", false)]
    [Index("rf_finddel", "PushMediaServerId,CameraId,Deleted", false)]
    [Serializable]
    public class RecordFile
    {
        private long _id;
        private string _cameraId;
        private string _cameraName;
        private string? _deptId;
        private string? _deptName;
        private string? _pDetpId;
        private string? _pDetpName;
        private string _cameraIpAddress;
        private long? _duration;
        private bool? _deleted;
        private string? _videoPath;
        private long? _fileSize;
        private string? _vhost;
        private string? _streamid;
        private string? _app;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private DateTime? _updateTime;
        private string? _recordDate;
        private string? _downloadUrl;
        private bool? _undo;
        private string _pushMediaServerId;

        /// <summary>
        /// 数据库主键
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// 摄像头实例ID
        /// </summary>

        public string CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        /// <summary>
        /// 摄像头名称
        /// </summary>
        public string CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        /// <summary>
        /// 部门代码
        /// </summary>
        public string? DeptId
        {
            get => _deptId;
            set => _deptId = value;
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string? DeptName
        {
            get => _deptName;
            set => _deptName = value;
        }

        /// <summary>
        /// 父部门代码
        /// </summary>
        public string? PDetpId
        {
            get => _pDetpId;
            set => _pDetpId = value;
        }

        /// <summary>
        /// 父部门名称
        /// </summary>
        public string? PDetpName
        {
            get => _pDetpName;
            set => _pDetpName = value;
        }

        /// <summary>
        /// 摄像头ip地址
        /// </summary>
        public string CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

        /// <summary>
        /// 录制文件的时长（秒）
        /// </summary>

        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        /// <summary>
        /// 删除标记（true为已删除）
        /// </summary>
        public bool? Deleted
        {
            get => _deleted;
            set => _deleted = value;
        }

        /// <summary>
        /// 录制文件的物理路径
        /// </summary>
        public string? VideoPath
        {
            get => _videoPath;
            set => _videoPath = value;
        }

        /// <summary>
        /// 录制文件大小(Byte)
        /// </summary>
        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        /// <summary>
        /// 流媒体Vhost标记
        /// </summary>

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// 流媒体StreamId标记
        /// </summary>
        public string? Streamid
        {
            get => _streamid;
            set => _streamid = value;
        }

        /// <summary>
        /// 流媒体App标记
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// 录制文件的开始时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 录制文件的结束时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 记录更新时间
        /// </summary>
        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// 记录录制日期
        /// </summary>
        public string? RecordDate
        {
            get => _recordDate;
            set => _recordDate = value;
        }

        /// <summary>
        /// 录制文件的播放与下载地址
        /// </summary>
        public string? DownloadUrl
        {
            get => _downloadUrl;
            set => _downloadUrl = value;
        }

        /// <summary>
        /// 是否可以恢复删除（true为可恢复）
        /// </summary>
        public bool? Undo
        {
            get => _undo;
            set => _undo = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }
    }
}