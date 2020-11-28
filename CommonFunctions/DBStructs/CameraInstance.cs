using System;
using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;
using GB28181.Sys.Model;

namespace CommonFunctions.DBStructs
{
    /// <summary>
    /// 音视频设备的注册实例结构，与数据库中Camera表一致是Camera表的字段映射
    /// </summary>
    [Table(Name = "Camera")]
    [Index("cmr_cameraid", "CameraId", true)]
    [Index("cmr_cameraname", "CameraName", false)]
    [Index("cmr_dept", "DeptId,PDetpId", false)]
    [Index("cmr_deptex", "DeptName", false)]
    [Index("cmr_enablelive", "EnableKive", false)]
    [Serializable]
    public class CameraInstance
    {
        private long _id;
        private string _cameraId;
        private string _cameraName;
        private bool? _mobileCamera;
        private string? _deptId;
        private string? _deptName;
        private string? _pDetpId;
        private string? _pDetpName;
        private string _cameraIpAddress;
        private string? _cameraChannelLable; //如果是28181设备，就是通道id
        private string? _cameraDeviceLable; //如果是28181设备，就是设备id
        private CameraType _cameraType;
        private string? _ifRtspUrl; //如果是rtsp,那么rtsp的地址
        private bool? _ifGB28181Tcp = false; //如果是gb28181是否采用tcp方式进行推流
        private string _pushMediaServerId; //推去哪个流媒体的id
        private bool _enableLive;
        private bool? _enablePtz;
        private DateTime? _createTime;
        private DateTime? _updateTime;
        private int? _retryTimes;
        private bool? _activated; //当有sip设备注册时，自动进入到数据库，但activated为false


        /// <summary>
        /// 数据库中的主键id
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id
        {
            get => _id;
            set => _id = value;
        }


        /// <summary>
        /// 摄像头实例ID(自动生成，全局唯一，添加摄像头实例时写null或空字符串)
        /// </summary>
        public string CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        /// <summary>
        /// 摄像头名称，添加摄像头实例或者修改摄像头实例时可修改
        /// </summary>
        public string CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        /// <summary>
        /// 是否为移动摄像头，如果是移动摄像头，将不再判定它的ip地址是否一致
        /// </summary>
        public bool? MobileCamera
        {
            get => _mobileCamera;
            set => _mobileCamera = value;
        }

        /// <summary>
        /// 部门代码
        /// </summary>
        public string? DeptId
        {
            get => _deptId;
            set => _deptId = value;
        }

        /// <summary>
        /// 部门名称
        /// </summary>

        public string? DeptName
        {
            get => _deptName;
            set => _deptName = value;
        }

        /// <summary>
        /// 父部门代码
        /// </summary>

        public string? PDetpId
        {
            get => _pDetpId;
            set => _pDetpId = value;
        }

        /// <summary>
        /// 父部门名称
        /// </summary>
        public string? PDetpName
        {
            get => _pDetpName;
            set => _pDetpName = value;
        }

        /// <summary>
        /// 摄像头ip地址
        /// </summary>
        public string CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

        /// <summary>
        /// GB28181设备的音视频通道ID
        /// </summary>
        public string? CameraChannelLable //如果是28181设备，就是通道id
        {
            get => _cameraChannelLable;
            set => _cameraChannelLable = value;
        }

        /// <summary>
        /// GB28181设备的设备ID
        /// </summary>
        public string? CameraDeviceLable //如果是28181设备，就是设备id
        {
            get => _cameraDeviceLable;
            set => _cameraDeviceLable = value;
        }


        /// <summary>
        /// 摄像头类型
        /// GB28181=GB28181设备
        /// Rtsp=RTSP设备
        /// LiveCast=直播设备
        /// None=未知设备
        /// </summary>
        [Column(MapType = typeof(string))]
        public CameraType CameraType
        {
            get => _cameraType;
            set => _cameraType = value;
        }

        /// <summary>
        /// 摄像头实例的RTSP地址，如果是RTSP设备，必须填写此地址
        /// </summary>
        public string? IfRtspUrl
        {
            get => _ifRtspUrl;
            set => _ifRtspUrl = value;
        }

        /// <summary>
        /// 如果是GB28181设备，此项为True时，采用TCP的方式推流，否则采用UDP方式。
        /// 此项参数与摄备是否支持TCP推流有关，如果不支持TCP推流则使此项参数为False。
        /// </summary>
        public bool? IfGb28181Tcp
        {
            get => _ifGB28181Tcp;
            set => _ifGB28181Tcp = value;
        }

        /// <summary>
        /// 流媒体服务器ID，指定将音视频流推向哪个流媒体节点
        /// </summary>
        public string PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        /// <summary>
        /// 是否启用自动推流，系统自动发生摄像头是否在线，如果在线且EnableLive为true,系统将自动尝试推流
        /// </summary>

        public bool EnableLive
        {
            get => _enableLive;
            set => _enableLive = value;
        }


        /// <summary>
        /// 是否允许PTZ控制（仅支持GB28181设备）
        /// </summary>
        public bool? EnablePtz
        {
            get => _enablePtz;
            set => _enablePtz = value;
        }

        /// <summary>
        /// 摄像头实例的创建时间（自动填写，添加时写null即可）
        /// </summary>
        public DateTime? CreateTime
        {
            get => _createTime;
            set => _createTime = value;
        }

        /// <summary>
        /// 摄像头实例的更新时间（自动填写，添加时写null即可）
        /// </summary>
        public DateTime? UpdateTime
        {
            get => _updateTime;
            set => _updateTime = value;
        }

        /// <summary>
        /// GB28181设备上线时，如果数据库中不存在此设备及通道信息，系统会自动将设备及通道信息写入数据库
        /// 同时将Activated设备为false,需要通过接口激活这类设备
        /// </summary>
        public bool? Activated
        {
            get => _activated;
            set => _activated = value;
        }


        [JsonIgnore]
        [Column(IsIgnore = true)]
        public int? RetryTimes
        {
            get => _retryTimes;
            set => _retryTimes = value;
        }
    }
}