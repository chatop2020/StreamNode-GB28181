using System;
using System.Collections.Generic;
using CommonFunctions.ManageStructs;
using FreeSql.DataAnnotations;
using GB28181.Sys.Model;

namespace CommonFunctions.WebApiStructs.Request
{
    /// <summary>
    /// 请求结构-摄像头实例列表（扩展）
    /// </summary>
    [Serializable]
    public class ReqGetCameraInstanceListEx
    {
        private string? _mediaServerId;
        private string? _cameraName;
        private string? _cameraDeptId;
        private CameraType? _cameraType;
        private bool? _enableLive;
        private string? _cameraId;
        private bool? _activated;
        private int? _pageIndex;
        private int? _pageSize;
        private List<OrderByStruct>? _orderBy;

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
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
        /// 摄像头部门代码
        /// </summary>
        public string? CameraDeptId
        {
            get => _cameraDeptId;
            set => _cameraDeptId = value;
        }

        /// <summary>
        /// 摄像头类型
        /// GB28181=GB28181设备
        /// Rtsp=RTSP设备
        /// LiveCast=直播设备
        /// None=未知设备
        /// </summary>

        [Column(MapType = typeof(string))]
        public CameraType? CameraType
        {
            get => _cameraType;
            set => _cameraType = value;
        }

        /// <summary>
        /// 是否启用自动推流
        /// </summary>
        public bool? EnableLive
        {
            get => _enableLive;
            set => _enableLive = value;
        }

        /// <summary>
        /// 摄像头实例ID
        /// </summary>
        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        /// <summary>
        /// 摄像头是否已激活（false为没有激活）
        /// </summary>
        public bool? Activated
        {
            get => _activated;
            set => _activated = value;
        }

        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int? PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        /// <summary>
        /// 每页多少条记录
        /// </summary>
        public int? PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        /// <summary>
        /// 排序结构
        /// </summary>
        public List<OrderByStruct>? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }
    }
}