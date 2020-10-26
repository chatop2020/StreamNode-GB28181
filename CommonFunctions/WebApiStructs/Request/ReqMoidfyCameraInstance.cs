namespace CommonFunctions.WebApiStructs.Request
{
    /// <summary>
    /// 请求结构-修改摄像头实例
    /// </summary>
    public class ReqMoidfyCameraInstance
    {
        private string _cameraId;
        private string? _cameraName;
        private string? _deptId;
        private string? _deptName;
        private string? _pDeptId;
        private string? _pDeptName;
        private bool? _enableLive;
        private bool? _enablePtz;
        private bool? _enableRecord;
        private bool? _ifGB28181Tcp;
        private string? _pushMediaServerId;
        private bool? _mobileCamera;

        /// <summary>
        /// 摄像头实例ID（不可修改，是条件）
        /// </summary>
        public string CameraId
        {
            get => _cameraId;
            set => _cameraId = value;
        }


        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        public string? DeptId
        {
            get => _deptId;
            set => _deptId = value;
        }

        public string? DeptName
        {
            get => _deptName;
            set => _deptName = value;
        }

        public string? PDeptId
        {
            get => _pDeptId;
            set => _pDeptId = value;
        }

        public string? PDeptName
        {
            get => _pDeptName;
            set => _pDeptName = value;
        }


        public bool? EnableLive
        {
            get => _enableLive;
            set => _enableLive = value;
        }

        public bool? EnablePtz
        {
            get => _enablePtz;
            set => _enablePtz = value;
        }

        public bool? EnableRecord
        {
            get => _enableRecord;
            set => _enableRecord = value;
        }

        public bool? IfGb28181Tcp
        {
            get => _ifGB28181Tcp;
            set => _ifGB28181Tcp = value;
        }

        public string? PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        public bool? MobileCamera
        {
            get => _mobileCamera;
            set => _mobileCamera = value;
        }
    }
}