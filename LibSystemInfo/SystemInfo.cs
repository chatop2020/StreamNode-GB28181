using System;
using System.Threading;
using SystemInfoLibrary.Hardware;
using SystemInfoLibrary.OperatingSystem;

namespace LibSystemInfo
{
    public class SystemInfo
    {
        private static OperatingSystemInfo _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
        private static HardwareInfo _hardwareInfo = _operatingSystemInfo.Hardware;
        private static OperatingSystemType _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
        private static GlobalSystemInfo _globalSystemInfo = new GlobalSystemInfo();
        private static object lockObj = new object();

        public SystemInfo()
        {
            _globalSystemInfo.Architecture = _operatingSystemInfo.Architecture;
            _globalSystemInfo.OSName = _operatingSystemInfo.Name;
            _globalSystemInfo.CpuCores = Environment.ProcessorCount;
            _globalSystemInfo.FrameworkVersion = _operatingSystemInfo.FrameworkVersion.ToString();
            _globalSystemInfo.SystemType = _operatingSystemType.ToString();


            new Thread(new ThreadStart(delegate
            {
                try
                {
                    GetInfo();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }


        private MemoryInfo getMeminfo()
        {
            MemoryInfo tmp = new MemoryInfo();

            tmp.Total = _hardwareInfo.RAM.Total * 1024;
            tmp.Used = _hardwareInfo.RAM.Total * 1024 - _hardwareInfo.RAM.Free * 1024;
            tmp.Free = _hardwareInfo.RAM.Free * 1024;
            tmp.FreePercent =
                Math.Round(100f - (double.Parse(tmp.Used.ToString()) * 100.00 / double.Parse(tmp.Total.ToString())), 3);
            tmp.UpdateTime = DateTime.Now;
            return tmp;
        }

        private void GetInfo()
        {
            ushort i = 0;
            ushort j = 0;
            while (true)
            {
                i++;
                j++;
                if (ushort.MaxValue - i < 100)
                {
                    i = 0;
                }

                if (ushort.MaxValue - j < 100)
                {
                    j = 0;
                }

                lock (lockObj)
                {
                    if ((j % 10 == 0 || j == 1) && _operatingSystemType != OperatingSystemType.Windows) //10秒更新一次内存情况
                    {
                        _operatingSystemInfo = null;
                        _hardwareInfo = null;
                        _operatingSystemInfo = OperatingSystemInfo.GetOperatingSystemInfo();
                        _hardwareInfo = _operatingSystemInfo.Hardware;
                        _operatingSystemType = _operatingSystemInfo.OperatingSystemType;
                        _globalSystemInfo.MemoryInfo = getMeminfo();
                    }

                    if (i % 120 == 0 || i == 1) //2分钟更新一次硬盘情况
                    {
                        _globalSystemInfo.DriveInfo = DiskInfoValue.GetDrivesInfo();
                    }

                    switch (_operatingSystemType)
                    {
                        case OperatingSystemType.Windows:
                            _globalSystemInfo.CpuLoad = CPUWinLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkWinValue.GetNetworkStat();
                            if ((j % 10 == 0 || j == 1) &&
                                _operatingSystemType == OperatingSystemType.Windows) //10秒更新一次内存情况
                            {
                                _globalSystemInfo.MemoryInfo = MemoryWinValue.GetMemoryStatus();
                            }

                            break;
                        case OperatingSystemType.MacOSX:
                            _globalSystemInfo.CpuLoad = CPUMacOSLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkMacValue.GetNetworkStat();
                            break;
                        case OperatingSystemType.Unix:
                            _globalSystemInfo.CpuLoad = CPULinuxLoadValue.CPULOAD;
                            _globalSystemInfo.NetWorkStat = NetWorkLinuxValue.GetNetworkStat();
                            break;
                    }

                    _globalSystemInfo.UpdateTime = DateTime.Now;
                }

                Thread.Sleep(1000);
            }
        }


        public string GetSystemInfoJson()
        {
            lock (lockObj)
            {
                return JsonHelper.ToJson(_globalSystemInfo);
            }
        }

        public GlobalSystemInfo GetSystemInfoObject()
        {
            lock (lockObj)
            {
                return _globalSystemInfo;
            }
        }
    }
}