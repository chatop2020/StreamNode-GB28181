using System;
using System.Diagnostics;

namespace Test_RunProcess
{
    public class ProcessHelper
    {

        private DataReceivedEventHandler _std = null;
        private DataReceivedEventHandler _err = null;
        private EventHandler _exitEventHandle = null;


        public ProcessHelper(DataReceivedEventHandler std=null, DataReceivedEventHandler err=null,EventHandler exitEvent=null)
        {
            _std = std;
            _err = err;
            _exitEventHandle = exitEvent;
        }
        
        public Process RunProcess(string StartFileName, string StartFileArg)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName; // 命令
            CmdProcess.StartInfo.Arguments = StartFileArg; // 参数

            CmdProcess.StartInfo.CreateNoWindow = true; // 不创建新窗口
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true; // 重定向输入
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            CmdProcess.StartInfo.RedirectStandardError = true; // 重定向错误输出
            if (_std != null)
            {
                CmdProcess.OutputDataReceived +=_std;
            }

            if (_err != null)
            {
                CmdProcess.ErrorDataReceived +=_err;
            }

            CmdProcess.EnableRaisingEvents = true; // 启用Exited事件
            if (_exitEventHandle != null)
            {
                CmdProcess.Exited +=_exitEventHandle; // 注册进程结束事件
            }

            CmdProcess.Start();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
            return CmdProcess;
        }

      

       
    }
}