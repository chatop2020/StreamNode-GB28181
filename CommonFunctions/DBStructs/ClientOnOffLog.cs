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

    /// <summary>
    /// 设备上线下线的记录，实际作用有点鸡肋，打算将来移除掉
    /// </summary>
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
        private string? _schema;
        private string? _pushMediaServerId;
        private OnOff _onOff;

        /// <summary>
        /// 数据库主键
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }

        /// <summary>
        /// ip地址
        /// </summary>
        public string? Ipaddress
        {
            get => _ipaddress;
            set => _ipaddress = value;
        }


        /// <summary>
        /// 摄像头实例ID（全局唯一）
        /// </summary>
        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        /// <summary>
        /// 记录生成时间
        /// </summary>
        public DateTime? CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }


        /// <summary>
        /// 客户端类型
        ///  Camera =摄像头
        ///  Player =播放者
        /// Livecast=直播者
        /// </summary>
        public ClientType? ClientType
        {
            get => _clientType;
            set => _clientType = value;
        }

        /// <summary>
        /// 摄像头类型
        /// GB28181=GB28181设备
        /// Rtsp=RTSP设备
        /// LiveCast=直播设备
        /// None=未知设备
        /// </summary>
        public CameraType? CameraProtocolType
        {
            get => _cameraProtocolType;
            set => _cameraProtocolType = value;
        }

        /// <summary>
        /// Vhost标记
        /// </summary>
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        /// <summary>
        /// APP标记
        /// </summary>
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        /// <summary>
        /// StreamId标记，流媒体对某个音视频流的标记，与Vhost和App结合后可以唯一标记在同一个流媒体服务器里的某一个音视频流
        /// </summary>
        public string? StreamId
        {
            get => _streamId;
            set => _streamId = value;
        }

        /// <summary>
        /// 段标记
        /// </summary>
        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        /// <summary>
        /// 上线或下线事件
        /// </summary>
        public OnOff OnOff
        {
            get => _onOff;
            set => _onOff = value;
        }
    }
}