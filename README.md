# StreamNode说明
## 简介
- 本项目是基于ZLMediaKit的流媒体控制管理接口平台，支持RTSP,GB28181的设备拉流与推流控制，GB28181部分支持PTZ控制。
- 对ZLMediaKit的源码做了一些小的改造，用于将ZLMediaKit的http回调增加流媒体服务的唯一标识，以及对ffmpeg管理部分的一个小修改
- /src/Common/config.cpp
~~~c++
namespace mediakit {
/*新增开始*/
    string generalGuid() {
        srand(time(0));
        std::string random_str("");
        for (int i = 0; i < 6; ++i) {
            for (int j = 0; j < 8; j++)
                switch (rand() % 2) {
                    case 1:
                        random_str += ('A' + rand() % 26);
                        break;
                    default:
                        random_str += ('0' + rand() % 10);
                        break;
                }
            if (i < 5)
                random_str += "-";
        }

        return random_str;
    }
/*新增结束*/
~~~

~~~c++
//通用配置项目
namespace General{
#define GENERAL_FIELD "general."
/*新增开始*/
const string kMediaServerId = GENERAL_FIELD"mediaServerId";
/*新增结束*/
const string kFlowThreshold = GENERAL_FIELD"flowThreshold";
const string kStreamNoneReaderDelayMS = GENERAL_FIELD"streamNoneReaderDelayMS";
const string kMaxStreamWaitTimeMS = GENERAL_FIELD"maxStreamWaitMS";
const string kEnableVhost = GENERAL_FIELD"enableVhost";
const string kAddMuteAudio = GENERAL_FIELD"addMuteAudio";
const string kResetWhenRePlay = GENERAL_FIELD"resetWhenRePlay";
const string kPublishToRtxp = GENERAL_FIELD"publishToRtxp";
const string kPublishToHls = GENERAL_FIELD"publishToHls";
const string kPublishToMP4 = GENERAL_FIELD"publishToMP4";
const string kMergeWriteMS = GENERAL_FIELD"mergeWriteMS";
const string kModifyStamp = GENERAL_FIELD"modifyStamp";

onceToken token([](){
    mINI::Instance()[kFlowThreshold] = 1024;
    mINI::Instance()[kStreamNoneReaderDelayMS] = 20 * 1000;
    mINI::Instance()[kMaxStreamWaitTimeMS] = 15 * 1000;
    mINI::Instance()[kEnableVhost] = 0;
    mINI::Instance()[kAddMuteAudio] = 1;
    mINI::Instance()[kResetWhenRePlay] = 1;
    mINI::Instance()[kPublishToRtxp] = 1;
    mINI::Instance()[kPublishToHls] = 1;
    mINI::Instance()[kPublishToMP4] = 0;
    mINI::Instance()[kMergeWriteMS] = 0;
    mINI::Instance()[kModifyStamp] = 0;
/*新增开始*/
    mINI::Instance()[kMediaServerId] = generalGuid();
/*新增结束*/
},nullptr);
~~~

- /src/Common/config.h
~~~c++
////////////通用配置///////////
namespace General{
/*新增开始*/
//每个流媒体服务器的ID（GUID）
extern const string kMediaServerId;
/*新增结束*/
//流量汇报事件流量阈值,单位KB，默认1MB
extern const string kFlowThreshold;
//流无人观看并且超过若干时间后才触发kBroadcastStreamNoneReader事件
//默认连续5秒无人观看然后触发kBroadcastStreamNoneReader事件
extern const string kStreamNoneReaderDelayMS;
//等待流注册超时时间，收到播放器后请求后，如果未找到相关流，服务器会等待一定时间，
//如果在这个时间内，相关流注册上了，那么服务器会立即响应播放器播放成功，
//否则会最多等待kMaxStreamWaitTimeMS毫秒，然后响应播放器播放失败
extern const string kMaxStreamWaitTimeMS;
//是否启动虚拟主机
extern const string kEnableVhost;
//拉流代理时是否添加静音音频
extern const string kAddMuteAudio;
//拉流代理时如果断流再重连成功是否删除前一次的媒体流数据，如果删除将重新开始，
//如果不删除将会接着上一次的数据继续写(录制hls/mp4时会继续在前一个文件后面写)
extern const string kResetWhenRePlay;
//是否默认推流时转换成rtsp或rtmp，hook接口(on_publish)中可以覆盖该设置
extern const string kPublishToRtxp ;
//是否默认推流时转换成hls，hook接口(on_publish)中可以覆盖该设置
extern const string kPublishToHls ;
//是否默认推流时mp4录像，hook接口(on_publish)中可以覆盖该设置
extern const string kPublishToMP4 ;
//合并写缓存大小(单位毫秒)，合并写指服务器缓存一定的数据后才会一次性写入socket，这样能提高性能，但是会提高延时
//开启后会同时关闭TCP_NODELAY并开启MSG_MORE
extern const string kMergeWriteMS ;
//全局的时间戳覆盖开关，在转协议时，对frame进行时间戳覆盖
extern const string kModifyStamp;
}//namespace General

~~~

-/server/FFmpegSource.cpp
~~~c++
/**
 * 定时检查媒体是否在线
 */
void FFmpegSource::startTimer(int timeout_ms) {
    weak_ptr<FFmpegSource> weakSelf = shared_from_this();
    _timer = std::make_shared<Timer>(1, [weakSelf, timeout_ms]() {
        auto strongSelf = weakSelf.lock();
        if (!strongSelf) {
            //自身已经销毁
            return false;
        }
        if (is_local_ip(strongSelf->_media_info._host)) {
            //推流给自己的，我们通过检查是否已经注册来判断FFmpeg是否工作正常
            strongSelf->findAsync(0, [&](const MediaSource::Ptr &src) {
                //同步查找流
                if (!src) {
                    //流不在线，重新拉流
/*修改原来的10到20，以保证ffmpeg拉流断掉后的重新拉流超时时间不会因为过短而反复失败*/
                    if(strongSelf->_replay_ticker.elapsedTime() > 20 * 1000){
/*修改原来的10到20，以保证ffmpeg拉流断掉后的重新拉流超时时间不会因为过短而反复失败*/
                        //上次重试时间超过10秒，那么再重试FFmpeg拉流
                        strongSelf->_replay_ticker.resetTime();
                        strongSelf->play(strongSelf->_src_url, strongSelf->_dst_url, timeout_ms, [](const SockException &) {});
                    }
                }
            });
        } else {
            //推流给其他服务器的，我们通过判断FFmpeg进程是否在线，如果FFmpeg推流中断，那么它应该会自动退出
            if (!strongSelf->_process.wait(false)) {
                //ffmpeg不在线，重新拉流
                strongSelf->play(strongSelf->_src_url, strongSelf->_dst_url, timeout_ms, [weakSelf](const SockException &ex) {
                    if(!ex){
                        //没有错误
                        return;
                    }
                    auto strongSelf = weakSelf.lock();
                    if (!strongSelf) {
                        //自身已经销毁
                        return;
                    }
                    //上次重试时间超过10秒，那么再重试FFmpeg拉流
                    strongSelf->startTimer(10 * 1000);
                });
            }
        }
        return true;
    }, _poller);
}
~~~
-server/WebHook.cpp
~~~c++
static ArgsType make_json(const MediaInfo &args){
    ArgsType body;
/*新增开始*/
    GET_CONFIG(string,mediaServerId,General::kMediaServerId);
/*新增结束*/
    body["mediaserverid"] =  mediaServerId;
    body["schema"] = args._schema;
    body["vhost"] = args._vhost;
    body["app"] = args._app;
    body["stream"] = args._streamid;
    body["params"] = args._param_strs;
    return std::move(body);
}
~~~
~~~c++
 //监听rtsp、rtmp源注册或注销事件
    NoticeCenter::Instance().addListener(nullptr,Broadcast::kBroadcastMediaChanged,[](BroadcastMediaChangedArgs){
        if(!hook_enable || hook_stream_chaned.empty()){
            return;
        }
        ArgsType body;
/*新增开始*/
        GET_CONFIG(string,mediaServerId,General::kMediaServerId);
        body["mediaserverid"] =  mediaServerId;
/*新增结束*/
        body["regist"] = bRegist;
        body["schema"] = sender.getSchema();
        body["vhost"] = sender.getVhost();
        body["app"] = sender.getApp();
        body["stream"] = sender.getId();
        //执行hook
        do_http_hook(hook_stream_chaned,body, nullptr);
    });
~~~
~~~c++
#ifdef ENABLE_MP4
    //录制mp4文件成功后广播
    NoticeCenter::Instance().addListener(nullptr,Broadcast::kBroadcastRecordMP4,[](BroadcastRecordMP4Args){
        if(!hook_enable || hook_record_mp4.empty()){
            return;
        }
        ArgsType body;
/*新增开始*/
        GET_CONFIG(string,mediaServerId,General::kMediaServerId);
        body["mediaserverid"] =  mediaServerId;
/*新增结束*/
        body["start_time"] = (Json::UInt64)info.ui64StartedTime;
        body["time_len"] = (Json::UInt64)info.ui64TimeLen;
        body["file_size"] = (Json::UInt64)info.ui64FileSize;
        body["file_path"] = info.strFilePath;
        body["file_name"] = info.strFileName;
        body["folder"] = info.strFolder;
        body["url"] = info.strUrl;
        body["app"] = info.strAppName;
        body["stream"] = info.strStreamId;
        body["vhost"] = info.strVhost;
        //执行hook
        do_http_hook(hook_record_mp4,body, nullptr);
    });
#endif //ENABLE_MP4
~~~c++
NoticeCenter::Instance().addListener(nullptr,Broadcast::kBroadcastShellLogin,[](BroadcastShellLoginArgs){
        if(!hook_enable || hook_shell_login.empty() || sender.get_peer_ip() == "127.0.0.1"){
            invoker("");
            return;
        }
        ArgsType body;
/*新增开始*/
        GET_CONFIG(string,mediaServerId,General::kMediaServerId);
        body["mediaserverid"] =  mediaServerId;
/*新增结束*/
        body["ip"] = sender.get_peer_ip();
        body["port"] = sender.get_peer_port();
        body["id"] = sender.getIdentifier();
        body["user_name"] = user_name;
        body["passwd"] = passwd;

        //执行hook
        do_http_hook(hook_shell_login,body, [invoker](const Value &,const string &err){
            invoker(err);
        });
    });
~~~c++
NoticeCenter::Instance().addListener(nullptr,Broadcast::kBroadcastStreamNoneReader,[](BroadcastStreamNoneReaderArgs){
        if(!hook_enable || hook_stream_none_reader.empty()){
            return;
        }

        ArgsType body;
/*新增开始*/
        GET_CONFIG(string,mediaServerId,General::kMediaServerId);
        body["mediaserverid"] =  mediaServerId;
/*新增结束*/
        body["schema"] = sender.getSchema();
        body["vhost"] = sender.getVhost();
        body["app"] = sender.getApp();
        body["stream"] = sender.getId();
        weak_ptr<MediaSource> weakSrc = sender.shared_from_this();
        //执行hook
        do_http_hook(hook_stream_none_reader,body, [weakSrc](const Value &obj,const string &err){
            bool flag = obj["close"].asBool();
            auto strongSrc = weakSrc.lock();
            if(!flag || !err.empty() || !strongSrc){
                return;
            }
            strongSrc->close(false);
        });

    });
~~~
~~~c++
 NoticeCenter::Instance().addListener(nullptr,Broadcast::kBroadcastHttpAccess,[](BroadcastHttpAccessArgs){
        if(sender.get_peer_ip() == "127.0.0.1" || parser.Params() == hook_adminparams){
            //如果是本机或超级管理员访问，那么不做访问鉴权；权限有效期1个小时
            invoker("","",60 * 60);
            return;
        }
        if(!hook_enable || hook_http_access.empty()){
            //未开启http文件访问鉴权，那么允许访问，但是每次访问都要鉴权；
            //因为后续随时都可能开启鉴权(重载配置文件后可能重新开启鉴权)
            invoker("","",0);
            return;
        }

        ArgsType body;
/*新增开始*/
        GET_CONFIG(string,mediaServerId,General::kMediaServerId);
        body["mediaserverid"] =  mediaServerId;
/*新增结束*/
        body["ip"] = sender.get_peer_ip();
        body["port"] = sender.get_peer_port();
        body["id"] = sender.getIdentifier();
        body["path"] = path;
        body["is_dir"] = is_dir;
        body["params"] = parser.Params();
        for(auto &pr : parser.getHeader()){
            body[string("header.") + pr.first] = pr.second;
        }
        //执行hook
        do_http_hook(hook_http_access,body, [invoker](const Value &obj,const string &err){
            if(!err.empty()){
                //如果接口访问失败，那么仅限本次没有访问http服务器的权限
                invoker(err,"",0);
                return;
            }
            //err参数代表不能访问的原因，空则代表可以访问
            //path参数是该客户端能访问或被禁止的顶端目录，如果path为空字符串，则表述为当前目录
            //second参数规定该cookie超时时间，如果second为0，本次鉴权结果不缓存
            invoker(obj["err"].asString(),obj["path"].asString(),obj["second"].asInt());
        });
    });

    //汇报服务器重新启动
    reportServerStarted();
}

~~~

# 组成部分
## StreamNodeWebApi
- 全局的流媒体管理API服务，包含了所有流媒体功能的控制，如摄像头注册，录制计划，rtp推流,ptz控制等。
- 此服务全局只存在一份，负责收集来原于StreamMediaServerKeeper的流媒体服务信息，并进行流媒体服务的管理。
## StreamMediaServerKeeper
- 用于流媒体的相关控制，如控制流媒体服务器的启动，停止，重启，获取某个录制文件是否存在，裁剪与合并任务的执行等。
- 此服务针对于流媒体进行部署，每一个流媒体服务都需要部署一个StreamMediaServerKeeper,此服务与StreamNodeWebApi通过WebApi进行通讯。
- 此服务启动时，将自己的状态以心跳的方式汇报给StreamNodeWebApi,并帮助StreamNodeWebApi服务控制ZLMediaKit流媒体服务器。
## 其他内容
- LibGB28181SipGate     GB28181 SIP信令服务网关
- GB28181.SIPSorcery    GB28181 SIP信令协议栈
- CommonFunctions       通用方法与通用结构
- Logger4Net            部分组件用到的日志工具
- sipsorcery            SIP协议栈
- StreamNodeCtrlApis    控制API集合
- Test_*                测试的工程


# 配置文件解释
## StreamNodeWebApi/Config/gb28181.xml
- 此文件用于指定GB28181 sip信令网关的参数
~~~xml
<sipaccounts>
  <sipaccount>
    <ID>10</ID>
    <Name>上级平台</Name>
    <GbVersion>GB-2016</GbVersion>
    <LocalID>34020000002000000001</LocalID>  //服务器id
    <LocalIP>192.168.2.43</LocalIP> //服务器ip
    <LocalPort>5060</LocalPort> //服务器的端口
    <RemotePort>5060</RemotePort>
    <Authentication>false</Authentication>
    <SIPUsername>admin</SIPUsername>
    <SIPPassword>123456</SIPPassword>
    <MsgProtocol>UDP</MsgProtocol> //sip服务的端口模式
    <StreamProtocol>UDP</StreamProtocol> 
    <TcpMode>passive</TcpMode>
    <MsgEncode>GB2312</MsgEncode>
    <PacketOutOrder>true</PacketOutOrder>
    <KeepaliveInterval>5000</KeepaliveInterval>
    <KeepaliveNumber>3</KeepaliveNumber>
    <MediaIP>192.168.2.43</MediaIP> //流媒体服务的ip地址
    <MediaPort>10000</MediaPort>  //忽略
    <MediaPortMin>10000</MediaPortMin> //忽略
    <MediaPortMax>10000</MediaPortMax> //忽略
    <!--<ServiceType>GBToGB</ServiceType>-->
    <!--<ServiceType>GBToDevice</ServiceType>-->
  </sipaccount>
</sipaccounts>
~~~


## StreamNodeWebApi/system.conf
- StreamNodeWebApi的配置文件,参数名与参数值以::分开，每行以;结束
- 数据库方面采用CodeFirst 模式，在数据库中建立一个名为streamnode的库，数据表会自动创建
~~~
httpport::5800;   //webapi的端口
password::password123!@#; //暂时无用
allowkey::0D906284-6801-4B84-AEC9-DCE07FAE81DA	*	192.168.2.*	; //鉴权，暂时无用
db::Data Source=192.168.2.35;Port=3306;User ID=root;Password=password; Initial Catalog=streamnode;Charset=utf8; SslMode=none;Min pool size=1;//数据库连接串
dbtype::mysql;//数据库类型
ffmpegpath::./ffmpeg;//ffmpeg可执行文件的位置
~~~

## StreamMediaServerKeeper/Config.conf 
- StreamMediaServerKeeper的配置文件
~~~
MediaServerBinPath::/root/MediaService/MediaServer;//ZLMediaKit流媒体服务器可执行文件路径
StreamNodeServerUrl::http://192.168.2.43:5800/WebHook/MediaServerRegister; //向哪个StreamNodeWebApi注册自己的服务
HttpPort::6880;//服务的WebApi端口
IpAddress::192.168.2.43;//本机ip地址
~~~


# 运行
## 环境
- .net core 3.1
- mysql 5.5以上或者其他freesql支持的数据库
- ffmpeg 4.2.2以上
- ffmpeg 需要放在StreamNodeWebApi和StreamMediaServerKeeper的部署目录中
## 启动
- StreamNodeWebApi 全局只启动一份
- 部署目录中手工创建一个log文件夹
~~~shell
nohup dotnet StreamNodeWebApi.dll >./log/run.log &
~~~
- StreamMediaServerKeeper 一个流媒体启动一份，可以与StreamNodeWebApi不在同一台服务器
- 部署目录中手工创建一个log文件夹
~~~shell
nohup dotnet StreamMediaServerKeeper.dll >./log/run.log &
~~~
# 调试
## StreamNodeWebApi
- http://ip:port(5800)/swagger/index.html
## StreamMediaServerKeeper
- http://ip:port(6880)/swagger/index.html
