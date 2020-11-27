using System;
using System.Threading;
using CommonFunctions;
using CommonFunctions.ManageStructs;

namespace StreamNodeWebApi.AutoTasker
{
    /// <summary>
    /// 未完成
    /// </summary>
    public class MediaKeeperCheckKeepAlive
    {
        private void KeepAlive()
        {
            while (true)
            {
                lock (Common.MediaServerList)
                {
                    for (int i = Common.MediaServerList.Count - 1; i <= 0; i--)
                    {
                        var ret = Common.MediaServerList[i].GetIsRunning(out ResponseStruct rs);
                        if (ret == true && rs.Code == ErrorNumber.None)
                        {
                            Common.MediaServerList[i].KeepAlive = DateTime.Now;
                            Common.MediaServerList[i].UpdateTime = DateTime.Now;
                        }

                        if ((DateTime.Now - Common.MediaServerList[i].KeepAlive).TotalMinutes > 20)
                        {
                            
                            Common.MediaServerList[i] = null;
                        }
                    }
                    Common.RemoveNull(Common.MediaServerList);
                }


                Thread.Sleep(10000); 
            }
        }
        public MediaKeeperCheckKeepAlive()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    KeepAlive();
                }
                catch (Exception ex)
                {
                    //
                }
            })).Start();
        }
    }
}