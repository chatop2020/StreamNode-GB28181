using System;
using System.IO;

namespace CommonFunctions
{
    public static class LogWriter
    {
        public static object lockLogFile = new object();

        public static void WriteLog(string message, string info = "", ConsoleColor color = ConsoleColor.Gray)
        {
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = color;
            }

            string logPath = Common.SystemLogPath;
            DateTime now = DateTime.Now;
            string logpath = string.Format(logPath + @"streamnodelog_Y{0}M{1}D{2}.log", now.Year, now.Month, now.Day);
            string saveLogTxt = "[" + DateTime.Now.ToString() + "]\t" + message + "\t" + info + "\r\n";
            Console.WriteLine(saveLogTxt.Trim());
            if (color != ConsoleColor.Gray)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
            }

            lock (lockLogFile)
            {
                File.AppendAllText(logpath, saveLogTxt);
            }
        }
    }
}