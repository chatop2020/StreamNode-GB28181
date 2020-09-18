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

        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }

        public string CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        public string CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        public string? DeptId
        {
            get => _deptId;
            set => _deptId = value;
        }

        public string? DeptName
        {
            get => _deptName;
            set => _deptName = value;
        }

        public string? PDetpId
        {
            get => _pDetpId;
            set => _pDetpId = value;
        }

        public string? PDetpName
        {
            get => _pDetpName;
            set => _pDetpName = value;
        }

        public string CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

      

        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public bool? Deleted
        {
            get => _deleted;
            set => _deleted = value;
        }

        public string? VideoPath
        {
            get => _videoPath;
            set => _videoPath = value;
        }

        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? Streamid
        {
            get => _streamid;
            set => _streamid = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        public string? RecordDate
        {
            get => _recordDate;
            set => _recordDate = value;
        }

        public string? DownloadUrl
        {
            get => _downloadUrl;
            set => _downloadUrl = value;
        }

        public bool? Undo
        {
            get => _undo;
            set => _undo = value;
        }

        public string PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }
    }
}