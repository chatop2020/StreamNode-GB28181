using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResToWebHookOnPublish : ResZLMediaKitResponseBase
    {
        private bool? _enableHls;
        private bool? _enableMP4;
       // private bool? _enableRtxp;
        private string? _msg;

        public bool? EnableHls
        {
            get => _enableHls;
            set => _enableHls = value;
        }

        public bool? EnableMp4
        {
            get => _enableMP4;
            set => _enableMP4 = value;
        }

        /*public bool? EnableRtxp
        {
            get => _enableRtxp;
            set => _enableRtxp = value;
        }*/

        public string? Msg
        {
            get => _msg;
            set => _msg = value;
        }
    }
}