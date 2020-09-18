using System;

namespace CommonFunctions.WebApiStructs.Request
{
    [Serializable]
    public class ReqForWebHookOnRecordMp4Completed
    {
        private string? _app;
        private string? _file_Name;
        private string? _file_Path;
        private string? _folder;
        private long? _file_Size;
        private long? _start_Time;
        private string? _stream;
        private string? _mediaserverid;
        private string? _url;
        private string? _vhost;
        private int? _time_len;
        
        /*
         * "app" : "rtp",
   "file_name" : "12-50-56.mp4",
   "file_path" : "/root/MediaService/www/record/rtp/20D4D5C0/2020-08-24/12-50-56.mp4",
   "file_size" : 7507638,
   "folder" : "/root/MediaService/www/record/rtp/20D4D5C0/",
   "mediaserverid" : "NF308M6K-129980D0-8AB1LE22-45K9J04C-6J00FB1D-718LUIA2",
   "start_time" : 1598244656,
   "stream" : "20D4D5C0",
   "time_len" : 30,
   "url" : "record/rtp/20D4D5C0/2020-08-24/12-50-56.mp4",
   "vhost" : "__defaultVhost__"

         */

        public string? App
        {
            get => _app;
            set => _app = value;
        }

        public string? File_Name
        {
            get => _file_Name;
            set => _file_Name = value;
        }

        public string? File_Path
        {
            get => _file_Path;
            set => _file_Path = value;
        }

        public string? Folder
        {
            get => _folder;
            set => _folder = value;
        }

        public long? File_Size
        {
            get => _file_Size;
            set => _file_Size = value;
        }

        public long? Start_Time
        {
            get => _start_Time;
            set => _start_Time = value;
        }

        public string? Stream
        {
            get => _stream;
            set => _stream = value;
        }

      

        public string? Mediaserverid
        {
            get => _mediaserverid;
            set => _mediaserverid = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public string? Vhost
        {
            get => _vhost;
            set => _vhost = value;
        }

        public int? Time_Len
        {
            get => _time_len;
            set => _time_len = value;
        }
    }
}