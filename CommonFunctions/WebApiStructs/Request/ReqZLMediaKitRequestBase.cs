using System;
using System.Text.Json.Serialization;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqZLMediaKitRequestBase
    {
        private string? _secret;

        [JsonIgnore]
        public string? Secret
        {
            get => _secret;
            set => _secret = value;
        }
    }
}