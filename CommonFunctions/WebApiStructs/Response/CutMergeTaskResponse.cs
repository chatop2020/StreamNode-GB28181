using System;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;

namespace CommonFunctions.WebApiStructs.Response
{
    public enum CutMergeRequestStatus
    {
        Succeed,
        Failed,
        WaitForCallBack,
    }

    [Serializable]
    public class CutMergeTaskResponse
    {
        private CutMergeTask _task = null!;
        private string? _filePath;
        private string? _uri;
        private long? _fileSize;
        private long? _duration;
        private CutMergeRequestStatus? _status;
        private double? _timeConsuming;
        private ReqCutOrMergeVideoFile? _request;

        public CutMergeTask Task
        {
            get => _task;
            set => _task = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public string? Uri
        {
            get => _uri;
            set => _uri = value;
        }

        public long? FileSize
        {
            get => _fileSize;
            set => _fileSize = value;
        }

        public long? Duration
        {
            get => _duration;
            set => _duration = value;
        }

        public CutMergeRequestStatus? Status
        {
            get => _status;
            set => _status = value;
        }

        public double? TimeConsuming
        {
            get => _timeConsuming;
            set => _timeConsuming = value;
        }

        public ReqCutOrMergeVideoFile? Request
        {
            get => _request;
            set => _request = value;
        }
    }
}