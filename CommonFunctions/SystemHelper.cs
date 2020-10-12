using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommonFunctions.WebApiStructs.Response;
using Newtonsoft.Json;

namespace CommonFunctions
{
    /// <summary>
    /// 系统帮助类
    /// </summary>
    public static class SystemHelper
    {
        #region 平台判断

        /// <summary>
        /// 是否windows平台
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        /// <summary>
        /// 是否windows32平台
        /// </summary>
        /// <returns></returns>
        public static bool IsWin32()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X86;
        }

        /// <summary>
        /// 是否windows64平台
        /// </summary>
        /// <returns></returns>
        public static bool IsWin64()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X64;
        }

        /// <summary>
        /// 是否Linux平台
        /// </summary>
        /// <returns></returns>
        public static bool IsLinux()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        /// <summary>
        /// 是否Linux32平台
        /// </summary>
        /// <returns></returns>
        public static bool IsLinux32()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X86;
        }

        /// <summary>
        /// 是否Linux64平台
        /// </summary>
        /// <returns></returns>
        public static bool IsLinux64()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X64;
        }

        /// <summary>
        /// 是否OSX平台
        /// </summary>
        /// <returns></returns>
        public static bool IsOSX()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        /// <summary>
        /// 是否OSX32平台
        /// </summary>
        /// <returns></returns>
        public static bool IsOSX32()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X86;
        }

        /// <summary>
        /// 是否OSX64平台
        /// </summary>
        /// <returns></returns>
        public static bool IsOSX64()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                   && RuntimeInformation.ProcessArchitecture == Architecture.X64;
        }

        /// <summary>
        /// 是否Unix平台
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix()
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
        }

        /// <summary>
        /// 是否Unix32平台
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix32()
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                   && RuntimeInformation.ProcessArchitecture == Architecture.X86;
        }

        /// <summary>
        /// 是否Unix64平台
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix64()
        {
            return (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                   && RuntimeInformation.ProcessArchitecture == Architecture.X64;
        }

        #endregion

        #region 路径获取

        /// <summary>
        /// 获取当前程序集文件dll路径（包含文件名）(绝对路径)
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyPath()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        /// <summary>
        /// 获取当前程序集文件夹路径(绝对路径)
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyFolderPath()
        {
            return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        }

        /// <summary>
        /// 获取程序集解析程序用于探测程序集的基目录的路径名。
        /// </summary>
        /// <returns></returns>
        public static string GetBaseDirectory()
        {
            return AppContext.BaseDirectory;
        }

        #endregion

        #region cpu相关

        /// <summary>
        /// 获取当前计算机上的处理器数。
        /// </summary>
        /// <returns></returns>
        public static int GetCpuNumber()
        {
            //如果机器超线程，获取到的是超线程的数量。例如cpu是4核8线程，获取到的就是8。
            return Environment.ProcessorCount;
        }



        
        /// <summary>
        /// 获取系统负载率
        /// </summary>
        /// <returns></returns>
        public static LoadAverage GetLoadAverage()
        {
            try
            {
                string std = "";
                string err = "";
                string cmd = "top - bn1 | awk '/average:/ '";

                var ret = LinuxShell.Run(cmd, 1000, out std, out err);
                if (ret)
                {
                    if (!string.IsNullOrEmpty(std))
                    {
                        int pos_start = std.IndexOf("load average:", StringComparison.Ordinal);
                        if (pos_start > 0)
                        {
                            std = std.Substring(pos_start + "load average:".Length).Trim();
                        }

                        string[] loadaverageinfo = std.Split(',');
                        if (loadaverageinfo.Length == 3)
                        {
                            var loadAverage = new LoadAverage();
                            loadAverage.LoadAverageMin1 = float.Parse((loadaverageinfo[0]).Trim());
                            loadAverage.LoadAverageMin5 = float.Parse((loadaverageinfo[1]).Trim());
                            loadAverage.LoadAverageMin15 = float.Parse((loadaverageinfo[2]).Trim());
                            return loadAverage;
                        }
                    }

                    if (!string.IsNullOrEmpty(err))
                    {
                        int pos_start = err.IndexOf("load average:", StringComparison.Ordinal);
                        if (pos_start > 0)
                        {
                            err = err.Substring(pos_start + "load average:".Length).Trim();
                        }

                        string[] loadaverageinfo = err.Split(',');
                        if (loadaverageinfo.Length == 3)
                        {
                            var loadAverage = new LoadAverage();
                            loadAverage.LoadAverageMin1 = float.Parse((loadaverageinfo[0]).Trim());
                            loadAverage.LoadAverageMin5 = float.Parse((loadaverageinfo[1]).Trim());
                            loadAverage.LoadAverageMin15 = float.Parse((loadaverageinfo[2]).Trim());
                            return loadAverage;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        
        
        
        
        

        /// <summary>
        /// 获取cpu使用率和空闲率
        /// </summary>
        /// <returns></returns>
        public static CPUInfo GetCPUused()
        {
            try
            {
                string std = "";
                string err = "";
                string cmd = "top -bn1 | awk '/%Cpu/ {print $2+$4,$8}'";
                var ret = LinuxShell.Run(cmd, 1000, out std, out err);
                if (ret)
                {
                    if (!string.IsNullOrEmpty(std))
                    {
                        string[] cpuinfo = std.Split(' ');
                        if (cpuinfo.Length == 2)
                        {
                            CPUInfo result = new CPUInfo();
                            result.CpuIdle = float.Parse(cpuinfo[1]);
                            result.CpuUsed = float.Parse(cpuinfo[0]);
                            return result;
                        }
                    }

                    if (!string.IsNullOrEmpty(err))
                    {
                        string[] cpuinfo = err.Split(' ');
                        if (cpuinfo.Length == 2)
                        {
                            CPUInfo result = new CPUInfo();
                            result.CpuIdle = float.Parse(cpuinfo[1]);
                            result.CpuUsed = float.Parse(cpuinfo[0]);
                            return result;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }


        public static string GetSystemRunningTimeText(long d)
        {
            if (d == 0) return "未知";
            long tickcount = d;
            long days = tickcount / (24 * 60 * 60 * 1000); //24*60*60*1000
            tickcount = tickcount % (24 * 60 * 60 * 1000);
            long hours = tickcount / (60 * 60 * 1000); //60*60*1000
            tickcount = tickcount % (60 * 60 * 1000);
            long minutes = tickcount / (60 * 1000); //60*1000
            tickcount = tickcount % (60 * 1000);
            long seconds = tickcount / 1000; //1000
            tickcount = tickcount % 1000;
            long milliseconds = tickcount;
            return $"{days}天{hours}时{minutes}分{seconds}秒{milliseconds}毫秒";
        }


        public static long GetSystemRunningTime()
        {
            if (File.Exists("/proc/uptime"))
            {
                string std = File.ReadAllText("/proc/uptime");
                string[] s_arr = std.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (s_arr.Length == 2)
                {
                    double r;
                    var ret = double.TryParse(s_arr[0], out r);
                    if (ret)
                    {
                        return (long) (r * 1000);
                    }
                }
            }

            return 0;
        }


        /// <summary>
        /// 获取指定进程运行总时间（毫秒）
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static long GetTotalRunningTime(Process process)
        {
            return (long) (DateTime.Now - process.StartTime).TotalMilliseconds;
        }

        /// <summary>
        /// 获取指定进程运行总时间（{days}天{hours}时{minutes}分{seconds}秒{milliseconds}毫秒）
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static string GetTotalRunningTimeEx(Process process)
        {
            long tickcount = (long) (DateTime.Now - process.StartTime).TotalMilliseconds;

            long days = tickcount / (24 * 60 * 60 * 1000); //24*60*60*1000
            tickcount = tickcount % (24 * 60 * 60 * 1000);

            long hours = tickcount / (60 * 60 * 1000); //60*60*1000
            tickcount = tickcount % (60 * 60 * 1000);

            long minutes = tickcount / (60 * 1000); //60*1000
            tickcount = tickcount % (60 * 1000);

            long seconds = tickcount / 1000; //1000
            tickcount = tickcount % 1000;

            long milliseconds = tickcount;

            return $"{days}天{hours}时{minutes}分{seconds}秒{milliseconds}毫秒";
        }

        /// <summary>
        /// 获取指定进程cpu占用百分比(以单核=100%为基准)
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static async Task<double> GetWindowsCpuUsageAsync(Process process)
        {
            var startTime = DateTime.Now;
            var startCpuUsage = process.TotalProcessorTime;
            await Task.Delay(500);
            var endTime = DateTime.Now;
            var endCpuUsage = process.TotalProcessorTime;
            var totalTime = (endTime - startTime).TotalMilliseconds;
            var cpuUsedTime = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var cpuUsed = Math.Round(cpuUsedTime * 100 / totalTime, 2);
            return cpuUsed;
        }

        /// <summary>
        /// 获取指定进程cpu占用百分比(以所有核加起来=100%为基准)
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static async Task<double> GetWindowsCpuUsageExAsync(Process process)
        {
            var startTime = DateTime.Now;
            var startCpuUsage = process.TotalProcessorTime;
            await Task.Delay(500);
            var endTime = DateTime.Now;
            var endCpuUsage = process.TotalProcessorTime;
            var totalTime = (endTime - startTime).TotalMilliseconds;
            var cpuUsedTime = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            var cpuUsed = Math.Round(cpuUsedTime * 100 / (totalTime * Environment.ProcessorCount), 2);
            return cpuUsed;
        }

        /// <summary>
        /// 获取指定进程cpu使用情况
        /// </summary>
        /// <returns></returns>
        public static double GetLinuxCpuUsage(Process process)
        {
            try
            {
                string output = "";

                var info = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    RedirectStandardOutput = true
                };
                info.Arguments = $"-c \"pidstat -p {process.Id} -u 1\"";

                using (var p = Process.Start(info))
                {
                    output = p.StandardOutput.ReadLine();
                    //LogHelper.Info(output);
                    output = p.StandardOutput.ReadLine();
                    //LogHelper.Info(output);
                    output = p.StandardOutput.ReadLine();
                    //LogHelper.Info(output);
                    output = p.StandardOutput.ReadLine();
                    //LogHelper.Info(output);
                }

                if (string.IsNullOrWhiteSpace(output))
                {
                    return 0;
                }

                /* 
2020-07-20 10:18:01,094 [1] INFO  Linux 3.10.0-1062.12.1.el7.x86_64 (localhost.localdomain) 	2020年07月20日 	_x86_64_	(4 CPU)

10时18分00秒   UID       PID    %usr %system  %guest    %CPU   CPU  Command
10时18分00秒     0     17371    0.00    0.00    0.00    0.00     1  dotnet

                 */
                var arr = output.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                return double.Parse(arr[6]);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        #endregion

        #region 内存相关

        /// <summary>
        /// 获取进程物理内存占用(返回单位MB)
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static double GetMemoryUsage(Process process)
        {
            long bytes = process.WorkingSet64;
            return BytesToMB(bytes);
        }

        /// <summary>
        /// 获取进程虚拟内存占用(返回单位MB)
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static double GetVirtualMemoryUsage(Process process)
        {
            long bytes = process.PeakPagedMemorySize64;
            return BytesToMB(bytes);
        }

        /// <summary>
        /// 获取进程物理内存占用(返回单位MB)
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static double GetLinuxMemoryUsage(Process process)
        {
            try
            {
                string output = "";

                var info = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    RedirectStandardOutput = true
                };
                info.Arguments = $"-c \"pidstat -p {process.Id} -r 1\"";

                using (var p = Process.Start(info))
                {
                    //output = p.StandardOutput.ReadToEnd();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();
                }

                if (string.IsNullOrWhiteSpace(output))
                {
                    return 0;
                }

                /* 
Linux 3.10.0-1062.12.1.el7.x86_64 (localhost.localdomain) 	2020年07月20日 	_x86_64_	(4 CPU)

10时08分28秒   UID       PID  minflt/s  majflt/s     VSZ    RSS   %MEM  Command
10时08分28秒     0     27874      0.04      0.00 23724596 404492   1.23  dotnet

                 */
                var arr = output.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                return Math.Round(double.Parse(arr[6]) / 1024, 2);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 获取Windows系统总物理内存使用情况(总量/已使用/未使用)(返回单位MB)
        /// </summary>
        /// <returns></returns>
        public static MemoryMetrics GetWindowsMemoryMetrics()
        {
            try
            {
                string output = "";

                var info = new ProcessStartInfo();
                info.FileName = "wmic";
                info.Arguments = "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value";
                info.RedirectStandardOutput = true;

                using (var process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd();
                }

                /*
FreePhysicalMemory=2527660

TotalVisibleMemorySize=8306868               
                 */

                if (string.IsNullOrWhiteSpace(output))
                {
                    return null;
                }

                var lines = output.Trim().Split('\n');
                var freeParts = lines[0].Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                var totalParts = lines[1].Split(new char[] {'='}, StringSplitOptions.RemoveEmptyEntries);

                MemoryMetrics memoryMetrics = new MemoryMetrics()
                {
                    Total = Math.Round(double.Parse(totalParts[1]) / 1024, 0),
                    Free = Math.Round(double.Parse(freeParts[1]) / 1024, 0)
                };
                memoryMetrics.Used = memoryMetrics.Total - memoryMetrics.Free;

                return memoryMetrics;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取linux系统总物理内存使用情况(总量/已使用/未使用)(返回单位MB)
        /// </summary>
        /// <returns></returns>
        public static MemoryMetrics GetLinuxMemoryMetrics()
        {
            try
            {
                string output = "";

                var info = new ProcessStartInfo();
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"free -m\"";
                info.RedirectStandardOutput = true;

                using (var process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd();
                }

                /*  单位应该是MB
              total        used        free      shared  buff/cache   available
Mem:          64299       16049        3002           2       45246       47763
Swap:          8191           0        8191
                 */

                if (string.IsNullOrWhiteSpace(output))
                {
                    return null;
                }

                var lines = output.Trim().Split('\n');
                var memory = lines[1].Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                MemoryMetrics memoryMetrics = new MemoryMetrics()
                {
                    Total = double.Parse(memory[1]),
                    Used = double.Parse(memory[2]),
                };
                memoryMetrics.Free = memoryMetrics?.Total - memoryMetrics?.Used;
                memoryMetrics.FreePercent = Math.Round(double.Parse(memory[2]) * 100.00 / double.Parse(memory[1]), 3);

                return memoryMetrics;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region 硬盘相关

        /// <summary>
        /// 获取当前驱动使用情况
        /// </summary>
        public static List<DriveMetrics> GetDriveMetrics()
        {
            DriveInfo[] DriveInfoArr = DriveInfo.GetDrives();
            List<DriveMetrics> result = null;
            foreach (var drv in DriveInfoArr)
            {
                if (drv != null)
                {
                    DriveMetrics driveMetrics = new DriveMetrics();
                    if (result == null)
                    {
                        result = new List<DriveMetrics>();
                    }

                    if (drv.IsReady && drv.DriveType != DriveType.Removable && drv.TotalSize > 0)
                    {
                        driveMetrics.Name = drv.Name;
                        driveMetrics.IsReady = drv.IsReady;
                        driveMetrics.Total = BytesToGB(drv.TotalSize);
                        driveMetrics.Free = BytesToGB(drv.AvailableFreeSpace);
                        driveMetrics.Used = BytesToGB(drv.TotalSize - drv.AvailableFreeSpace);
                        driveMetrics.FreePercent = Math.Round(drv.AvailableFreeSpace * 100.00 / drv.TotalSize, 3);
                        result.Add(driveMetrics);
                    }
                }
            }

            return result;
        }

        #endregion

        #region 磁盘IO

        /// <summary>
        /// 获取指定进程cpu使用情况(Linux)
        /// </summary>
        /// <returns></returns>
        public static DiskIO GetLinuxDiskIO(Process process)
        {
            DiskIO diskIO = new DiskIO();
            try
            {
                string output = "";

                var info = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    RedirectStandardOutput = true
                };
                info.Arguments = $"-c \"pidstat -p {process.Id} -d 1\"";

                using (var p = Process.Start(info))
                {
                    //output = p.StandardOutput.ReadToEnd();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();

                    output = p.StandardOutput.ReadLine();
                }

                if (string.IsNullOrWhiteSpace(output))
                {
                    return diskIO;
                }

                /* 
Linux 3.10.0-1062.12.1.el7.x86_64 (localhost.localdomain) 	2020年07月20日 	_x86_64_	(4 CPU)

10时07分03秒   UID       PID   kB_rd/s   kB_wr/s kB_ccwr/s  Command
10时07分03秒     0     27874      0.01      0.02      0.00  dotnet

                 */
                var arr = output.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                diskIO.Read = double.Parse(arr[3]);
                diskIO.Read = double.Parse(arr[4]);

                return diskIO;
            }
            catch (Exception ex)
            {
                return diskIO;
            }
        }

        #endregion

        #region 网络IO

        /// <summary>
        /// 获取系统网络吞吐(Linux)
        /// </summary>
        /// <returns></returns>
        public static NetIO GetLinuxNetIO()
        {
            NetIO netIO = new NetIO();
            try
            {
                NetIO first = ReadProcNetDev();
                Thread.Sleep(1000);
                NetIO second = ReadProcNetDev();

                netIO.Receive = second.Receive - first.Receive;
                netIO.Transmit = second.Transmit - first.Transmit;

                return netIO;
            }
            catch (Exception ex)
            {
                return netIO;
            }
        }

        /// <summary>
        /// 读取/proc/net/dev文件(Linux)
        /// </summary>
        /// <returns></returns>
        private static NetIO ReadProcNetDev()
        {
            NetIO netIO = new NetIO();
            try
            {
                string output = "";

                var info = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    RedirectStandardOutput = true
                };
                info.Arguments = $"-c \"cat /proc/net/dev\"";

                using (var p = Process.Start(info))
                {
                    output = p.StandardOutput.ReadToEnd();
                }

                if (string.IsNullOrWhiteSpace(output))
                {
                    return netIO;
                }

                long receive = 0;
                long transmit = 0;
                var lines = output.Trim().Split('\n');
                for (int i = 2; i < lines.Length; i++)
                {
                    var arr = lines[i].Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    receive += long.Parse(arr[1]);
                    transmit += long.Parse(arr[9]);
                }

                return new NetIO()
                {
                    Receive = receive,
                    Transmit = transmit
                };
            }
            catch (Exception ex)
            {
                return netIO;
            }
        }

        #endregion

        #region 单位转换

        /// <summary>
        /// 字节转MB，保留3位小数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double BytesToMB(long bytes)
        {
            return Math.Round(bytes * 1.00 / (1024 * 1024), 3);
        }

        /// <summary>
        /// 字节转GB，保留3位小数
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double BytesToGB(long bytes)
        {
            return Math.Round(bytes * 1.00 / (1024 * 1024 * 1024), 3);
        }

        #endregion
    }

    #region 数据模型

    /// <summary>
    /// 系统使用情况
    /// </summary>
    public class SysUsage
    {
        /// <summary>
        /// 当前进程占用CPU(百分比)
        /// </summary>
        [JsonProperty("processCpu")]
        public double ProcessCpu { get; set; }

        /// <summary>
        /// 当前进程占用物理内存(MB)
        /// </summary>
        [JsonProperty("processMemory")]
        public double ProcessMemory { get; set; }

        /// <summary>
        /// 当前进程所在驱动使用情况
        /// </summary>
        [JsonProperty("processDriveInfo")]
        public DriveMetrics ProcessDriveInfo { get; set; }

        /// <summary>
        /// 当前进程磁盘IO
        /// </summary>
        [JsonProperty("processDiskIO")]
        public DiskIO ProcessDiskIO { get; set; }

        /// <summary>
        /// 系统内存使用情况
        /// </summary>
        [JsonProperty("systemMemoryInfo")]
        public MemoryMetrics SystemMemoryInfo { get; set; }

        /// <summary>
        /// 系统网络IO
        /// </summary>
        [JsonProperty("systemNetIO")]
        public NetIO SystemNetIO { get; set; }
    }

    /// <summary>
    /// 系统内存情况
    /// </summary>
    public class MemoryMetrics
    {
        /// <summary>
        /// 总量(MB)
        /// </summary>
        [JsonProperty("total")]
        public double? Total { get; set; }

        /// <summary>
        /// 已使用(MB)
        /// </summary>
        [JsonProperty("used")]
        public double? Used { get; set; }

        /// <summary>
        /// 未使用(MB)
        /// </summary>
        [JsonProperty("free")]
        public double? Free { get; set; }

        /// <summary>
        /// 空闲百分比
        /// </summary>
        [JsonProperty("freePercent")]
        public double? FreePercent { get; set; }
    }

    [Serializable]
    /// <summary>
    /// 系统硬盘情况
    /// </summary>
    public class DriveMetrics
    {
        /// <summary>
        /// 驱动名
        /// </summary>
        [JsonProperty("name")]
        public string? Name { get; set; }

        /// <summary>
        /// 驱动状态，是否可用
        /// </summary>
        [JsonProperty("isReady")]
        public bool? IsReady { get; set; }

        /// <summary>
        /// 总量(GB)
        /// </summary>
        [JsonProperty("total")]
        public double? Total { get; set; }

        /// <summary>
        /// 已使用(GB)
        /// </summary>
        [JsonProperty("used")]
        public double? Used { get; set; }

        /// <summary>
        /// 未使用(GB)
        /// </summary>
        [JsonProperty("free")]
        public double? Free { get; set; }

        /// <summary>
        /// 空闲百分比
        /// </summary>
        [JsonProperty("freePercent")]
        public double? FreePercent { get; set; }
    }

    /// <summary>
    /// 指定进程磁盘IO
    /// </summary>
    public class DiskIO
    {
        /// <summary>
        /// 读(kb/s)
        /// </summary>
        [JsonProperty("read")]
        public double Read { get; set; }

        /// <summary>
        /// 写(kb/s)
        /// </summary>
        [JsonProperty("write")]
        public double Write { get; set; }

        public DiskIO()
        {
            Read = 0;
            Write = 0;
        }
    }

    /// <summary>
    /// 系统IO
    /// </summary>
    public class NetIO
    {
        /// <summary>
        /// 接受(bytes/s)
        /// </summary>
        [JsonProperty("receive")]
        public double Receive { get; set; }

        /// <summary>
        /// 发送(bytes/s)
        /// </summary>
        [JsonProperty("transmit")]
        public double Transmit { get; set; }

        public NetIO()
        {
            Receive = 0;
            Transmit = 0;
        }
    }

    #endregion
}