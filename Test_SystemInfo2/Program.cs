using System;
using System.Collections.Generic;
using CZGL;

namespace Test_SystemInfo2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var opt = SystemInfoLibrary.OperatingSystem.OperatingSystemInfo.GetOperatingSystemInfo();
            var hard = opt.Hardware;
            var EnvironmentVariables = CZGL.SystemInfo.EnvironmentInfo.GetEnvironmentVariables();
            var GetMachineInfo = CZGL.SystemInfo.EnvironmentInfo.GetMachineInfo();
            var GetMachineInfoValue = CZGL.SystemInfo.EnvironmentInfo.GetMachineInfoValue();
            var GetSystemPlatformInfo = CZGL.SystemInfo.EnvironmentInfo.GetSystemPlatformInfo();
            var GetSystemRunInfo = CZGL.SystemInfo.EnvironmentInfo.GetSystemRunInfo();
            var GetSystemPlatformInfoValue = CZGL.SystemInfo.EnvironmentInfo.GetSystemPlatformInfoValue();
            var GetSystemRunInfoValue = CZGL.SystemInfo.EnvironmentInfo.GetSystemRunInfoValue();
            
            foreach (var ev in EnvironmentVariables)
            {
                if (!ev.Equals(null) && !string.IsNullOrEmpty(ev.Key))
                {
                    Console.WriteLine("EnvironmentVariables->" + ev.Key + "->" +
                                      ev.Value);
                }
            }

            var gmi = GetMachineInfo.Item2;
            foreach (var t in gmi)
            {
                if (!t.Equals(null) && !string.IsNullOrEmpty(t.Key))
                {
                    if (!t.Key.ToString().Equals("系统所有进程各种使用的内存"))
                    {
                        Console.WriteLine("GetMachineInfo->" + GetMachineInfo.Item1 + "->" + t.Key + "->" + t.Value);
                    }
                    else
                    {
                        var tmp = t.Value as KeyValuePair<System.String, System.Int64>[];
                        foreach (var tt in tmp)
                        {
                            if (!string.IsNullOrEmpty(tt.Key))
                            {
                                Console.WriteLine("GetMachineInfo->" + GetMachineInfo.Item1 + "->" + t.Key + "->" +
                                                  tt.Key +
                                                  "->" + tt.Value);
                            }
                        }
                    }
                }
            }


            foreach (var gmiv in GetMachineInfoValue)
            {
                if (!gmiv.Equals(null) && !string.IsNullOrEmpty(gmiv.Key))
                {
                    if (!gmiv.Key.Equals("AllProcessMemory"))
                    {
                        Console.WriteLine("GetMachineInfoValue->" + gmiv.Key + "->" +
                                          gmiv.Value);
                    }
                    else
                    {
                        var tmp = gmiv.Value as KeyValuePair<System.String, System.Int64>[];
                        foreach (var tt in tmp)
                        {
                            if (!string.IsNullOrEmpty(tt.Key))
                            {
                                Console.WriteLine("GetMachineInfoValue->" + gmiv.Key + "->" +
                                                  tt.Key +
                                                  "->" + tt.Value);
                            }
                        }
                    }
                }
            }

            var gspi = GetSystemPlatformInfo.Item2;
            foreach (var t in gspi)
            {
                if (!t.Equals(null) && !string.IsNullOrEmpty(t.Key))
                {
                    Console.WriteLine("GetSystemPlatformInfo->" + GetSystemPlatformInfo.Item1 + "->" + t.Key + "->" +
                                      t.Value);
                }
            }
            
            
            var gsri = GetSystemRunInfo.Item2;
            foreach (var t in gsri)
            {
                if (!t.Equals(null) && !string.IsNullOrEmpty(t.Key))
                {
                    Console.WriteLine("GetSystemRunInfo->" + GetSystemRunInfo.Item1 + "->" + t.Key + "->" + t.Value);
                }
            }
            foreach (var gspiv in GetSystemPlatformInfoValue)
            {
                if (!gspiv.Equals(null) && !string.IsNullOrEmpty(gspiv.Key))
                {
                    Console.WriteLine("GetSystemPlatformInfoValue->" + gspiv.Key + "->" +
                                      gspiv.Value);
                }
            }
            foreach (var gsriv in GetSystemRunInfoValue)
            {
                if (!gsriv.Equals(null) && !string.IsNullOrEmpty(gsriv.Key))
                {
                    Console.WriteLine("GetSystemRunInfoValue->" + gsriv.Key + "->" +
                                      gsriv.Value);
                }
            }
            
            Console.WriteLine("TatolMem->"+hard.RAM.Total);
            Console.WriteLine("FreeMem->"+hard.RAM.Free);
            
        }
    }
}