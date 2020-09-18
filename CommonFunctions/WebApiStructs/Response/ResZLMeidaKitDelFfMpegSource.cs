using System;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class Data_1
    {
        private string? _flag;

        public string? Flag
        {
            get => _flag;
            set => _flag = value;
        }
    }

    public class ResZLMeidaKitDelFfMpegSource : ResZLMediaKitResponseBase
    {
        private Data_1 _data;

        public Data_1 Data
        {
            get => _data;
            set => _data = value;
        }
    }
}