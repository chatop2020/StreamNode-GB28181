using System;
using CommonFunction.ManageStructs;
using FreeSql.DataAnnotations;
using GB28181.Sys.Model;

namespace CommonFunctions.DBStructs
{
    [Serializable]
    public enum OnOff
    {
        On,
        Off,
    }

    [Serializable]
    [Table(Name = "ClientLogOnOff")]
    [Index("clof_ipaddress", "IpAddress", false)]
    [Index("clof_clienttype", "ClientType", false)]
    [Index("clof_onoff", "OnOff", false)]
    [Index("clof_cameraid", "cameraid", false)]
    public class ClientOnOffLog
    {
        private long _id;
        private string? _ipaddress;
        private string? _cameraId;
        private DateTime? _createTime;
        private ClientType? _clientType;
        private CameraType? _cameraProtocolType;
        private string? _vhost;
        private string? _app;
        private string? _streamId;
        private string? _pushMediaServerId;
        private OnOff _onOff;

        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }

        public string? Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value;
        }


        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        public DateTime? CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }


        public ClientType? ClientType
        {
            get => _clientType;
            set => _clientType = value;
        }

        public CameraType? CameraProtocolType
        {
            get => _cameraProtocolType;
            set => _cameraProtocolType = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }


        public string? PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        public OnOff OnOff
        {
            get => _onOff;
            set => _onOff = value;
        }
    }
}