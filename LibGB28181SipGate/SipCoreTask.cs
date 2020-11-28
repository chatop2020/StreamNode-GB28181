using System;
using System.Linq;
using System.Threading;
using GB28181.Servers.SIPMonitor;

namespace LibGB28181SipGate
{
    [Serializable]
    public enum SipCommandType
    {
        GetDeviceList,
        InviteVideo,
        ByeVideo,
        PtzControl,
    }

    /// <summary>
    /// 用于执行sip指令，并监控异步执行状态
    /// </summary>
    [Serializable]
    public class SipCoreTask
    {
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private string _callId = "";
        private string _pDeviceId = "";
        private string _deviceId = "";
        private SipCommandType _sipCommandType;
        private SipCoreHelper _sipCoreHelper = null;

        public string CallId
        {
            get => _callId;
            set => _callId = value;
        }

        public string DeviceId
        {
            get => _deviceId;
            set => _deviceId = value;
        }

        public string PDeviceId
        {
            get => _pDeviceId;
            set => _pDeviceId = value;
        }

        public SipCommandType SipCommandType
        {
            get => _sipCommandType;
            set => _sipCommandType = value;
        }

        public SipCoreHelper SipCoreHelper
        {
            get => _sipCoreHelper;
            set => _sipCoreHelper = value;
        }

        /// <summary>
        /// 构造函数，用于初始化一个任务
        /// </summary>
        /// <param name="pDeviceId"></param>
        /// <param name="deviceId"></param>
        /// <param name="sipCoreHelper"></param>
        public SipCoreTask(string pDeviceId, string deviceId, SipCoreHelper sipCoreHelper)
        {
            _deviceId = deviceId;
            _sipCoreHelper = sipCoreHelper;
            _pDeviceId = pDeviceId;
        }

        /// <summary>
        /// 当任务得到回复时调用此方法来执行后续操作
        /// </summary>
        public void Next()
        {
            _autoResetEvent.Set();
        }

        /// <summary>
        /// 结束远端视频推流
        /// </summary>
        /// <returns></returns>
        public bool ByeLive()
        {
            var monitor = _sipCoreHelper.SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(_deviceId));
            if (monitor.Value != null)
            {
                var obj = monitor.Value;
                obj.ByeVideoReq(out _callId, true);
                var timeout = _autoResetEvent.WaitOne(5000);
                if (!timeout)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 启动远端视频推流,动态创建端口
        /// </summary>
        /// <returns></returns>
        public bool Invite(uint stid, string rtpServerIp, ushort rtpPort, bool tcp = false)
        {
            var monitor = _sipCoreHelper.SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(_deviceId));
            if (monitor.Value != null)
            {
                var obj = monitor.Value;
                var ret = obj.RealVideoReq(stid, out _callId, rtpServerIp, rtpPort, true, tcp);
                var timeout = _autoResetEvent.WaitOne(5000);
                if (!timeout)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 启动远端视频推流
        /// </summary>
        /// <returns></returns>
        public bool Invite(uint stid, out string ip, out int port)
        {
            ip = "";
            port = 0;
            var monitor = _sipCoreHelper.SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(_deviceId));
            if (monitor.Value != null)
            {
                var obj = monitor.Value;
                var ret = obj.RealVideoReq(stid, out _callId, out ip, out port, true);
                var timeout = _autoResetEvent.WaitOne(5000);
                if (!timeout)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 获取GB28181录像文件
        /// </summary>
        /// <returns></returns>
        public int RecordFileQuery(DateTime startTime, DateTime endTime, string type)
        {
            var monitor = _sipCoreHelper.SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(_deviceId));
            if (monitor.Value != null)
            {
                var obj = monitor.Value;
                /*< ! -- 录像产生类型(可选)time 或alarm 或 manual 或all--> <element name="Type" type="string"/>*/
                var ret = obj.RecordFileQuery(startTime, endTime, type, out _callId, true);
                var timeout = _autoResetEvent.WaitOne(5000);
                if (!timeout)
                {
                    return ret;
                }
                else
                {
                    return -1;
                }
            }

            return -1;
        }

        /// <summary>
        /// 获取sip客户端的设备列表
        /// </summary>
        /// <returns></returns>
        public bool GetDeviceList()
        {
            _sipCoreHelper.SipMessageCore.DeviceCatalogQuery(_deviceId, out _callId, true);
            var timeout = _autoResetEvent.WaitOne(5000);
            if (!timeout)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 对sip设备进和ptz控制
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public bool PtzContorl(string dir, int speed, string channelId = "")
        {
            var monitor = _sipCoreHelper.SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(_deviceId));
            if (monitor.Value != null)
            {
                var obj = monitor.Value;
                PTZCommand cmd = PTZCommand.Up;
                switch (dir.ToLower().Trim())
                {
                    case "up":
                        cmd = PTZCommand.Up;
                        break;
                    case "down":
                        cmd = PTZCommand.Down;
                        break;
                    case "left":
                        cmd = PTZCommand.Left;
                        break;
                    case "right":
                        cmd = PTZCommand.Right;
                        break;
                    case "upleft":
                        cmd = PTZCommand.UpLeft;
                        break;
                    case "upright":
                        cmd = PTZCommand.UpRight;
                        break;
                    case "downleft":
                        cmd = PTZCommand.DownLeft;
                        break;
                    case "downright":
                        cmd = PTZCommand.DownRight;
                        break;
                    case "stop":
                        cmd = PTZCommand.Stop;
                        break;
                    case "zoom1":
                        cmd = PTZCommand.Zoom1;
                        break;
                    case "zoom2":
                        cmd = PTZCommand.Zoom2;
                        break;
                    case "focus1":
                        cmd = PTZCommand.Focus1;
                        break;
                    case "focus2":
                        cmd = PTZCommand.Focus2;
                        break;
                    case "iris1":
                        cmd = PTZCommand.Iris1;
                        break;
                    case "iris2":
                        cmd = PTZCommand.Iris2;
                        break;
                }

                obj.PtzContrl(out _callId, cmd, speed, true, channelId);
                var timeout = _autoResetEvent.WaitOne(5000);
                if (!timeout)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }
    }
}