using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResToWebHookOnStreamNoneReader : ResZLMediaKitResponseBase
    {
        private bool? close;

        public bool? Close
        {
            get => close;
            set => close = value;
        }
    }
}