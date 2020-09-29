using System;
using System.Collections.Generic;
using System.Threading;

namespace CommonFunctions
{
    /// <summary>
    /// session类结构
    /// </summary>
    public class Session
    {
        private string _allowKey = null!;
        private long _expires; //过期时间
        private string _refreshCode = null!;
        private string _sessionCode = null!;

        /// <summary>
        /// 授权key
        /// </summary>
        public string AllowKey
        {
            get => _allowKey;
            set => _allowKey = value;
        }

        /// <summary>
        /// session刷新code
        /// </summary>
        public string RefreshCode
        {
            get => _refreshCode;
            set => _refreshCode = value;
        }

        /// <summary>
        /// session code
        /// </summary>
        public string SessionCode
        {
            get => _sessionCode;
            set => _sessionCode = value;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        public long Expires
        {
            get => _expires;
            set => _expires = value;
        }
    }

    /// <summary>
    /// session管理
    /// </summary>
    public class SessionManager
    {
        private List<Session> _sessionList = new List<Session>();

        private byte addMin = 50;

        /// <summary>
        /// Session管理构造函数
        /// </summary>
        /// <exception cref="Exception"></exception>
        public SessionManager()
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    clearExpires();
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("Session管理线程启动异常...，系统退出", ex.Message + "\r\n" + ex.StackTrace);
                    Common.KillSelf();
                }
            })).Start();
        }

        /// <summary>
        /// session列表
        /// </summary>
        public List<Session> SessionList
        {
            get => _sessionList;
            set => _sessionList = value;
        }

        /// <summary>
        /// 清空已经过期的session
        /// </summary>
        private void clearExpires()
        {
            while (true)
            {
                try
                {
                    lock (this)
                    {
                        foreach (var session in _sessionList)
                        {
                            if (session.Expires <= Common.GetTimeStampMilliseconds())
                            {
                                _sessionList.Remove(session);
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("报错了：\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                    continue;
                }
            }
        }

        /// <summary>
        /// 刷新Session
        /// </summary>
        /// <param name="session">旧的session</param>
        /// <returns></returns>
        public Session RefreshSession(Session session)
        {
            bool found = false;
            int i = 0;
            lock (this)
            {
                for (i = 0; i <= _sessionList.Count - 1; i++)
                {
                    if (_sessionList[i].AllowKey.Trim().ToLower().Equals(session.AllowKey.Trim().ToLower()) &&
                        _sessionList[i].RefreshCode.Trim().ToLower().Equals(session.RefreshCode.Trim().ToLower())
                    )
                    {
                        _sessionList[i].SessionCode = Common.CreateUuid()!;
                        _sessionList[i].RefreshCode = Common.CreateUuid()!;
                        _sessionList[i].Expires =
                            Common.GetTimeStampMilliseconds() + (addMin * 1000 * 60);
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    return _sessionList[i];
                }
                else
                {
                    return null!;
                }
            }
        }

        /// <summary>
        /// 创建一个新的Session
        /// </summary>
        /// <param name="allowKey"></param>
        /// <returns></returns>
        public Session NewSession(string allowKey)
        {
            if (_sessionList != null)
            {
                Session s = _sessionList.FindLast(x => x.AllowKey.Trim().ToLower().Equals(allowKey.Trim().ToLower()))!;
                if (s != null)
                {
                    return s;
                }
            }

            Session session = new Session()
            {
                AllowKey = allowKey,
                SessionCode = Common.CreateUuid()!,
                RefreshCode = Common.CreateUuid()!,
                Expires = Common.GetTimeStampMilliseconds() + (addMin * 1000 * 60),
            };
            lock (this)
            {
                _sessionList!.Add(session);
            }

            return session;
        }
    }
}