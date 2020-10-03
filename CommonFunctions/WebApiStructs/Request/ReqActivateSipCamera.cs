namespace CommonFunctions.WebApiStructs.Request
{
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

        public string CameraChannelLable
        {
            get => _cameraChannelLable;
            set => _cameraChannelLable = value;
        }

        public string CameraDeviceLable
        {
            get => _cameraDeviceLable;
            set => _cameraDeviceLable = value;
        }

        public string PushMediaServerId
        {
            get => _pushMediaServerId;
            set => _pushMediaServerId = value;
        }

        public string? CameraName
        {
            get => _cameraName;
            set => _cameraName = value;
        }

        public bool? MobileCamera
        {
            get => _mobileCamera;
            set => _mobileCamera = value;
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

        public string? PDetpId
        {
            get => _pDetpId;
            set => _pDetpId = value;
        }

        public string? PDetpName
        {
            get => _pDetpName;
            set => _pDetpName = value;
        }

        public string CameraIpAddress
        {
            get => _cameraIpAddress;
            set => _cameraIpAddress = value;
        }

        public bool? IfGb28181Tcp
        {
            get => _ifGB28181Tcp;
            set => _ifGB28181Tcp = value;
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
    }
    
}