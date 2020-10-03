using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StreamMediaServerKeeper
{
    [Serializable]
    public class LoadAverage
    {
        private float? _loadAverageMin1;
        private float? _loadAverageMin5;
        private float? _loadAverageMin15;

        public float? LoadAverageMin1
        {
            get => _loadAverageMin1;
            set => _loadAverageMin1 = value;
        }

        public float? LoadAverageMin5
        {
            get => _loadAverageMin5;
            set => _loadAverageMin5 = value;
        }

        public float? LoadAverageMin15
        {
            get => _loadAverageMin15;
            set => _loadAverageMin15 = value;
        }
    }

    [Serializable]
    public enum OStype
    {
        WindowsX32,
        WindowsX64,
        LinuxX32,
        LinuxX64,
        MacOSX32,
        MacOSX64,
        UnixX32,
        UnixX64,
        Unknow,
    }

    [Serializable]
    public class CPUInfo
    {
        private float _cpuUsed = -1f;
        private float _cpuIdle = 100f;
        private int _cpuCores = 0;

        public float CpuUsed
        {
            get => _cpuUsed;
            set => _cpuUsed = value;
        }

        public float CpuIdle
        {
            get => _cpuIdle;
            set => _cpuIdle = value;
        }

        public int CpuCores
        {
            get => _cpuCores;
            set => _cpuCores = value;
        }
    }

    [Serializable]
    public class ResGetSystemInfo
    {
        public DateTime? UpdateTime
        {
            get { return DateTime.Now; }
        }

        public OStype? OStype
        {
            get
            {
                var islinux = SystemHelper.IsLinux();
                if (islinux)
                {
                    if (SystemHelper.IsLinux32())
                    {
                        return StreamMediaServerKeeper.OStype.LinuxX32;
                    }
                    else
                    {
                        return StreamMediaServerKeeper.OStype.LinuxX64;
                    }
                }

                var iswin = SystemHelper.IsWindows();
                if (iswin)
                {
                    if (SystemHelper.IsWin32())
                    {
                        return StreamMediaServerKeeper.OStype.WindowsX32;
                    }
                    else
                    {
                        return StreamMediaServerKeeper.OStype.WindowsX64;
                    }
                }

                var ismac = SystemHelper.IsOSX();
                if (ismac)
                {
                    if (SystemHelper.IsOSX32())
                    {
                        return StreamMediaServerKeeper.OStype.MacOSX32;
                    }
                    else
                    {
                        return StreamMediaServerKeeper.OStype.MacOSX64;
                    }
                }

                var isunix = SystemHelper.IsUnix();
                {
                    if (SystemHelper.IsUnix32())
                    {
                        return StreamMediaServerKeeper.OStype.UnixX32;
                    }
                    else
                    {
                        return StreamMediaServerKeeper.OStype.UnixX64;
                    }
                }
                return StreamMediaServerKeeper.OStype.Unknow;
            }
        }

        public string? AssemblyPath
        {
            get { return SystemHelper.GetAssemblyPath(); }
        }

        public string? AssemblyFolderPath
        {
            get { return SystemHelper.GetAssemblyFolderPath(); }
        }

        public long? TotalProcessRunningTime
        {
            get
            {
                Process process = Process.GetCurrentProcess();
                return SystemHelper.GetTotalRunningTime(process);
            }
        }

        public string? TotalProcessRunningTimeText
        {
            get
            {
                Process process = Process.GetCurrentProcess();
                return SystemHelper.GetTotalRunningTimeEx(process);
            }
        }

        public long? TatolSystemRunningTime
        {
            get { return SystemHelper.GetSystemRunningTime(); }
        }

        public string? TatolSystemRunningTimeText
        {
            get { return SystemHelper.GetSystemRunningTimeText(SystemHelper.GetSystemRunningTime()); }
        }


        public CPUInfo? LinuxCpuInfo
        {
            get
            {
                if (OStype == StreamMediaServerKeeper.OStype.LinuxX32 ||
                    OStype == StreamMediaServerKeeper.OStype.LinuxX64)
                {
                    var c = SystemHelper.GetCPUused();
                    return new CPUInfo()
                    {
                        CpuCores = SystemHelper.GetCpuNumber(),
                        CpuIdle = c != null ? c.CpuIdle : 0f,
                        CpuUsed = c != null ? c.CpuUsed : 0f,
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public LoadAverage? LinuxLoadAverage
        {
            get
            {
                if (OStype == StreamMediaServerKeeper.OStype.LinuxX32 ||
                    OStype == StreamMediaServerKeeper.OStype.LinuxX64)
                {
                    var l = SystemHelper.GetLoadAverage();
                    return l != null ? l : null;
                }

                return null;
            }
        }

        public MemoryMetrics? LinuxMemoryUsage
        {
            get
            {
                if (OStype == StreamMediaServerKeeper.OStype.LinuxX32 ||
                    OStype == StreamMediaServerKeeper.OStype.LinuxX64)
                {
                    return SystemHelper.GetLinuxMemoryMetrics();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<DriveMetrics>? HddListInfo
        {
            get { return SystemHelper.GetDriveMetrics(); }
        }
    }
}