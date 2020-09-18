using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitCloseRtpPort : ResZLMediaKitResponseBase
    {
        private int? _hit;

        public int? Hit
        {
            get => _hit;
            set => _hit = value;
        }
    }
}