using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitIsRecord : ResZLMediaKitResponseBase
    {
        private bool? _status;

        public bool? Status
        {
            get => _status;
            set => _status = value;
        }
    }
}