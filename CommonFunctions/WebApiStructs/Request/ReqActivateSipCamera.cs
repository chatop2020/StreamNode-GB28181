using System;

namespace CommonFunctions.WebApiStructs.Request
{
    /// <summary>
    /// 请求结构-自动加入未激活GB28181设备
    /// </summary>
    [Serializable]
    public class ReqActivateSipCamera
    {
        private string _cameraChannelLable; //如果是28181设备，就是通道id
        private string _cameraDeviceLable; //如果是28181设备，就是设备id
        private string _pushMediaServerId; //推去哪个流媒体的id
        private string? _cameraName;
        private bool? _mobileCamera;
        private string? _deptId;
        private string? _deptName;
        private string? _pDetpId;
        private string? _pDetpName;
        private string _cameraIpAddress;
        private bool? _ifGB28181Tcp = false; //如果是gb28181是否采用tcp方式进行推流
        private bool? _enableLive;
        private bool? _enablePtz;
        private bool _activated=true; //默认为激活状态

        /// <summary>
        /// 摄像头音视频流的GB28181 ID
        /// </summary>
        public string CameraChannelLable
        {
            get => _cameraChannelLable;
            set => _cameraChannelLable = value;
        }

        /// <summary>
        /// GB28181设备的设备ID
        /// </summary>
        public string CameraDeviceLable
        {
            get => _cameraDeviceLable;
            set => _cameraDeviceLable = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        /// <summary>
        /// 摄像头实例名称
        /// </summary>
        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        /// <summary>
        /// 是否为移动GB28181设备（非固定ip设备）
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
        /// 摄像头设备IP地址（非固定ip可随意填写一个ip地址，与MobileCamera配合使用）
        /// </summary>
        public string CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

        /// <summary>
        /// 使用TCP方式来接受GB28181设备的音频视频（告知摄像头用TCP推流）
        /// </summary>
        public bool? IfGb28181Tcp
        {
            get => _ifGB28181Tcp;
            set => _ifGB28181Tcp = value;
        }

        /// <summary>
        /// 是否自动推流（设备上线自动推流）
        /// </summary>
        public bool? EnableLive
        {
            get => _enableLive;
            set => _enableLive = value;
        }

        /// <summary>
        /// 是否启用PTZ控制（仅支持GB28181设备）
        /// </summary>
        public bool? EnablePtz
        {
            get => _enablePtz;
            set => _enablePtz = value;
        }

        public bool Activated
        {
            get => _activated;
            set => _activated = value;
        }
    }
}