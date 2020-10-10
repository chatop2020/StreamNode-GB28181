using System;
using System.IO;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace Logger
{
    public static class Logger
    {
        private static ILog logger = null;
        public static bool init = true;
        private static object lockobj=new object();
        static Logger()
        {
            lock (lockobj)
            {
                ILoggerRepository repository = LogManager.CreateRepository("NETCoreRepository");
                    XmlConfigurator.Configure(repository,
                        new FileInfo(Environment.CurrentDirectory + "/Config/" + "logconfig.xml"));
                    _instance = LogManager.GetLogger(repository.Name, "StreamNode");
            }
        }
        
        private static readonly ILog _instance =null;

        public static void Info(string msg)
        {
         
            _instance.Info(msg); 
        }
        
        public static void Debug(string msg)
        {
          
            _instance.Debug(msg); 
        }
        
        public static void Error(string msg)
        {
            
            _instance.Error(msg); 
        }

        public static void Warn(string msg)
        {
           
            _instance.Warn(msg);
        }

        public static void Fatal(string msg)
        {
            
            _instance.Fatal(msg);
        }
        
    }
}