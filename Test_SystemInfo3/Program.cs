using System;
using CZGL.SystemInfo;


namespace Test_SystemInfo3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("================SystemPlatformInfo================");
            Console.WriteLine("FrameworkDescription->" + SystemPlatformInfo.FrameworkDescription);
            Console.WriteLine("FrameworkVersion->" + SystemPlatformInfo.FrameworkVersion);
            Console.WriteLine("MachineName->" + SystemPlatformInfo.MachineName);
            Console.WriteLine("ProcessArchitecture->" + SystemPlatformInfo.ProcessArchitecture);
            Console.WriteLine("ProcessorCount->" + SystemPlatformInfo.ProcessorCount);
            Console.WriteLine("SystemDirectory->" + SystemPlatformInfo.SystemDirectory);
            Console.WriteLine("UserName->" + SystemPlatformInfo.UserName);
            Console.WriteLine("IsUserInteractive->" + SystemPlatformInfo.IsUserInteractive);
            Console.WriteLine("MemoryPageSize->" + SystemPlatformInfo.MemoryPageSize);
            Console.WriteLine("OSArchitecture->" + SystemPlatformInfo.OSArchitecture);
            Console.WriteLine("OSDescription->" + SystemPlatformInfo.OSDescription);
            Console.WriteLine("OSVersion->" + SystemPlatformInfo.OSVersion);
            Console.WriteLine("UserDomainName->" + SystemPlatformInfo.UserDomainName);
            Console.WriteLine("OSPlatformID->" + SystemPlatformInfo.OSPlatformID);
            foreach (var disk in SystemPlatformInfo.GetLogicalDrives)
            {
                Console.WriteLine("DISK->" + disk);
            }

            Console.WriteLine("================SystemPlatformInfo================");
            Console.WriteLine("================DiskInfo================");
            var disks = DiskInfo.GetDisks();
            foreach (var disk in disks)
            {
                Console.WriteLine("Id->" + disk.Id);
                Console.WriteLine("Name->" + disk.Name);
                Console.WriteLine("FileSystem->" + disk.FileSystem);
                Console.WriteLine("FreeSpace->" + disk.FreeSpace);
                Console.WriteLine("TotalSize->" + disk.TotalSize);
                Console.WriteLine("UsedSize->" + disk.UsedSize);
                var ret = disk.DriveInfo;
                var ret2 = disk.DriveType;
            }
            Console.WriteLine("================DiskInfo================");
            Console.WriteLine("================Network================");
            var network = NetworkInfo.GetNetworkInfo();
            Console.WriteLine("ReceivedLength->"+network.ReceivedLength);
            Console.WriteLine("SendLength->"+network.SendLength);
            Console.WriteLine("================Network================");
            Console.WriteLine("================Process================");
            
            
        }
    }
}