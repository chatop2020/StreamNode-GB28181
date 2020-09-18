using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CommonFunctions.WebApiStructs.Response
{
    [Serializable]
    public class TracksItem
    {
        private int? _channels;
        private int? _codec_id;
        private string? _codec_id_name;
        private int? _codec_type;
        private string? _ready;
        private int? _sample_bit;
        private int? _sample_rate;
        private int? _fps;
        private int? _width;
        private int? _height;

        [JsonProperty("channels")]
        public int? Channels
        {
            get => _channels;
            set => _channels = value;
        }

        [JsonProperty("codec_id")]
        public int? Codec_Id
        {
            get => _codec_id;
            set => _codec_id = value;
        }

        [JsonProperty("codec_id_name")]
        public string? Codec_Id_Name
        {
            get => _codec_id_name;
            set => _codec_id_name = value;
        }

        [JsonProperty("codec_type")]
        public int? Codec_Type
        {
            get => _codec_type;
            set => _codec_type = value;
        }

        [JsonProperty("ready")]
        public string? Ready
        {
            get => _ready;
            set => _ready = value;
        }

        [JsonProperty("sample_bit")]
        public int? Sample_Bit
        {
            get => _sample_bit;
            set => _sample_bit = value;
        }

        [JsonProperty("sample_rate")]
        public int? Sample_Rate
        {
            get => _sample_rate;
            set => _sample_rate = value;
        }

        public int? Fps
        {
            get => _fps;
            set => _fps = value;
        }

        public int? Width
        {
            get => _width;
            set => _width = value;
        }

        public int? Height
        {
            get => _height;
            set => _height = value;
        }
    }

    [Serializable]
    public class MediaDataItem
    {
        private string? _app;
        private int? _readerCount;
        private string? _schema;
        private string? _stream;
        private int? _totalReaderCount;
        private string? _vhost;
        private List<TracksItem> _tracks;

        [JsonProperty("app")]
        public string? App
        {
            get => _app;
            set => _app = value;
        }

        [JsonProperty("readerCount")]
        public int? ReaderCount
        {
            get => _readerCount;
            set => _readerCount = value;
        }

        public string? Schema
        {
            get => _schema;
            set => _schema = value;
        }

        [JsonProperty("stream")]
        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

        [JsonProperty("totalReaderCount")]
        public int? TotalReaderCount
        {
            get => _totalReaderCount;
            set => _totalReaderCount = value;
        }

        [JsonProperty("vhost")]
        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        [JsonProperty("tracks")]
        public List<TracksItem> Tracks
        {
            get => _tracks;
            set => _tracks = value;
        }
    }

    [Serializable]
    public class ResZLMediaKitMediaList : ResZLMediaKitResponseBase
    {
        private List<MediaDataItem> _data;

        [JsonProperty("data")]
        public List<MediaDataItem> Data
        {
            get => _data;
            set => _data = value;
        }
    }
}