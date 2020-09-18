using System.Diagnostics;

namespace StreamMediaServerKeeper
{
    /// <summary>
    /// 
    /// </summary>
    public static class LinuxShell
    {
        private const string processName = "/bin/bash";

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="command">命令内容</param>
        /// <returns>返回执行结果</returns>
        public static bool Run(string command) => Run(command, -1);

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="command">命令内容</param>
        /// <param name="milliseconds">超时时间（负数表示无限等待）</param>
        /// <returns>返回执行结果</returns>
        public static bool Run(string command, int milliseconds)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //不使用shell以免出现操作系统shell出错
                    process.StartInfo.CreateNoWindow = true; //不显示窗口

                    process.StartInfo.Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"";
                    if (process.Start())
                    {
                        return process.WaitForExit(milliseconds);
                    }

                    return false;
                }
            }
            catch //异常直接返回错误
            {
                //异常处理
                return false;
            }
        }

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="command">命令内容</param>
        /// <param name="milliseconds">超时时间（负数表示无限等待）</param>
        /// <param name="stdOutput">结果输出</param>
        /// <returns></returns>
        public static bool Run(string command, int milliseconds, out string stdOutput)
        {
            stdOutput = null!;
            try
            {
                string escapedArgs = command.Replace("\"", "\\\"").Replace("$", "\\$");
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //不使用shell以免出现操作系统shell出错
                    process.StartInfo.CreateNoWindow = true; //不显示窗口
                    process.StartInfo.RedirectStandardOutput = true;

                    process.StartInfo.Arguments = $"-c \"{escapedArgs}\"";

                    bool result = process.Start();
                    if (result)
                    {
                        result = process.WaitForExit(milliseconds);
                    }

                    if (result)
                    {
                        stdOutput = process.StandardOutput.ReadToEnd();
                    }

                    return result;
                }
            }
            catch //异常直接返回错误
            {
                //异常处理
                return false;
            }
        }

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        /// <param name="command">命令内容</param>
        /// <param name="milliseconds">超时时间（负数表示无限等待）</param>
        /// <param name="stdOutput">结果输出</param>
        /// <param name="stdError">错误输出</param>
        /// <returns></returns>
        public static bool Run(string command, int milliseconds, out string stdOutput, out string stdError)
        {
            stdOutput = null!;
            stdError = null!;
            try
            {
                string escapedArgs = command.Replace("\"", "\\\"");

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = processName;
                    process.StartInfo.UseShellExecute = false; //不使用shell以免出现操作系统shell出错
                    process.StartInfo.CreateNoWindow = true; //不显示窗口
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.Arguments = $"-c \"{escapedArgs}\"";
                    bool result = process.Start();
                    if (result)
                    {
                        result = process.WaitForExit(milliseconds);
                    }

                    if (result)
                    {
                        stdOutput = process.StandardOutput.ReadToEnd();
                        stdError = process.StandardError.ReadToEnd()!;
                    }

                    return result;
                }
            }
            catch //异常直接返回错误
            {
                //异常处理
                return false;
            }
        }
    }
}