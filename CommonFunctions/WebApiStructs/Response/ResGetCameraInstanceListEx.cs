using System;
using System.Collections.Generic;
using CommonFunctions.DBStructs;
using CommonFunctions.WebApiStructs.Request;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResGetCameraInstanceListEx
    {
        private List<CameraInstance>? _cameraInstanceList;
        private ReqGetCameraInstanceListEx? _request;
        private long? _total;

        public List<CameraInstance>? CameraInstanceList
        {
            get => _cameraInstanceList;
            set => _cameraInstanceList = value;
        }

        public ReqGetCameraInstanceListEx? Request
        {
            get => _request;
            set => _request = value;
        }

        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}