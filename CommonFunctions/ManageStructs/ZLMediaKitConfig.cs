using System;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class ZLMediaKitConfig_API
    {
        private bool? _apiDebug;
        private string? _defaultSnap;
        private string? _secret;
        private string? _snapRoot;

        public bool? ApiDebug
        {
            get => _apiDebug;
            set => _apiDebug = value;
        }

        public string? DefaultSnap
        {
            get => _defaultSnap;
            set => _defaultSnap = value;
        }

        public string? Secret
        {
            get => _secret;
            set => _secret = value;
        }

        public string? SnapRoot
        {
            get => _snapRoot;
            set => _snapRoot = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_FFMpeg
    {
        private string? _bin;
        private string? _cmd;
        private string? _log;
        private string? _snap;

        public string? Bin
        {
            get => _bin;
            set => _bin = value;
        }

        public string? Cmd
        {
            get => _cmd;
            set => _cmd = value;
        }

        public string? Log
        {
            get => _log;
            set => _log = value;
        }

        public string? Snap
        {
            get => _snap;
            set => _snap = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_General
    {
        private bool? _addMuteAudio;
        private bool? _enableVhost;
        private int? _flowThreshold;
        private int? _maxStreamWaitMS;
        private string? _mediaServerId;
        private int? _mergeWriteMS;
        private bool? _modifyStamp;
        private bool? _publishToHls;
        private bool? _publishToMP4;
        private bool? _publishToRtxp;
        private bool? _resetWhenRePlay;
        private int? _streamNoneReaderDelayMS;

        public bool? AddMuteAudio
        {
            get => _addMuteAudio;
            set => _addMuteAudio = value;
        }

        public bool? EnableVhost
        {
            get => _enableVhost;
            set => _enableVhost = value;
        }

        public int? FlowThreshold
        {
            get => _flowThreshold;
            set => _flowThreshold = value;
        }

        public int? MaxStreamWaitMs
        {
            get => _maxStreamWaitMS;
            set => _maxStreamWaitMS = value;
        }

        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        public int? MergeWriteMs
        {
            get => _mergeWriteMS;
            set => _mergeWriteMS = value;
        }

        public bool? ModifyStamp
        {
            get => _modifyStamp;
            set => _modifyStamp = value;
        }

        public bool? PublishToHls
        {
            get => _publishToHls;
            set => _publishToHls = value;
        }

        public bool? PublishToMp4
        {
            get => _publishToMP4;
            set => _publishToMP4 = value;
        }

        public bool? PublishToRtxp
        {
            get => _publishToRtxp;
            set => _publishToRtxp = value;
        }

        public bool? ResetWhenRePlay
        {
            get => _resetWhenRePlay;
            set => _resetWhenRePlay = value;
        }

        public int? StreamNoneReaderDelayMs
        {
            get => _streamNoneReaderDelayMS;
            set => _streamNoneReaderDelayMS = value;
        }
    }


    [Serializable]
    public class ZLMediaKitConfig_Hls
    {
        private int? _fileBufSize;
        private string? _filePath;
        private int? _segDur;
        private int? _segNum;
        private int? _segRetain;

        public int? FileBufSize
        {
            get => _fileBufSize;
            set => _fileBufSize = value;
        }

        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public int? SegDur
        {
            get => _segDur;
            set => _segDur = value;
        }

        public int? SegNum
        {
            get => _segNum;
            set => _segNum = value;
        }

        public int? SegRetain
        {
            get => _segRetain;
            set => _segRetain = value;
        }
    }


    [Serializable]
    public class ZLMediaKitConfig_Hook
    {
        private string? _admin_params;
        private bool? _enable;
        private string? _on_flow_report;
        private string? _on_http_access;
        private string? _on_play;
        private string? _on_publish;
        private string? _on_record_mp4;
        private string? _on_rtsp_auth;
        private string? _on_rtsp_realm;
        private string? _on_server_started;
        private string? _on_shell_login;
        private string? _on_stream_changed;
        private string? _on_stream_none_reader;
        private string? _on_stream_not_found;
        private int? _timeoutSec;

        public string? Admin_Params => _admin_params;

        public bool? Enable => _enable;

        public string? On_Flow_Report => _on_flow_report;

        public string? On_Http_Access => _on_http_access;

        public string? On_Play => _on_play;

        public string? On_Publish => _on_publish;

        public string? On_Record_Mp4 => _on_record_mp4;

        public string? On_Rtsp_Auth => _on_rtsp_auth;

        public string? On_Rtsp_Realm => _on_rtsp_realm;

        public string? On_Server_Started => _on_server_started;

        public string? On_Shell_Login => _on_shell_login;

        public string? On_Stream_Changed => _on_stream_changed;

        public string? On_Stream_None_Reader => _on_stream_none_reader;

        public string? On_Stream_Not_Found => _on_stream_not_found;

        public int? TimeoutSec => _timeoutSec;
    }


    [Serializable]
    public class ZLMediaKitConfig_Http
    {
        private string? _charSet;
        private bool? _dirMenu;
        private int? _keepAliveSecond;
        private int? _maxReqSize;
        private string? _notFound;
        private ushort? _port;
        private string? _rootPath;
        private int? _sendBufSize;
        private ushort? _sslport;

        public string? CharSet
        {
            get => _charSet;
            set => _charSet = value;
        }

        public bool? DirMenu
        {
            get => _dirMenu;
            set => _dirMenu = value;
        }

        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }

        public int? MaxReqSize
        {
            get => _maxReqSize;
            set => _maxReqSize = value;
        }

        public string? NotFound
        {
            get => _notFound;
            set => _notFound = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public string? RootPath
        {
            get => _rootPath;
            set => _rootPath = value;
        }

        public int? SendBufSize
        {
            get => _sendBufSize;
            set => _sendBufSize = value;
        }

        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Multicast
    {
        private string? _addrMax;
        private string? _addrMin;
        private int? _udpTTL;

        public string? AddrMax
        {
            get => _addrMax;
            set => _addrMax = value;
        }

        public string? AddrMin
        {
            get => _addrMin;
            set => _addrMin = value;
        }

        public int? UdpTtl
        {
            get => _udpTTL;
            set => _udpTTL = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Record
    {
        private string? _appName;
        private bool? _fastStart;
        private int? _fileBufSize;
        private string? _filePath;
        private bool? _fileRepeat;
        private int? _fileSecond;
        private int? _sampleMS;

        public string? AppName
        {
            get => _appName;
            set => _appName = value;
        }

        public bool? FastStart
        {
            get => _fastStart;
            set => _fastStart = value;
        }

        public int? FileBufSize
        {
            get => _fileBufSize;
            set => _fileBufSize = value;
        }

        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        public bool? FileRepeat
        {
            get => _fileRepeat;
            set => _fileRepeat = value;
        }

        public int? FileSecond
        {
            get => _fileSecond;
            set => _fileSecond = value;
        }

        public int? SampleMs
        {
            get => _sampleMS;
            set => _sampleMS = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Rtmp
    {
        private int? _handshakeSecond;
        private int? _keepAliveSecond;
        private bool? _modifyStamp;
        private ushort? _port;
        private ushort? _sslport;

        public int? HandshakeSecond
        {
            get => _handshakeSecond;
            set => _handshakeSecond = value;
        }

        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }

        public bool? ModifyStamp
        {
            get => _modifyStamp;
            set => _modifyStamp = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Rtp
    {
        private int? _audioMtuSize;
        private int? _clearCount;
        private ulong? _cycleMS;
        private int? _maxRtpCount;
        private int? _videoMtuSize;

        public int? AudioMtuSize
        {
            get => _audioMtuSize;
            set => _audioMtuSize = value;
        }

        public int? ClearCount
        {
            get => _clearCount;
            set => _clearCount = value;
        }

        public ulong? CycleMs
        {
            get => _cycleMS;
            set => _cycleMS = value;
        }

        public int? MaxRtpCount
        {
            get => _maxRtpCount;
            set => _maxRtpCount = value;
        }

        public int? VideoMtuSize
        {
            get => _videoMtuSize;
            set => _videoMtuSize = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Rtp_Proxy
    {
        private bool? _checkSource;
        private string? _dumpDir;
        private ushort? _port;
        private int? _timeoutSec;

        public bool? CheckSource
        {
            get => _checkSource;
            set => _checkSource = value;
        }

        public string? DumpDir
        {
            get => _dumpDir;
            set => _dumpDir = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public int? TimeoutSec
        {
            get => _timeoutSec;
            set => _timeoutSec = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Rtsp
    {
        private bool? _authBasic;
        private bool? _directProxy;
        private int? _handshakeSecond;
        private int? _keepAliveSecond;
        private ushort? _port;
        private ushort? _sslport;

        public bool? AuthBasic
        {
            get => _authBasic;
            set => _authBasic = value;
        }

        public bool? DirectProxy
        {
            get => _directProxy;
            set => _directProxy = value;
        }

        public int? HandshakeSecond
        {
            get => _handshakeSecond;
            set => _handshakeSecond = value;
        }

        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    [Serializable]
    public class ZLMediaKitConfig_Shell
    {
        private int? _maxReqSize;
        private ushort? _port;

        public int? MaxReqSize
        {
            get => _maxReqSize;
            set => _maxReqSize = value;
        }

        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }
    }


    [Serializable]
    public class ZLMediaKitConfig
    {
        private ZLMediaKitConfig_API? _api;
        private ZLMediaKitConfig_FFMpeg? _ffmpeg;
        private ZLMediaKitConfig_General? _general;
        private ZLMediaKitConfig_Hls? _hls;
        private ZLMediaKitConfig_Hook? _hook;
        private ZLMediaKitConfig_Http? _http;
        private ZLMediaKitConfig_Multicast? _multicast;
        private ZLMediaKitConfig_Record? _record;
        private ZLMediaKitConfig_Rtmp? _rtmp;
        private ZLMediaKitConfig_Rtp? _rtp;
        private ZLMediaKitConfig_Rtp_Proxy? _rtp_Proxy;
        private ZLMediaKitConfig_Rtsp _rtsp;
        private ZLMediaKitConfig_Shell _shell;

        public ZLMediaKitConfig_API? Api
        {
            get => _api;
            set => _api = value;
        }

        public ZLMediaKitConfig_FFMpeg? Ffmpeg
        {
            get => _ffmpeg;
            set => _ffmpeg = value;
        }

        public ZLMediaKitConfig_General? General
        {
            get => _general;
            set => _general = value;
        }

        public ZLMediaKitConfig_Hls? Hls
        {
            get => _hls;
            set => _hls = value;
        }

        public ZLMediaKitConfig_Hook? Hook
        {
            get => _hook;
            set => _hook = value;
        }

        public ZLMediaKitConfig_Http? Http
        {
            get => _http;
            set => _http = value;
        }

        public ZLMediaKitConfig_Multicast? Multicast
        {
            get => _multicast;
            set => _multicast = value;
        }

        public ZLMediaKitConfig_Record? Record
        {
            get => _record;
            set => _record = value;
        }

        public ZLMediaKitConfig_Rtmp? Rtmp
        {
            get => _rtmp;
            set => _rtmp = value;
        }

        public ZLMediaKitConfig_Rtp? Rtp
        {
            get => _rtp;
            set => _rtp = value;
        }

        public ZLMediaKitConfig_Rtp_Proxy? Rtp_Proxy
        {
            get => _rtp_Proxy;
            set => _rtp_Proxy = value;
        }

        public ZLMediaKitConfig_Rtsp Rtsp
        {
            get => _rtsp;
            set => _rtsp = value;
        }

        public ZLMediaKitConfig_Shell Shell
        {
            get => _shell;
            set => _shell = value;
        }
    }
}