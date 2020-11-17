using System;
using System.Diagnostics;

namespace Test_RunProcess
{
    public class TestProcess
    {
        public delegate void DelReadStdOutput(string result);

        public delegate void DelReadErrOutput(string result);

        public event DelReadStdOutput ReadStdOutput;
        public event DelReadErrOutput ReadErrOutput;

        public TestProcess()
        {
            ReadStdOutput += new DelReadStdOutput(ReadStdOutputAction);
            ReadErrOutput += new DelReadErrOutput(ReadErrOutputAction);
        }

        public Process RealAction(string StartFileName, string StartFileArg)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName; // 命令
            CmdProcess.StartInfo.Arguments = StartFileArg; // 参数

            CmdProcess.StartInfo.CreateNoWindow = true; // 不创建新窗口
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true; // 重定向输入
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            CmdProcess.StartInfo.RedirectStandardError = true; // 重定向错误输出
            CmdProcess.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            CmdProcess.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
            CmdProcess.EnableRaisingEvents = true; // 启用Exited事件
            CmdProcess.Exited += new EventHandler(CmdProcess_Exited); // 注册进程结束事件
            CmdProcess.Start();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
            return CmdProcess;
        }

        private void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ReadStdOutput(e.Data);
            }
        }

        private void p_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                ReadErrOutput(e.Data);
            }
        }

        private void ReadStdOutputAction(string result)
        {
            Console.WriteLine("STD->" + result);
        }

        private void ReadErrOutputAction(string result)
        {
            Console.WriteLine("ERR->" + result);
        }

        private void CmdProcess_Exited(object sender, EventArgs e)
        {
            // 执行结束后触发
            Console.WriteLine("进程退出->");
        }
    }
}