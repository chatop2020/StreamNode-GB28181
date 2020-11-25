using System;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 流媒体服务器的配置结构-API
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_API
    {
        private bool? _apiDebug;
        private string? _defaultSnap;
        private string? _secret;
        private string? _snapRoot;

        /// <summary>
        /// 是否启用Debug日志输出
        /// </summary>
        public bool? ApiDebug
        {
            get => _apiDebug;
            set => _apiDebug = value;
        }

        /// <summary>
        /// 启用截图？
        /// </summary>
        public string? DefaultSnap
        {
            get => _defaultSnap;
            set => _defaultSnap = value;
        }

        /// <summary>
        /// WebApi鉴权用的Key
        /// </summary>
        public string? Secret
        {
            get => _secret;
            set => _secret = value;
        }

        /// <summary>
        /// 截图保存路径
        /// </summary>
        public string? SnapRoot
        {
            get => _snapRoot;
            set => _snapRoot = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-FFMPEG
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_FFMpeg
    {
        private string? _bin;
        private string? _cmd;
        private string? _log;
        private string? _snap;

        /// <summary>
        /// ffmpeg可执行文件路径
        /// </summary>
        public string? Bin
        {
            get => _bin;
            set => _bin = value;
        }

        /// <summary>
        /// ffmpeg的命令行参数
        /// </summary>
        public string? Cmd
        {
            get => _cmd;
            set => _cmd = value;
        }

        /// <summary>
        /// ffmpeg的日志文件路径
        /// </summary>
        public string? Log
        {
            get => _log;
            set => _log = value;
        }

        /// <summary>
        /// ffmpeg获取截图的命令
        /// </summary>
        public string? Snap
        {
            get => _snap;
            set => _snap = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-通用
    /// </summary>
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
        private bool? _hls_demand;
        private bool? _rtsp_demand;
        private bool? _rtmp_demand;
        private bool? _ts_demand;
        private bool? _fmp4_demand;
      

        /// <summary>
        /// 自动静音
        /// </summary>
        public bool? AddMuteAudio
        {
            get => _addMuteAudio;
            set => _addMuteAudio = value;
        }

        /// <summary>
        /// 启用vhost标记
        /// </summary>
        public bool? EnableVhost
        {
            get => _enableVhost;
            set => _enableVhost = value;
        }

        /// <summary>
        /// 参数控制触发hook.on_flow_report事件阈值，使用流量超过该阈值后才触发，单位KB
        /// </summary>
        public int? FlowThreshold
        {
            get => _flowThreshold;
            set => _flowThreshold = value;
        }

        /// <summary>
        /// 播放最多等待时间，单位毫秒
        /// 播放在播放某个流时，如果该流不存在，
        /// ZLMediaKit会最多让播放器等待maxStreamWaitMS毫秒
        /// 如果在这个时间内，该流注册成功，那么会立即返回播放器播放成功
        /// 否则返回播放器未找到该流，该机制的目的是可以先播放再推流
        /// </summary>
        public int? MaxStreamWaitMs
        {
            get => _maxStreamWaitMS;
            set => _maxStreamWaitMS = value;
        }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string? MediaServerId
        {
            get => _mediaServerId;
            set => _mediaServerId = value;
        }

        /// <summary>
        /// 合并写缓存大小(单位毫秒)，合并写指服务器缓存一定的数据后才会一次性写入socket，这样能提高性能，但是会提高延时
        /// 开启后会同时关闭TCP_NODELAY并开启MSG_MORE
        /// </summary>
        public int? MergeWriteMs
        {
            get => _mergeWriteMS;
            set => _mergeWriteMS = value;
        }

        /// <summary>
        /// 全局的时间戳覆盖开关，在转协议时，对frame进行时间戳覆盖
        /// 该开关对rtsp/rtmp/rtp推流、rtsp/rtmp/hls拉流代理转协议时生效
        /// 会直接影响rtsp/rtmp/hls/mp4/flv等协议的时间戳
        /// 同协议情况下不影响(例如rtsp/rtmp推流，那么播放rtsp/rtmp时不会影响时间戳)
        /// </summary>
        public bool? ModifyStamp
        {
            get => _modifyStamp;
            set => _modifyStamp = value;
        }

        /// <summary>
        /// 是否默认推流时转换成hls，hook接口(on_publish)中可以覆盖该设置
        /// </summary>
        public bool? PublishToHls
        {
            get => _publishToHls;
            set => _publishToHls = value;
        }

        /// <summary>
        /// 是否默认推流时mp4录像，hook接口(on_publish)中可以覆盖该设置
        /// </summary>
        public bool? PublishToMp4
        {
            get => _publishToMP4;
            set => _publishToMP4 = value;
        }

        /// <summary>
        /// 是否默认推流时rtxp录像，hook接口(on_publish)中可以覆盖该设置
        /// </summary>
        public bool? PublishToRtxp
        {
            get => _publishToRtxp;
            set => _publishToRtxp = value;
        }

        /// <summary>
        /// 拉流代理时如果断流再重连成功是否删除前一次的媒体流数据，如果删除将重新开始，
        /// 如果不删除将会接着上一次的数据继续写(录制hls/mp4时会继续在前一个文件后面写)
        /// </summary>
        public bool? ResetWhenRePlay
        {
            get => _resetWhenRePlay;
            set => _resetWhenRePlay = value;
        }

        /// <summary>
        /// 某个流无人观看时，触发hook.on_stream_none_reader事件的最大等待时间，单位毫秒
        /// 在配合hook.on_stream_none_reader事件时，可以做到无人观看自动停止拉流或停止接收推流
        /// </summary>
        public int? StreamNoneReaderDelayMs
        {
            get => _streamNoneReaderDelayMS;
            set => _streamNoneReaderDelayMS = value;
        }

        public bool? Hls_Demand
        {
            get => _hls_demand;
            set => _hls_demand = value;
        }

        public bool? Rtsp_Demand
        {
            get => _rtsp_demand;
            set => _rtsp_demand = value;
        }

        public bool? Rtmp_Demand
        {
            get => _rtmp_demand;
            set => _rtmp_demand = value;
        }

        public bool? Ts_Demand
        {
            get => _ts_demand;
            set => _ts_demand = value;
        }

        public bool? Fmp4_Demand
        {
            get => _fmp4_demand;
            set => _fmp4_demand = value;
        }
    }


    /// <summary>
    /// 流媒体服务器的配置结构-HLS
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Hls
    {
        private int? _fileBufSize;
        private string? _filePath;
        private int? _segDur;
        private int? _segNum;
        private int? _segRetain;
        private bool? _broadcastRecordTs;

        /// <summary>
        /// 文件写入缓冲区大小
        /// </summary>
        public int? FileBufSize
        {
            get => _fileBufSize;
            set => _fileBufSize = value;
        }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        /// <summary>
        /// 文件段长度
        /// </summary>
        public int? SegDur
        {
            get => _segDur;
            set => _segDur = value;
        }

        /// <summary>
        /// 文件段数量
        /// </summary>
        public int? SegNum
        {
            get => _segNum;
            set => _segNum = value;
        }

        /// <summary>
        /// HLS切片从m3u8文件中移除后，继续保留在磁盘上的个数
        /// </summary>
        public int? SegRetain
        {
            get => _segRetain;
            set => _segRetain = value;
        }

        /// <summary>
        /// 是否广播 ts 切片完成通知
        /// </summary>
        public bool? BroadcastRecordTs
        {
            get => _broadcastRecordTs;
            set => _broadcastRecordTs = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Hook
    /// </summary>
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
        private string? _on_record_ts;
        private string? _on_rtsp_auth;
        private string? _on_rtsp_realm;
        private string? _on_server_started;
        private string? _on_shell_login;
        private string? _on_stream_changed;
        private string? _on_stream_none_reader;
        private string? _on_stream_not_found;
        private int? _timeoutSec;


        /// <summary>
        /// 在推流时，如果url参数匹对admin_params，那么可以不经过hook鉴权直接推流成功，播放时亦然
        /// 该配置项的目的是为了开发者自己调试测试，该参数暴露后会有泄露隐私的安全隐患
        /// </summary>
        public string? Admin_Params => _admin_params;

        /// <summary>
        /// 是否启用hook
        /// </summary>
        public bool? Enable => _enable;

        /// <summary>
        /// 流量汇报
        /// </summary>
        public string? On_Flow_Report => _on_flow_report;

        /// <summary>
        /// 访问http文件鉴权事件，置空则关闭鉴权
        /// </summary>
        public string? On_Http_Access => _on_http_access;

        /// <summary>
        /// 当有播放事件时触发
        /// </summary>
        public string? On_Play => _on_play;

        /// <summary>
        /// 当有音视频流发布时触发
        /// </summary>
        public string? On_Publish => _on_publish;

        /// <summary>
        /// 当录制mp4（段）结束后触发
        /// </summary>
        public string? On_Record_Mp4 => _on_record_mp4;

        /// <summary>
        /// 录制 hls ts 切片完成事件
        /// </summary>
        public string? On_Record_TS => _on_record_ts;
        /// <summary>
        /// rtsp鉴权事件，此事件中比对rtsp的用户名密码
        /// </summary>
        public string? On_Rtsp_Auth => _on_rtsp_auth;

        /// <summary>
        /// rtsp播放是否开启专属鉴权事件，置空则关闭rtsp鉴权。rtsp播放鉴权还支持url方式鉴权
        /// 建议开发者统一采用url参数方式鉴权，rtsp用户名密码鉴权一般在设备上用的比较多
        /// 开启rtsp专属鉴权后，将不再触发on_play鉴权事件
        /// </summary>
        public string? On_Rtsp_Realm => _on_rtsp_realm;

        /// <summary>
        /// 当流媒体服务器启动时触发
        /// </summary>
        public string? On_Server_Started => _on_server_started;

        /// <summary>
        /// 当shell登陆时触发
        /// </summary>
        public string? On_Shell_Login => _on_shell_login;

        /// <summary>
        /// 当有流状态变改时触发
        /// </summary>
        public string? On_Stream_Changed => _on_stream_changed;

        /// <summary>
        /// 当流无人观看时触发
        /// </summary>
        public string? On_Stream_None_Reader => _on_stream_none_reader;

        /// <summary>
        /// 当流不存在时触发
        /// </summary>
        public string? On_Stream_Not_Found => _on_stream_not_found;

        /// <summary>
        /// hook api最大等待回复时间，单位秒
        /// </summary>
        public int? TimeoutSec => _timeoutSec;
    }


    /// <summary>
    /// 流媒体服务器的配置结构-Http
    /// </summary>
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

        /// <summary>
        /// 字符集
        /// </summary>
        public string? CharSet
        {
            get => _charSet;
            set => _charSet = value;
        }

        /// <summary>
        /// 是否显示文件夹菜单，开启后可以浏览文件夹
        /// </summary>
        public bool? DirMenu
        {
            get => _dirMenu;
            set => _dirMenu = value;
        }

        /// <summary>
        /// http链接超时时间
        /// </summary>
        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }


        /// <summary>
        /// 调试telnet服务器接受最大bufffer大小
        /// </summary>
        public int? MaxReqSize
        {
            get => _maxReqSize;
            set => _maxReqSize = value;
        }

        /// <summary>
        /// 404页面
        /// </summary>
        public string? NotFound
        {
            get => _notFound;
            set => _notFound = value;
        }

        /// <summary>
        /// http服务端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 根目录地址
        /// </summary>
        public string? RootPath
        {
            get => _rootPath;
            set => _rootPath = value;
        }

        /// <summary>
        /// 发送缓冲大小
        /// </summary>
        public int? SendBufSize
        {
            get => _sendBufSize;
            set => _sendBufSize = value;
        }

        /// <summary>
        /// ssl端口
        /// </summary>
        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Multicast
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Multicast
    {
        private string? _addrMax;
        private string? _addrMin;
        private int? _udpTTL;

        /// <summary>
        /// 最大地址
        /// </summary>
        public string? AddrMax
        {
            get => _addrMax;
            set => _addrMax = value;
        }

        /// <summary>
        /// 最小地址
        /// </summary>
        public string? AddrMin
        {
            get => _addrMin;
            set => _addrMin = value;
        }

        /// <summary>
        /// udp ttl
        /// </summary>
        public int? UdpTtl
        {
            get => _udpTTL;
            set => _udpTTL = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-录制
    /// </summary>
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

        /// <summary>
        /// 流媒体APP标记
        /// </summary>
        public string? AppName
        {
            get => _appName;
            set => _appName = value;
        }

        /// <summary>
        /// mp4的快速加载播放标记
        /// </summary>
        public bool? FastStart
        {
            get => _fastStart;
            set => _fastStart = value;
        }

        /// <summary>
        /// 文件写入缓冲大小
        /// </summary>
        public int? FileBufSize
        {
            get => _fileBufSize;
            set => _fileBufSize = value;
        }

        /// <summary>
        /// 文件录制位置 
        /// </summary>
        public string? FilePath
        {
            get => _filePath;
            set => _filePath = value;
        }

        /// <summary>
        /// MP4点播(rtsp/rtmp/http-flv/ws-flv)是否循环播放文件
        /// </summary>
        public bool? FileRepeat
        {
            get => _fileRepeat;
            set => _fileRepeat = value;
        }

        /// <summary>
        /// mp4录制切片时间，单位秒
        /// </summary>
        public int? FileSecond
        {
            get => _fileSecond;
            set => _fileSecond = value;
        }

        /// <summary>
        /// mp4点播每次流化数据量，单位毫秒，
        /// 减少该值可以让点播数据发送量更平滑，增大该值则更节省cpu资源
        /// </summary>
        public int? SampleMs
        {
            get => _sampleMS;
            set => _sampleMS = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Rtmp
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Rtmp
    {
        private int? _handshakeSecond;
        private int? _keepAliveSecond;
        private bool? _modifyStamp;
        private ushort? _port;
        private ushort? _sslport;

        /// <summary>
        /// rtmp必须在此时间内完成握手，否则服务器会断开链接，单位秒
        /// </summary>
        public int? HandshakeSecond
        {
            get => _handshakeSecond;
            set => _handshakeSecond = value;
        }

        /// <summary>
        /// rtmp超时时间，如果该时间内未收到客户端的数据，
        /// 或者tcp发送缓存超过这个时间，则会断开连接，单位秒
        /// </summary>
        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }

        /// <summary>
        /// 在接收rtmp推流时，是否重新生成时间戳(很多推流器的时间戳着实很烂)
        /// </summary>
        public bool? ModifyStamp
        {
            get => _modifyStamp;
            set => _modifyStamp = value;
        }

        /// <summary>
        /// rtmp端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// rtmp ssl端口
        /// </summary>
        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Rtp
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Rtp
    {
        private int? _audioMtuSize;
     //   private int? _clearCount;
        private ulong? _cycleMS;
       // private int? _maxRtpCount;
        private int? _videoMtuSize;

        /// <summary>
        /// 音频的mtu大小
        /// </summary>
        public int? AudioMtuSize
        {
            get => _audioMtuSize;
            set => _audioMtuSize = value;
        }

        /// <summary>
        /// 清除数量
        /// </summary>
        /*
        public int? ClearCount
        {
            get => _clearCount;
            set => _clearCount = value;
        }
        */

        /// <summary>
        /// rtp时间戳回环时间，单位毫秒
        /// </summary>
        public ulong? CycleMs
        {
            get => _cycleMS;
            set => _cycleMS = value;
        }

        /// <summary>
        /// 最大rtp包数量
        /// </summary>
        /*
        public int? MaxRtpCount
        {
            get => _maxRtpCount;
            set => _maxRtpCount = value;
        }
        */

        /// <summary>
        ///  视频mtu大小，该参数限制rtp最大字节数，推荐不要超过1400
        /// </summary>
        public int? VideoMtuSize
        {
            get => _videoMtuSize;
            set => _videoMtuSize = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Rtp代理
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Rtp_Proxy
    {
        private bool? _checkSource;
        private string? _dumpDir;
        private ushort? _port;
        private int? _timeoutSec;


        /// <summary>
        /// 是否检查源
        /// </summary>
        public bool? CheckSource
        {
            get => _checkSource;
            set => _checkSource = value;
        }

        /// <summary>
        /// dump地址
        /// </summary>
        public string? DumpDir
        {
            get => _dumpDir;
            set => _dumpDir = value;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int? TimeoutSec
        {
            get => _timeoutSec;
            set => _timeoutSec = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-Rtsp
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Rtsp
    {
        private bool? _authBasic;
        private bool? _directProxy;
        private int? _handshakeSecond;
        private int? _keepAliveSecond;
        private ushort? _port;
        private ushort? _sslport;

        /// <summary>
        /// rtsp专有鉴权方式是采用base64还是md5方式
        /// </summary>
        public bool? AuthBasic
        {
            get => _authBasic;
            set => _authBasic = value;
        }

        /// <summary>
        /// rtsp拉流代理是否是直接代理模式
        /// 直接代理后支持任意编码格式，但是会导致GOP缓存无法定位到I帧，可能会导致开播花屏
        /// 并且如果是tcp方式拉流，如果rtp大于mtu会导致无法使用udp方式代理
        /// 假定您的拉流源地址不是264或265或AAC，那么你可以使用直接代理的方式来支持rtsp代理
        /// 默认开启rtsp直接代理，rtmp由于没有这些问题，是强制开启直接代理的
        /// </summary>
        public bool? DirectProxy
        {
            get => _directProxy;
            set => _directProxy = value;
        }

        /// <summary>
        /// #rtsp必须在此时间内完成握手，否则服务器会断开链接，单位秒
        /// </summary>
        public int? HandshakeSecond
        {
            get => _handshakeSecond;
            set => _handshakeSecond = value;
        }

        /// <summary>
        /// rtsp超时时间，如果该时间内未收到客户端的数据，
        /// 或者tcp发送缓存超过这个时间，则会断开连接，单位秒
        /// </summary>
        public int? KeepAliveSecond
        {
            get => _keepAliveSecond;
            set => _keepAliveSecond = value;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }

        /// <summary>
        /// ssl端口
        /// </summary>
        public ushort? Sslport
        {
            get => _sslport;
            set => _sslport = value;
        }
    }

    /// <summary>
    /// 流媒体服务器的配置结构-shell
    /// </summary>
    [Serializable]
    public class ZLMediaKitConfig_Shell
    {
        private int? _maxReqSize;
        private ushort? _port;

        /// <summary>
        /// 调试telnet服务器接受最大bufffer大小
        /// </summary>
        public int? MaxReqSize
        {
            get => _maxReqSize;
            set => _maxReqSize = value;
        }

        /// <summary>
        /// 端口
        /// </summary>
        public ushort? Port
        {
            get => _port;
            set => _port = value;
        }
    }


    /// <summary>
    /// ZLMediaKit配置
    /// </summary>
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

        /// <summary>
        /// ZLMeidaKit_Api配置
        /// </summary>
        public ZLMediaKitConfig_API? Api
        {
            get => _api;
            set => _api = value;
        }

        /// <summary>
        /// ZLMeidaKit_FFMPEG配置
        /// </summary>
        public ZLMediaKitConfig_FFMpeg? Ffmpeg
        {
            get => _ffmpeg;
            set => _ffmpeg = value;
        }

        /// <summary>
        /// ZLMeidaKit_通用配置
        /// </summary>
        public ZLMediaKitConfig_General? General
        {
            get => _general;
            set => _general = value;
        }

        /// <summary>
        /// ZLMeidaKit_HLS配置
        /// </summary>
        public ZLMediaKitConfig_Hls? Hls
        {
            get => _hls;
            set => _hls = value;
        }

        /// <summary>
        /// ZLMeidaKit_HOOK配置
        /// </summary>
        public ZLMediaKitConfig_Hook? Hook
        {
            get => _hook;
            set => _hook = value;
        }

        /// <summary>
        /// ZLMeidaKit_HTTP配置
        /// </summary>
        public ZLMediaKitConfig_Http? Http
        {
            get => _http;
            set => _http = value;
        }

        /// <summary>
        /// ZLMeidaKit_Multicast配置
        /// </summary>
        public ZLMediaKitConfig_Multicast? Multicast
        {
            get => _multicast;
            set => _multicast = value;
        }

        /// <summary>
        /// ZLMeidaKit_Record配置
        /// </summary>
        public ZLMediaKitConfig_Record? Record
        {
            get => _record;
            set => _record = value;
        }

        /// <summary>
        /// ZLMeidaKit_Rtmp配置
        /// </summary>
        public ZLMediaKitConfig_Rtmp? Rtmp
        {
            get => _rtmp;
            set => _rtmp = value;
        }

        /// <summary>
        /// ZLMeidaKit_Rtp配置
        /// </summary>
        public ZLMediaKitConfig_Rtp? Rtp
        {
            get => _rtp;
            set => _rtp = value;
        }

        /// <summary>
        /// ZLMeidaKit_Rtp_Proxy配置
        /// </summary>
        public ZLMediaKitConfig_Rtp_Proxy? Rtp_Proxy
        {
            get => _rtp_Proxy;
            set => _rtp_Proxy = value;
        }

        /// <summary>
        /// ZLMeidaKit_Rtsp配置
        /// </summary>
        public ZLMediaKitConfig_Rtsp Rtsp
        {
            get => _rtsp;
            set => _rtsp = value;
        }

        /// <summary>
        /// ZLMeidaKit_shell配置
        /// </summary>
        public ZLMediaKitConfig_Shell Shell
        {
            get => _shell;
            set => _shell = value;
        }
    }
}