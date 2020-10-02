using System;
using System.Collections.Generic;
using CommonFunctions.ManageStructs;
using FreeSql.DataAnnotations;
using GB28181.Sys.Model;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqGetCameraInstanceListEx
    {
        private string? _mediaServerId;
        private string? _cameraName;
        private string? _cameraDeptId;
        private CameraType? _cameraType;
        private bool? _enableLive;
        private string? _cameraId;
        private int? _pageIndex;
        private int? _pageSize;
        private List<OrderByStruct>? _orderBy;

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        public string? CameraDeptId
        {
            get => _cameraDeptId;
            set => _cameraDeptId = value;
        }

        [Column(MapType = typeof(string))]
        public CameraType? CameraType
        {
            get => _cameraType;
            set => _cameraType = value;
        }

        public bool? EnableLive
        {
            get => _enableLive;
            set => _enableLive = value;
        }

        public string? CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }

        public int? PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        public int? PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        public List<OrderByStruct>? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }
    }
}