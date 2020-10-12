using System;
using SystemInfoLibrary;

namespace Test_SystemInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var a = SystemInfoLibrary.OperatingSystem.OperatingSystemInfo.GetOperatingSystemInfo();
            
            var h = a.Hardware;
            var o = a.OperatingSystemType;
            Console.WriteLine("a.Architecture->"+a.Architecture);
            Console.WriteLine("a.Name->"+a.Name);
            Console.WriteLine("a.FrameworkVersion->"+a.FrameworkVersion);
            Console.WriteLine("a.IsMono->"+a.IsMono);
            Console.WriteLine("a.JavaVersion->"+a.JavaVersion);

            foreach (var cpu in h.CPUs)
            {
               
                Console.WriteLine("cpu.Architecture->"+cpu.Architecture);
                Console.WriteLine("cpu.Brand->"+cpu.Brand);
                Console.WriteLine("cpu.Cores->"+cpu.Cores);
                Console.WriteLine("cpu.Frequency->"+cpu.Frequency);
                Console.WriteLine("cpu.Name->"+cpu.Name);
            }

            try
            {
                foreach (var gpu in h.GPUs)
                {
                    Console.WriteLine("gpu.Resolution->" + gpu.Resolution);
                    Console.WriteLine("gpu.Brand->" + gpu.Brand);
                    Console.WriteLine("gpu.MemoryTotal->" + gpu.MemoryTotal);
                    Console.WriteLine("gpu.RefreshRate->" + gpu.RefreshRate);
                    Console.WriteLine("gpu.Name->" + gpu.Name);
                }
            }
            catch
            {
                
            }

            Console.WriteLine("RAM.free->"+h.RAM.Free);
            Console.WriteLine("RAM.total->"+h.RAM.Total);
            
            Console.WriteLine("systemType->"+o.ToString());
            
            
           Console.WriteLine();
        }
    }
}