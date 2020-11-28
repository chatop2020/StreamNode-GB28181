using System;
using System.Threading;
using CommonFunctions;

namespace StreamNodeWebApi.AutoTasker
{
    /// <summary>
    /// 流媒体服务器健康检查线程
    /// 每次循环都将所有流媒体服务器循环一次，尝试调取流媒体服务器/Health接口
    /// /Health接口正确返回则将流媒体服务器的KeepAlive设置成当前时间
    /// 当发现流媒体服务器的KeepAlive与当前服务器时间时长3秒或以上时，考虑流媒体服务器掉线
    /// 把该流媒体服务器从流媒体服务器列表中移除，等其恢复正常后通过Webhook接口重新注册自己
    /// </summary>
    public class StreamNodeKeeperMonitor
    {
        private void Monitor()
        {
            while (true)
            {
                try
                {
                    for (int i = Common.MediaServerList.Count - 1; i >= 0; i--)
                    {
                        if (Common.MediaServerList[i] != null && Common.MediaServerList[i].Health)
                        {
                            Common.MediaServerList[i].KeepAlive = DateTime.Now;
                            Common.MediaServerList[i].UpdateTime = DateTime.Now;
                        }

                        if ((DateTime.Now - Common.MediaServerList[i].KeepAlive).TotalMinutes > 2)
                        {
                            lock (Common.MediaServerLock)
                            {
                                Common.MediaServerList[i] = null;
                            }
                        }
                    }

                    lock (Common.MediaServerLock)
                    {
                        Common.RemoveNull(Common.MediaServerList);
                    }

                    Thread.Sleep(1000);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public StreamNodeKeeperMonitor()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    Monitor();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }
    }
}