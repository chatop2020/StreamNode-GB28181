using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GB28181;
using GB28181.App;
using GB28181.Cache;
using GB28181.Config;
using GB28181.Servers;
using GB28181.Servers.SIPMessage;
using GB28181.Servers.SIPMonitor;
using GB28181.Sys;
using GB28181.Sys.Model;
using GB28181.Sys.XML;
using SIPSorcery.SIP;
using SIPRequest = GB28181.SIPRequest;
using SIPResponse = GB28181.SIPResponse;
using SIPTransaction = GB28181.SIPTransaction;
using SIPTransport = GB28181.SIPTransport;

namespace LibGB28181SipGate
{
    public class SipCoreHelper
    {
        private List<SipCoreTask> TaskList = new List<SipCoreTask>();
        private SIPMessageCore _sipMessageCore = null;
        private bool _autoPushStream = false;
        private bool _isRunning = false;

        public Object SipDeviceLock = new Object();

        public SIPMessageCore SipMessageCore
        {
            get => _sipMessageCore;
            set => _sipMessageCore = value;
        }


        private List<SipDevice> _sipDeviceList = new List<SipDevice>();

        /// <summary>
        /// 保存所有sip设备
        /// </summary>
        public List<SipDevice> SipDeviceList
        {
            get => _sipDeviceList;
            set => _sipDeviceList = value;
        }

        public bool AutoPushStream
        {
            get => _autoPushStream;
            set => _autoPushStream = value;
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => _isRunning = value;
        }

        public int GetTaskListCount()
        {
            return TaskList.Count;
        }


        /// <summary>
        /// 自动任务
        /// </summary>
        private void AutoTask()
        {
            while (_isRunning)
            {
                for (int i = SipDeviceList.Count - 1; i >= 0; i--)
                {
                    var obj = _sipDeviceList[i];
                    if ((obj.SipDeviceStatus == SipDeviceStatus.Register ||
                         obj.SipDeviceStatus == SipDeviceStatus.GetDeviceList) &&
                        (obj.CameraExList == null || obj.CameraExList.Count == 0)) //自动获取设备列表
                    {
                        Logger.Logger.Info("发现设备处于注册状态，或者通道列表为空->开始获取通道列表->" + obj.IpAddress + "->" + obj.SipPort +
                                           "->" + obj.DeviceId);
                        if (GetDeviceList(obj.DeviceId))
                        {
                            Logger.Logger.Debug(
                                "获取通道列表成功了->" + obj.IpAddress + "->" + obj.SipPort + "->" + obj.DeviceId);

                            lock (SipDeviceLock)
                            {
                                obj.SipDeviceStatus = SipDeviceStatus.GetDeviceList;
                            }
                        }
                        else
                        {
                            Logger.Logger.Info("获取通道列表失败了，可能断线了，把它踢了，等他重新注册->" + obj.IpAddress + "->" + obj.SipPort +
                                               "->" + obj.DeviceId);
                            lock (SipDeviceLock)
                            {
                                obj.SipDeviceStatus = SipDeviceStatus.LooksLikeOffline;
                                obj.LastKeepAliveTime = DateTime.Now.AddMinutes(10); //用于将sip设备踢掉，等它重新注册
                            }
                        }

                        Thread.Sleep(100);
                    }


                    if (_autoPushStream) //自动推流
                    {
                        foreach (var camera in obj.CameraExList)
                        {
                            if (camera != null && camera.SipCameraStatus == SipCameraStatus.Idle)
                            {
                                if (ReqLive(camera.Camera.DeviceID))
                                {
                                    lock (SipDeviceLock)
                                    {
                                        camera.SipCameraStatus = SipCameraStatus.RealVideo;
                                    }
                                }
                                else
                                {
                                    lock (SipDeviceLock)
                                    {
                                        camera.SipCameraStatus = SipCameraStatus.Idle;
                                    }
                                }

                                Thread.Sleep(100);
                            }
                        }
                    }


                    var now = DateTime.Now;
                    var time = obj.LastKeepAliveTime;
                    var subTime = Math.Abs((now - time).TotalSeconds);
                    if (subTime > 60) //1分钟以上没有心跳的，就踢掉
                        // if (obj.LastKeepAliveTime.AddMinutes(2) < DateTime.Now) //2分钟以上没有心跳的，就踢掉
                    {
                        Logger.Logger.Info("踢掉超过60秒没有心跳的设备->IP->" + obj.IpAddress + "->Port->" + obj.SipPort +
                                           "->DevId->" + obj.DeviceId + "->" + subTime + "->" +
                                           now.ToString("yyyy-MM-dd HH:mm:ss") + "->" +
                                           time.ToString("yyyy-MM-dd HH:mm:ss"));
                        foreach (var camera in obj.CameraExList)
                        {
                            if (camera != null && camera.SipCameraStatus == SipCameraStatus.RealVideo)
                            {
                                try
                                {
                                    ReqStopLive(camera.Camera.DeviceID);
                                }
                                catch
                                {
                                    //
                                }
                            }
                        }

                        lock (SipDeviceLock)
                        {
                            _sipMessageCore._registrarCore.RemoveDeviceItem(obj.IpAddress + ":" + obj.SipPort);
                            _sipMessageCore.RemoteTransEPs.Remove(obj.IpAddress + ":" + obj.SipPort);
                            _sipDeviceList[i] = null;
                        }
                    }
                }

                lock (SipDeviceLock)
                {
                    RemoveNull(SipDeviceList);
                }

                Thread.Sleep(5000); //5秒一次
            }
        }

        /// <summary>
        /// 删除List<T>中null的记录
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        private static void RemoveNull<T>(List<T> list)
        {
            // 找出第一个空元素 O(n)
            int count = list.Count;
            for (int i = 0; i < count; i++)
                if (list[i] == null)
                {
                    // 记录当前位置
                    int newCount = i++;

                    // 对每个非空元素，复制至当前位置 O(n)
                    for (; i < count; i++)
                        if (list[i] != null)
                            list[newCount++] = list[i];

                    // 移除多余的元素 O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }

        public void TickOutDevice(string devId)
        {
            lock (SipDeviceLock)
            {
                var dev = SipDeviceList.FindLast(x => x.DeviceId.Equals(devId));
                if (dev != null)
                {
                    lock (SipDeviceLock)
                    {
                        dev.SipDeviceStatus = SipDeviceStatus.LooksLikeOffline;
                        dev.LastKeepAliveTime = DateTime.Now.AddMinutes(10); //用于将sip设备踢掉，等它重新注册
                    }
                }
            }
        }

        public SipCoreHelper(bool autoPushStream = false)
        {
            _autoPushStream = autoPushStream;
        }

        /// <summary>
        /// 添加设备报警信息
        /// </summary>
        /// <param name="sIPTransaction"></param>
        private void OnDeviceAlarmSubscribeReceived(SIPTransaction sIPTransaction)
        {
            lock (SipDeviceLock)
            {
                var dev = SipDeviceList.FindLast(x => x.IpAddress.Equals(sIPTransaction.RemoteEndPoint.Address));
                if (dev != null)
                {
                    dev.AlarmList.Add(JsonHelper.ToJson(sIPTransaction), DateTime.Now);
                }
            }
        }

        /// <summary>
        /// 设备注册  
        /// </summary>
        /// <param name="sipRequest"></param>
        /// <param name="sIPAccount"></param>
        private void OnSipRegisterReceived(SIPRequest sipRequest, SIPAccount sIPAccount)
        {
            lock (SipDeviceLock)
            {
                string ip = sipRequest.RemoteSIPEndPoint.Address.ToString();
                int port = sipRequest.RemoteSIPEndPoint.Port;
                string devid = sipRequest.Header.From.FromURI.User.Trim();
                var dev = SipDeviceList.FindLast(x => x.DeviceId.Equals(devid));
                if (dev == null)
                {
                    Logger.Logger.Debug("设备注册消息->SipDevList中没有找到->进行正常注册->" + ip + "->" + port + "->" + devid);
                    var newSip = new SipDevice();
                    newSip.CRC32 = CRC32Cls.GetCRC32(ip + port + devid).ToString();
                    newSip.DeviceId = devid;
                    newSip.SipPort = port;
                    newSip.IpAddress = ip;
                    newSip.CameraExList.Clear();
                    newSip.AlarmList.Clear();
                    newSip.LastSipRequest = sipRequest;
                    newSip.SipDeviceStatus = SipDeviceStatus.Register;
                    newSip.LastKeepAliveTime = DateTime.Now;
                    newSip.LastUpdateTime = DateTime.Now;
                    SipDeviceList.Add(newSip);
                    Logger.Logger.Info("设备注册->" + ip + "->" + newSip.DeviceId);
                }
                else if (dev != null && dev.SipDeviceStatus == SipDeviceStatus.UnRegister)
                {
                    Logger.Logger.Info("设备注册消息->SipDevList找到->属于未注册状态->进行状态转换->" + ip + "->" + port + "->" + devid);
                    dev.CameraExList.Clear();
                    dev.AlarmList.Clear();
                    dev.LastSipRequest = sipRequest;
                    dev.SipDeviceStatus = SipDeviceStatus.Register;
                    dev.LastUpdateTime = DateTime.Now;
                }
                else //发现公网不固定ip设备，可能因网络波动导致n次注册，而ip地址又不一致，造成straemnode后续处理问题，这边做一次信息修改来解决问题
                {
                    lock (SipDeviceLock)
                    {
                        if (!ip.Equals(dev.IpAddress) || !port.Equals(dev.SipPort))
                        {
                            Logger.Logger.Info("收到一个错误的注册设备,要准备踢掉了->" + dev.DeviceId + "->" + dev.IpAddress + "->" +
                                               dev.SipPort);
                            dev.SipDeviceStatus = SipDeviceStatus.LooksLikeOffline;
                            dev.LastKeepAliveTime = DateTime.Now.AddMinutes(10); //用于将sip设备踢掉，等它重新注册
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 设备注销
        /// </summary>
        /// <param name="sipRequest"></param>
        /// <param name="sIPAccount"></param>
        private void OnSipUnRegisterReceived(SIPRequest sipRequest, SIPAccount sIPAccount)
        {
            Logger.Logger.Debug("设备注销消息->IP->" + sipRequest.Header.Vias.TopViaHeader.Host + "->Port->" +
                                sipRequest.Header.Vias.TopViaHeader.Port + "->DevID->" +
                                sipRequest.Header.From.FromURI.User);
            _sipMessageCore._registrarCore.RemoveDeviceItem(sipRequest); //清理掉sip网关中的残留设备信息
            _sipMessageCore.RemoteTransEPs.Remove(sipRequest.Header.Vias.TopViaHeader.Host + ":" +
                                                  sipRequest.Header.Vias.TopViaHeader.Port);
            lock (SipDeviceLock)
            {
                var dev = SipDeviceList.FindLast(x =>
                    x.IpAddress.Equals(sipRequest.Header.Vias.TopViaHeader.Host)
                    && x.SipPort.Equals(sipRequest.Header.Vias.TopViaHeader.Port)
                    && x.DeviceId.Equals(sipRequest.Header.From.FromURI.User));
                if (dev != null)
                {
                    Logger.Logger.Info("设备注销->" + dev.IpAddress + "->" + dev.DeviceId);
                    dev.SipDeviceStatus = SipDeviceStatus.UnRegister;
                    dev.LastSipRequest = sipRequest;
                    dev.LastUpdateTime = DateTime.Now;
                    dev.CameraExList.Clear();
                }
            }
        }


        /// <summary>
        /// 获取配置文件中的http端口号
        /// </summary>
        /// <param name="httpPort"></param>
        /// <returns></returns>
        private bool getHttpPortFromSystemConfig(out string httpPort)
        {
            var dir = Environment.CurrentDirectory;
            if (File.Exists(dir + "/" + "system.conf"))
            {
                List<string> tmp_sl = File.ReadAllLines(dir + "/" + "system.conf").ToList();
                foreach (var str in tmp_sl)
                {
                    string tmps = str.Trim().ToLower();
                    if (!tmps.EndsWith(";") || tmps.StartsWith("#")) continue;
                    tmps = tmps.TrimEnd(';');
                    if (tmps.Contains("httpport"))
                    {
                        var arr_s = tmps.Split("::", StringSplitOptions.RemoveEmptyEntries);
                        if (arr_s.Length == 2)
                        {
                            ushort pp = 0;
                            var r = ushort.TryParse(arr_s[1], out pp);
                            if (r == true && pp >= ushort.MinValue && pp <= ushort.MaxValue)
                            {
                                httpPort = pp.ToString();
                                return true;
                            }
                        }
                    }
                }
            }

            httpPort = "";
            return false;
        }

        private void OnSipServiceChange(string msg, ServiceStatus state)
        {
            //sip服务状态改变时，待实现
        }

        /// <summary>
        /// 接收设备目录信息
        /// </summary>
        /// <param name="catalog"></param>
        private void OnCatalogReceived(Catalog catalog)
        {
            lock (SipDeviceLock)
            {
                string[] _tmpArr = catalog.RemoteEP.Split(":", StringSplitOptions.RemoveEmptyEntries);
                string ip = "";
                int port = -1;
                if (_tmpArr.Length == 2)
                {
                    ip = _tmpArr[0];
                    port = int.Parse(_tmpArr[1]);
                }

                var dev = SipDeviceList.FindLast(x =>
                    x.IpAddress.Equals(ip)
                    && x.SipPort.Equals(port)
                    && x.DeviceId.Equals(catalog.DeviceID));
                if (dev != null)
                {
                    foreach (var sub in catalog.DeviceList.Items)
                    {
                        if (sub.Status != DevStatus.ON) continue;
                        int extId = int.Parse(sub.DeviceID.Substring(10, 3));
                        if (extId == 131 || extId == 132 || extId == 134 || extId == 137)
                        {
                            var obj = dev.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(sub.DeviceID));
                            if (obj == null)
                            {
                                var camera = new CameraEx();
                                camera.Ctype = CameraType.GB28181;
                                camera.MediaServerId = "";
                                camera.App = "";
                                camera.Vhost = "";
                                camera.StreamId = 0;
                                camera.SipCameraStatus = SipCameraStatus.Idle;
                                camera.StreamServerIp = "";
                                camera.StreamServerPort = 0;
                                camera.Camera = new Camera();
                                camera.Camera.IPAddress = ip;
                                camera.Camera.Port = port;
                                camera.Camera.DeviceID = sub.DeviceID;
                                camera.Camera.Name = sub.Name;
                                camera.Camera.Manufacturer = sub.Manufacturer;
                                camera.Camera.Model = sub.Model;
                                camera.Camera.Owner = sub.Owner;
                                camera.Camera.CivilCode = sub.CivilCode;
                                camera.Camera.Adddress = sub.Address;
                                if (sub.Parental != null) camera.Camera.Parental = (long) sub.Parental;
                                camera.Camera.ParentID = dev.DeviceId;
                                camera.Camera.SafetyWay = sub.SafetyWay;
                                if (sub.RegisterWay != null) camera.Camera.RegisterWay = (long) sub.RegisterWay;
                                if (sub.Secrecy != null) camera.Camera.Secrecy = (long) sub.Secrecy;
                                camera.Camera.Status = sub.Status.ToString();
                                camera.Camera.Longitude = sub.LongitudeValue;
                                camera.Camera.Latitude = sub.LatitudeValue;
                                dev.CameraExList.Add(camera);
                                dev.LastUpdateTime = DateTime.Now;


                                CameraInstanceForSip c = new CameraInstanceForSip();
                                c.Activated = false;
                                c.EnableLive = false;
                                c.EnablePtz = false;
                                c.CameraName = camera.Camera.Name;
                                c.CreateTime = DateTime.Now;
                                c.DeptId = "";
                                c.DeptName = "";
                                c.MobileCamera = false;
                                c.UpdateTime = DateTime.Now;
                                c.CameraId = "unknow_" + DateTime.Now.Ticks;
                                c.CameraType = CameraType.GB28181;
                                c.CameraChannelLable = camera.Camera.DeviceID;
                                c.CameraDeviceLable = dev.DeviceId;
                                c.CameraIpAddress = dev.IpAddress;
                                c.IfGb28181Tcp = false;
                                c.IfRtspUrl = "";
                                c.PDetpId = "";
                                c.PushMediaServerId = "unknow_" + DateTime.Now.Ticks;
                                string reqData = JsonHelper.ToJson(c);
                                try
                                {
                                    string httpport = "";
                                    string url = "";

                                    if (getHttpPortFromSystemConfig(out httpport))
                                    {
                                        url = "http://127.0.0.1:" + httpport + "/WebHook/OnSipDeviceRegister";
                                    }
                                    else
                                    {
                                        url = "http://127.0.0.1:5800/WebHook/OnSipDeviceRegister";
                                    }

                                    var httpRet = NetHelper.HttpPostRequest(url, null!, reqData, "utf-8", 3000);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                }
            }


            var ret = TaskList.FindLast(x => x.DeviceId.Equals(catalog.DeviceID));
            if (ret != null)
            {
                ret.Next();
            }
        }

        private void OnNotifyCatalogReceived(NotifyCatalog notifyCatalog)
        {
            //待实现
        }

        private void OnAlarmReceived(Alarm alarm)
        {
            //待实现
        }

        /// <summary>
        /// 当有录像文件查询回调时触发
        /// </summary>
        /// <param name="recordInfo"></param>
        private void OnRecordInfoReceived(RecordInfo recordInfo)
        {
            //待实现
            var str = "收到GB28181的录像文件目录->\r\n" + JsonHelper.ToJson(recordInfo);
            Logger.Logger.Debug(str);
        }

        private void OnKeepAliveReceived(SIPEndPoint remoteEp, KeepAlive keepAlive, string devId)
        {
            lock (SipDeviceLock)
            {
                var camearDev = _sipDeviceList.FindLast(x => x.DeviceId.Equals(devId));
                if (camearDev != null && camearDev.SipDeviceStatus == SipDeviceStatus.LooksLikeOffline)
                {
                    Logger.Logger.Debug("收到判断为断线Sip设备的心跳数据，不做处理->" + remoteEp.Address + "->" + remoteEp.Port + "->" +
                                        devId);
                    return; //如果是断线状态，就不处理
                }


                var deviceObj = SipMessageCore.NodeMonitorService.FirstOrDefault(x => x.Key.Equals(devId));
                if (!remoteEp.Address.ToString().Equals(deviceObj.Value.RemoteEndPoint.Address.ToString())
                    || !remoteEp.Port.ToString().Equals(deviceObj.Value.RemoteEndPoint.Port.ToString()))
                {
                    deviceObj.Value.RemoteEndPoint = remoteEp;
                }


                if (camearDev != null && camearDev.CameraExList != null)
                {
                    foreach (var cex in camearDev.CameraExList)
                    {
                        if (cex != null && cex.Camera != null && !string.IsNullOrEmpty(cex.Camera.DeviceID))
                        {
                            var obj = SipMessageCore.NodeMonitorService.FirstOrDefault(x =>
                                x.Key.Equals(cex.Camera.DeviceID));

                            if (obj.Value != null)
                            {
                                if (!remoteEp.Address.ToString()
                                        .Equals(obj.Value.RemoteEndPoint.Address.ToString())
                                    || !remoteEp.Port.ToString()
                                        .Equals(obj.Value.RemoteEndPoint.Port.ToString()))
                                {
                                    obj.Value.RemoteEndPoint = remoteEp;
                                }
                            }
                        }
                    }
                }


                var dev = SipDeviceList.FindLast(x =>
                    x.IpAddress.Equals(remoteEp.Address.ToString())
                    && x.SipPort.Equals(remoteEp.Port)
                    && x.DeviceId.Equals(devId));
                if (dev != null)
                {
                    Logger.Logger.Debug("收到Sip设备的心跳数据并更新设备心跳时间->" + remoteEp.Address + "->" + remoteEp.Port + "->" +
                                        devId);
                    dev.LastUpdateTime = DateTime.Now;
                    dev.LastKeepAliveTime = DateTime.Now;
                }
                else
                {
                    Logger.Logger.Debug("收到Sip设备的心跳数据但没有找到相关设备，不做设备心跳时间更新->" + remoteEp.Address + "->" + remoteEp.Port +
                                        "->" + devId);
                }
            }
        }

        private void OnDeviceStatusReceived(SIPEndPoint remoteEp, DeviceStatus deviceStatus)
        {
            //待实现
        }

        private void OnDeviceInfoReceived(SIPEndPoint remoteEp, DeviceInfo deviceInfo)
        {
            //待实现
        }

        private void OnMediaStatusReceived(SIPEndPoint remoteEp, MediaStatus mediaStatus)
        {
            //待实现
        }

        private void OnPresetQueryReceived(SIPEndPoint remoteEp, PresetInfo presetInfo)
        {
            //待实现
        }

        private void OnDeviceConfigDownloadReceived(SIPEndPoint remoteEp, DeviceConfigDownload deviceConfigDownload)
        {
            //待实现
        }

        /// <summary>
        /// 实时视频请求，For ZLMediakit 动态生成端口
        /// </summary>
        /// <param name="devId"></param>
        /// <returns></returns>
        public bool ReqLiveForCreateRptPort(string devId, string rptServerIp, ushort rptPort, bool tcp = false)
        {
            string pid = "";
            string pip = "";
            bool found = false;
            foreach (var device in _sipDeviceList)
            {
                foreach (var ca in device.CameraExList)
                {
                    if (ca.Camera.DeviceID.Equals(devId))
                    {
                        pid = device.DeviceId;
                        pip = device.IpAddress;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }

            if (!string.IsNullOrEmpty(pid))
            {
                SipCoreTask gdlt = new SipCoreTask(pid, devId, this);
                TaskList.Add(gdlt);

                string streamid = pip + pid + devId;
                uint stid = CRC32Cls.GetCRC32(streamid);
                Logger.Logger.Info("资料->" + streamid + " 10进制->" + stid + " 16进制->" + string.Format("{0:X8}", stid));
                var ret = gdlt.Invite(stid, rptServerIp, rptPort, tcp);
                if (ret)
                {
                    var obj = _sipDeviceList.FindLast(x => x.DeviceId.Equals(pid));
                    if (obj != null)
                    {
                        var camera = obj.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(devId));
                        if (camera != null)
                        {
                            camera.App = "rtp";
                            camera.Vhost = "__defaultVhost__";
                            camera.StreamId = stid;
                            camera.SipCameraStatus = SipCameraStatus.RealVideo;
                            camera.StreamServerIp = rptServerIp;
                            camera.StreamServerPort = rptPort;
                            camera.PushStreamSocketType =
                                tcp ? PushStreamSocketType.TCP : PushStreamSocketType.UDP;
                        }
                    }

                    Logger.Logger.Info("请求实时视频成功->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }
                else
                {
                    var obj = _sipDeviceList.FindLast(x => x.DeviceId.Equals(pid));
                    if (obj != null)
                    {
                        var camera = obj.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(devId));
                        if (camera != null)
                        {
                            camera.App = "rtp";
                            camera.Vhost = "__defaultVhost__";
                            camera.StreamId = 0;
                            camera.SipCameraStatus = SipCameraStatus.Idle;
                            camera.StreamServerIp = "";
                            camera.StreamServerPort = 0;
                            camera.PushStreamSocketType = null;
                        }
                    }

                    Logger.Logger.Warn("请求实时视频失败->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }

                TaskList.Remove(gdlt);
                return ret;
            }

            return false;
        }

        public bool ReqLive(string devId)
        {
            string pid = "";
            string pip = "";
            foreach (var device in _sipDeviceList)
            {
                foreach (var ca in device.CameraExList)
                {
                    if (ca.Camera.DeviceID.Equals(devId))
                    {
                        pid = device.DeviceId;
                        pip = device.IpAddress;
                    }
                }
            }

            if (!string.IsNullOrEmpty(pid))
            {
                SipCoreTask gdlt = new SipCoreTask(pid, devId, this);
                TaskList.Add(gdlt);
                string ip = "";
                int port = 0;
                string streamid = pip + pid + devId;
                uint stid = CRC32Cls.GetCRC32(streamid);
                Logger.Logger.Info("资料->" + streamid + " 10进制->" + stid + " 16进制->" + string.Format("{0:X8}", stid));
                var ret = gdlt.Invite(stid, out ip, out port);

                if (ret)
                {
                    var obj = _sipDeviceList.FindLast(x => x.DeviceId.Equals(pid));
                    if (obj != null)
                    {
                        var camera = obj.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(devId));
                        if (camera != null)
                        {
                            camera.App = "rtp";
                            camera.Vhost = "__defaultVhost__";
                            camera.StreamId = stid;
                            camera.SipCameraStatus = SipCameraStatus.RealVideo;
                            camera.StreamServerIp = ip;
                            camera.StreamServerPort = port;
                            camera.PushStreamSocketType =
                                gdlt.SipCoreHelper.SipMessageCore.LocalSipAccount.StreamProtocol == ProtocolType.Tcp
                                    ? PushStreamSocketType.TCP
                                    : PushStreamSocketType.UDP;
                        }
                    }


                    Logger.Logger.Info("请求实时视频成功->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }
                else
                {
                    var obj = _sipDeviceList.FindLast(x => x.DeviceId.Equals(pid));
                    if (obj != null)
                    {
                        var camera = obj.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(devId));
                        if (camera != null)
                        {
                            camera.App = "rtp";
                            camera.Vhost = "__defaultVhost__";
                            camera.StreamId = 0;
                            camera.SipCameraStatus = SipCameraStatus.Idle;
                            camera.StreamServerIp = "";
                            camera.StreamServerPort = 0;
                            camera.PushStreamSocketType = null;
                        }
                    }

                    Logger.Logger.Warn("请求实时视频失败->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }

                TaskList.Remove(gdlt);
                return ret;
            }

            return false;
        }

        public bool ReqStopLive(string devId)
        {
            string pid = "";
            foreach (var device in _sipDeviceList)
            {
                foreach (var ca in device.CameraExList)
                {
                    if (ca.Camera.DeviceID.Equals(devId))
                    {
                        pid = device.DeviceId;
                    }
                }
            }

            if (!string.IsNullOrEmpty(pid))
            {
                SipCoreTask gdlt = new SipCoreTask(pid, devId, this);
                TaskList.Add(gdlt);
                var ret = gdlt.ByeLive();
                if (ret)
                {
                    var obj = _sipDeviceList.FindLast(x => x.DeviceId.Equals(pid));
                    if (obj != null)
                    {
                        var camera = obj.CameraExList.FindLast(x => x.Camera.DeviceID.Equals(devId));
                        if (camera != null)
                        {
                            camera.App = "rtp";
                            camera.Vhost = "__defaultVhost__";
                            camera.StreamId = null!;
                            camera.SipCameraStatus = SipCameraStatus.Idle;
                        }
                    }

                    Logger.Logger.Info("请求实时视频成功->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }
                else
                {
                    Logger.Logger.Warn("请求终止实时视频失败->" + gdlt.DeviceId + "->" + gdlt.CallId);
                }

                TaskList.Remove(gdlt);
                return ret;
            }

            return false;
        }

        public bool ReqPtzControl(string devId, string dir, int speed, string channelId = "")
        {
            var dev = _sipDeviceList.FindLast(x => x.DeviceId.Equals(devId));
            if (dev != null)
            {
                SipCoreTask gdlt = new SipCoreTask("", devId, this);
                TaskList.Add(gdlt);
                var ret = gdlt.PtzContorl(dir, speed, channelId);
                if (ret)
                {
                    Logger.Logger.Info("请求PTZ控制成功->" + gdlt.DeviceId + "->" + gdlt.CallId + "->" + dir + "->" + speed);
                }
                else
                {
                    Logger.Logger.Warn("请求PTZ控制失败->" + gdlt.DeviceId + "->" + gdlt.CallId + "->" + dir + "->" + speed);
                }

                TaskList.Remove(gdlt);
                return ret;
            }

            return false;
        }

        public bool GetDeviceList(string devId)
        {
            SipCoreTask gdlt = new SipCoreTask("", devId, this);
            TaskList.Add(gdlt);
            var ret = gdlt.GetDeviceList();
            if (ret)
            {
                Logger.Logger.Info("获取设备目录成功->" + gdlt.DeviceId + "->" + gdlt.CallId);
            }
            else
            {
                Logger.Logger.Warn("获取设备目录失败->" + gdlt.DeviceId + "->" + gdlt.CallId);
            }

            TaskList.Remove(gdlt);
            return ret;
        }

        /// <summary>
        /// 获取gb28181的录像文件
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type">录像产生类型 time alarm manual all</param>
        /// <returns></returns>
        public int GetRecordFile(string deviceId, DateTime startTime, DateTime endTime, string type)
        {
            SipCoreTask gdlt = new SipCoreTask("", deviceId, this);
            TaskList.Add(gdlt);
            var ret = gdlt.RecordFileQuery(startTime, endTime, type);
            if (ret > -1)
            {
                Logger.Logger.Info("获取录像目录成功->录像数量->(" + ret + ")个->" + gdlt.DeviceId + "->" + gdlt.CallId);
            }
            else
            {
                Logger.Logger.Warn("获取录像目录失败->" + gdlt.DeviceId + "->" + gdlt.CallId);
            }

            TaskList.Remove(gdlt);
            return ret;
        }

        public void GetDeviceStatus(string devId)
        {
            //待实现
        }

        private void OnResponseCodeReceived(SIPResponse response, string message, SIPEndPoint remoteEp)
        {
            //待实现
            //  Console.Write("Response:" + response.ToString() + " Message:" + message + " Remote:" + remoteEp.Address);
        }

        private void OnResponseNeedResponeReceived(SIPResponse response, SIPRequest request, string message,
            SIPEndPoint remoteEp)
        {
            /*Console.Write("Response:" + response.ToString() + " Request:" + request.ToString() + "Message:" + message +
                          " Remote:" + remoteEp.Address);*/
            var ret = TaskList.FindLast(x => x.CallId.Equals(request.Header.CallId));
            if (ret != null)
            {
                ret.Next();
            }
        }

        public bool Stop()
        {
            _isRunning = false;
            _sipMessageCore.OnServiceChanged -= OnSipServiceChange;
            _sipMessageCore.OnCatalogReceived -= OnCatalogReceived;
            _sipMessageCore.OnNotifyCatalogReceived -= OnNotifyCatalogReceived;
            _sipMessageCore.OnAlarmReceived -= OnAlarmReceived;
            _sipMessageCore.OnRecordInfoReceived -= OnRecordInfoReceived;
            _sipMessageCore.OnKeepaliveReceived -= OnKeepAliveReceived;
            _sipMessageCore.OnDeviceStatusReceived -= OnDeviceStatusReceived;
            _sipMessageCore.OnDeviceInfoReceived -= OnDeviceInfoReceived;
            _sipMessageCore.OnMediaStatusReceived -= OnMediaStatusReceived;
            _sipMessageCore.OnPresetQueryReceived -= OnPresetQueryReceived;
            _sipMessageCore.OnDeviceConfigDownloadReceived -= OnDeviceConfigDownloadReceived;
            _sipMessageCore.OnResponseCodeReceived -= OnResponseCodeReceived;
            _sipMessageCore.OnResponseNeedResponeReceived -= OnResponseNeedResponeReceived;
            _sipMessageCore.OnRegisterReceived -= OnSipRegisterReceived;
            _sipMessageCore.OnUnRegisterReceived -= OnSipUnRegisterReceived;
            _sipMessageCore.OnDeviceAlarmSubscribe -= OnDeviceAlarmSubscribeReceived;
            _sipMessageCore.Stop();
            _sipMessageCore._registryServiceToken.Cancel();
            _sipMessageCore._registrarCore = null;
            _sipMessageCore = null;
            return true;
        }

        public bool Start()
        {
            _sipDeviceList.Clear();
            SIPTransport m_sipTransport;
            m_sipTransport = new SIPTransport(SIPDNSManager.ResolveSIPService, new SIPTransactionEngine(), false);
            m_sipTransport.PerformanceMonitorPrefix = SIPSorceryPerformanceMonitor.REGISTRAR_PREFIX;
            SipStorage sipAccountStorage = new SipStorage();
            IMemoCache<Camera> memoCache = new DeviceObjectCache();
            _sipMessageCore = new SIPMessageCore(m_sipTransport, sipAccountStorage, memoCache);
            _sipMessageCore.OnServiceChanged += OnSipServiceChange;
            _sipMessageCore.OnCatalogReceived += OnCatalogReceived;
            _sipMessageCore.OnNotifyCatalogReceived += OnNotifyCatalogReceived;
            _sipMessageCore.OnAlarmReceived += OnAlarmReceived;
            _sipMessageCore.OnRecordInfoReceived += OnRecordInfoReceived;
            _sipMessageCore.OnKeepaliveReceived += OnKeepAliveReceived;
            _sipMessageCore.OnDeviceStatusReceived += OnDeviceStatusReceived;
            _sipMessageCore.OnDeviceInfoReceived += OnDeviceInfoReceived;
            _sipMessageCore.OnMediaStatusReceived += OnMediaStatusReceived;
            _sipMessageCore.OnPresetQueryReceived += OnPresetQueryReceived;
            _sipMessageCore.OnDeviceConfigDownloadReceived += OnDeviceConfigDownloadReceived;
            _sipMessageCore.OnResponseCodeReceived += OnResponseCodeReceived;
            _sipMessageCore.OnResponseNeedResponeReceived += OnResponseNeedResponeReceived;
            _sipMessageCore.OnRegisterReceived += OnSipRegisterReceived;
            _sipMessageCore.OnUnRegisterReceived += OnSipUnRegisterReceived;
            _sipMessageCore.OnDeviceAlarmSubscribe += OnDeviceAlarmSubscribeReceived;
            _sipMessageCore.Start();
            _isRunning = true;
            Task.Factory.StartNew(() => AutoTask());
            return true;
        }
    }
}