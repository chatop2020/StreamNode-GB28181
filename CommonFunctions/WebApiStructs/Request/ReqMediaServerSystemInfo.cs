using System;
using System.Collections.Generic;
using CommonFunctions.WebApiStructs.Response;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqMediaServerSystemInfo
    {
        private OStype? _osType;
        private DateTime? _updateTime;
        private string? _assemblyPath;
        private string? _assemblyFolderPath;
        private long? _totalProcessRunningTime;
        private string _totalProcessRunningTimeText;
        private long? _tatolSystemRunningTime;
        private string? _tatolSystemRunningTimeText;
        private CPUInfo? _linuxCpuInfo;
        private LoadAverage? _linuxLoadAverage;
        private MemoryMetrics? _linuxMemoryUsage;
        private List<DriveMetrics>? _HddListInfo;

        public OStype? OsType
        {
            get => _osType;
            set => _osType = value;
        }

        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        public string? AssemblyPath
        {
            get => _assemblyPath;
            set => _assemblyPath = value;
        }

        public string? AssemblyFolderPath
        {
            get => _assemblyFolderPath;
            set => _assemblyFolderPath = value;
        }

        public long? TotalProcessRunningTime
        {
            get => _totalProcessRunningTime;
            set => _totalProcessRunningTime = value;
        }

        public string TotalProcessRunningTimeText
        {
            get => _totalProcessRunningTimeText;
            set => _totalProcessRunningTimeText = value;
        }

        public long? TatolSystemRunningTime
        {
            get => _tatolSystemRunningTime;
            set => _tatolSystemRunningTime = value;
        }

        public string? TatolSystemRunningTimeText
        {
            get => _tatolSystemRunningTimeText;
            set => _tatolSystemRunningTimeText = value;
        }

        public CPUInfo? LinuxCpuInfo
        {
            get => _linuxCpuInfo;
            set => _linuxCpuInfo = value;
        }

        public LoadAverage? LinuxLoadAverage
        {
            get => _linuxLoadAverage;
            set => _linuxLoadAverage = value;
        }

        public MemoryMetrics? LinuxMemoryUsage
        {
            get => _linuxMemoryUsage;
            set => _linuxMemoryUsage = value;
        }

        public List<DriveMetrics>? HddListInfo
        {
            get => _HddListInfo;
            set => _HddListInfo = value;
        }
    }
}