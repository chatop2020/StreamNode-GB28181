namespace WinNetworkStaCli
{
    public class NetworkAdapter
    {
        // private string _adapterName;
        // private long _mtu;
        private long _send;

        private long _recv;

        // private string _desc;
        private string _mac;
        private string _ipAddress;

        // public string AdapterName { get => _adapterName; set => _adapterName = value; }
        //  public long Mtu { get => _mtu; set => _mtu = value; }
        public long Send
        {
            get => _send;
            set => _send = value;
        }

        public long Recv
        {
            get => _recv;
            set => _recv = value;
        }

        //  public string Desc { get => _desc; set => _desc = value; }
        public string MAC
        {
            get => _mac;
            set => _mac = value;
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }
    }
}