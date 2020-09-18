using System;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqZLMediaKitDelFFmpegSource : ReqZLMediaKitRequestBase
    {
        private string? _key;

        public string? Key
        {
            get => _key;
            set => _key = value;
        }
    }
}