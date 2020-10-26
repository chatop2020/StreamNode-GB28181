using System;
using System.Collections.Generic;
using CommonFunctions.WebApiStructs.Response;

namespace CommonFunctions.WebApiStructs.Request
{
    /// <summary>
    /// 流媒体服务器的系统信息
    /// </summary>
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

        /// <summary>
        /// 操作系统类型
        /// WindowsX32,
        /// WindowsX64,
        /// LinuxX32,
        /// LinuxX64,
        /// MacOSX32,
        /// MacOSX64,
        /// UnixX32,
        /// UnixX64,
        /// Unknow,
        /// </summary>
        public OStype? OsType
        {
            get => _osType;
            set => _osType = value;
        }

        
        /// <summary>
        /// 获取数据时间
        /// </summary>
        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// 可执行文件路径
        /// </summary>
        public string? AssemblyPath
        {
            get => _assemblyPath;
            set => _assemblyPath = value;
        }

        /// <summary>
        /// 可执行文件目录
        /// </summary>
        public string? AssemblyFolderPath
        {
            get => _assemblyFolderPath;
            set => _assemblyFolderPath = value;
        }

        /// <summary>
        /// 可执行文件进程运行时长（秒）
        /// </summary>
        public long? TotalProcessRunningTime
        {
            get => _totalProcessRunningTime;
            set => _totalProcessRunningTime = value;
        }

        /// <summary>
        /// 可执行文件进程运行时长，文本方式显示
        /// </summary>
        public string TotalProcessRunningTimeText
        {
            get => _totalProcessRunningTimeText;
            set => _totalProcessRunningTimeText = value;
        }
        
        /// <summary>
        /// 系统运行时长（秒）
        /// </summary>

        public long? TatolSystemRunningTime
        {
            get => _tatolSystemRunningTime;
            set => _tatolSystemRunningTime = value;
        }

        /// <summary>
        /// 系统运行时长，文本方式显示
        /// </summary>
        public string? TatolSystemRunningTimeText
        {
            get => _tatolSystemRunningTimeText;
            set => _tatolSystemRunningTimeText = value;
        }

        /// <summary>
        /// CPU信息
        /// CpuUsed=cpu使用量
        /// CpuIdle=cpu空闲量
        /// CpuCores=cpu核数
        /// </summary>
        public CPUInfo? LinuxCpuInfo
        {
            get => _linuxCpuInfo;
            set => _linuxCpuInfo = value;
        }

        /// <summary>
        /// linux操作系统负载
        /// LoadAverageMin1=1分钟负载
        /// LoadAverageMin5=5分钟负载
        /// LoadAverageMin15=15分钟负载
        /// </summary>
        public LoadAverage? LinuxLoadAverage
        {
            get => _linuxLoadAverage;
            set => _linuxLoadAverage = value;
        }

        /// <summary>
        /// 操作系统内存信息
        /// Total=内存总量
        /// Used=使用内存总量
        /// Free=空闲内容总量
        /// FreePercent=内存空闲率
        /// </summary>
        public MemoryMetrics? LinuxMemoryUsage
        {
            get => _linuxMemoryUsage;
            set => _linuxMemoryUsage = value;
        }

        /// <summary>
        /// 操作硬盘信息列表
        /// Name=Win:硬盘盘符;类unix 挂载目录地址
        /// IsReady=是否可用
        /// Total=存储总量
        /// Used=使用存储总量
        /// Free=空闲存储总量
        /// FreePercent=存储空闲率
        /// </summary>
        public List<DriveMetrics>? HddListInfo
        {
            get => _HddListInfo;
            set => _HddListInfo = value;
        }
    }
}