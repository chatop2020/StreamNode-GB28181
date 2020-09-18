using System;
using System.Threading.Tasks;
using LibGB28181SipGate;

namespace Test_Gb28181SipGate
{
    class Program
    {
        private static SipCoreHelper _process;

        static void Main(string[] args)
        {
            _process = new SipCoreHelper(true);
            _process.Start();
            string cmd = "a";
            string p1 = "";
            string p2 = "";
            string p3 = "";
            while (!string.IsNullOrEmpty(cmd))
            {
                cmd = Console.ReadLine();
                while (string.IsNullOrEmpty(cmd))
                {
                    cmd = Console.ReadLine();
                }

                cmd = cmd.Trim().ToLower();

                if (cmd.Contains(' '))
                {
                    string[] strArr = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (strArr.Length > 0)
                    {
                        cmd = strArr[0];
                    }

                    for (int i = 1; i <= strArr.Length - 1; i++)
                    {
                        if (i <= 3)
                        {
                            if (i == 1)
                            {
                                p1 = strArr[i];
                            }

                            if (i == 2)
                            {
                                p2 = strArr[i];
                            }

                            if (i == 3)
                            {
                                p3 = strArr[i];
                            }
                        }
                    }
                }

                switch (cmd)
                {
                    case "start":
                        _process.Start();
                        break;
                    case "stop":
                        _process.Stop();
                        break;
                    case "exit":
                        return;
                        break;
                    case "gdev":
                        Task.Factory.StartNew(() => _process.GetDeviceList(p1));

                        break;
                    case "gst":
                        _process.GetDeviceStatus(p1);
                        break;
                    case "video":
                        Task.Factory.StartNew(() => _process.ReqLive(p1));

                        break;
                    case "bye":
                        Task.Factory.StartNew(() => _process.ReqStopLive(p1));
                        break;
                    case "ptz":
                        if (string.IsNullOrEmpty(p3))
                        {
                            p3 = "30";
                        }

                        Task.Factory.StartNew(() => _process.ReqPtzControl(p1, p2, int.Parse(p3)));
                        break;

                    case "sta":
                        foreach (var sip in _process.SipDeviceList)
                        {
                            Console.WriteLine(sip.ToString());
                            foreach (var camera in sip.CameraExList)
                            {
                                Console.WriteLine(camera.ToString());
                            }
                        }

                        break;
                    case "count":
                        Console.WriteLine(_process.GetTaskListCount());
                        break;
                }
            }
        }
    }
}