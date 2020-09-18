using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class ResZLMediaKitResponseBase
    {
        private int _code;

        public int Code
        {
            get => _code;
            set => _code = value;
        }
    }
}