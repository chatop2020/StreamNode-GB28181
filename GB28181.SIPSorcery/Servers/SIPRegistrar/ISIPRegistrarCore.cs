using SIPSorcery.SIP;

namespace GB28181.Servers
{
    public interface ISIPRegistrarCore
    {
        bool IsNeedAuthentication { get; }
        void ProcessRegisterRequest();

        void AddRegisterRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint, SIPRequest registerRequest);

        /// <summary>
        /// 设备注册到DMS
        /// </summary>
        event RegisterDelegate RegisterReceived;

        event UnRegisterDelegate UnRegisterReceived;

        event DeviceAlarmSubscribeDelegate DeviceAlarmSubscribe;

        public void RemoveDeviceItem(SIPRequest sipRequest);
        public void RemoveDeviceItem(string deviceKey);

        public void CacheDeviceItem(SIPRequest sipRequest);

    }
}