# StreamNode说明
## 简介
- 本项目是基于ZLMediaKit的流媒体控制管理接口平台，支持RTSP,GB28181的设备拉流与推流控制，GB28181部分支持PTZ控制。
- 对ZLMediaKit的源码做了一些小的改造，用于将ZLMediaKit的http回调增加流媒体服务的唯一标识，以及对ffmpeg管理部分的一个小修改
- 【新增】支持对公网动态ip的GB28181设备支持，通过数据库中的标记来确定是否校验设备IP地址，因此可以支持如4G国标协议执法记录仪接入
- 【修复】Sip网关支持部署在内网，映射到公网IP的端口，即可提供服务,同时修复了推流设备在内网中GB28181协议注册时未正确获得真实设备IP的问题，表示可以正确识别与通讯内网的GB28181设备。
## 接口功能
### DvrPlan 录制计划
- /DvrPlan​/DeleteDvrPlanById          删除一个录制计划ById
- ​/DvrPlan​/OnOrOffDvrPlanById         启用或停用一个录制计划
- ​/DvrPlan​/SetDvrPlanById         修改录制计划ById
- /DvrPlan​/CreateDvrPlan           创建录制计划
- /DvrPlan​/GetDvrPlan          获取录制计划
### MediaServer 流媒体控制
- ​/MediaServer​/CutOrMergeVideoFile        添加一个裁剪合并任务
- ​/MediaServer​/GetMergeTaskStatus         获取裁剪合并任务状态
- /MediaServer​/GetBacklogTaskList          获取裁剪合并任务积压列表
- /MediaServer​/UndoSoftDelete          恢复被软删除的录像文件
- /MediaServer​/HardDeleteDvrVideoById          删除一个录像文件ById（硬删除，立即删除文件，数据库做delete标记）
- /MediaServer​/HardDeleteDvrVideoByIdList          删除一批录像文件ById（硬删除，立即删除文件，数据库做delete标记）
- /MediaServer​/SoftDeleteDvrVideoById          删除一个录像文件ById（软删除，只做标记，不删除文件，文件在24小时后删除）
- /MediaServer​/GetDvrVideoById         根据id获取视频文件信息
- /MediaServer​/GetDvrVideoList         获取录像文件列表
- /MediaServer​/GetCameraInstanceListEx         扩展查询已注册摄像头列表
- /MediaServer​/GetCameraInstanceList           获取摄像头实例列表
- /MediaServer​/ModifyCameraInstance            修改一个注册摄像头实例
- /MediaServer​/DeleteCameraInstance            删除一个摄像头实例
- ​/MediaServer​/AddCameraInstance          注册添加一个摄像头实例
- /MediaServer​/GetPlayerSessionList            获取在线播放器列表
- /MediaServer​/GetCameraInstanceByCameraId         根据摄像头ID查询在线摄像头对象
- /MediaServer​/GetCameraSessionList            获取在线摄像头列表
- /MediaServer​/GetConfig           获取流媒体配置信息
- /MediaServer​/AddFFmpegProxy          启动一个ffmpeg代理流
- /MediaServer​/CloseStreams            关闭一个流
- /MediaServer​/GetStreamList           获取流列表
- /MediaServer​/StartRecord         启动流的录制
- /MediaServer​/StopRecord          停止流的录制
- /MediaServer​/GetRecordStatus         获取流的录制状态
- /MediaServer​/OpenRtpPort         打开某个rtp端口
- ​/MediaServer​/CloseRtpPort           关闭某个rtp端口
- ​/MediaServer​/GetRtpPortList         获取流媒体已经开放的rtp端口列表
- /MediaServer​/CheckMediaServerRunning         检查流媒体服务是否正在运行
- /MediaServer​/RestartMediaServer          重启流媒体服务
- /MediaServer​/StopMediaServer         关闭流媒体服务
- /MediaServer​/StartMediaServer            启动流媒体服务
- /MediaServer/ActivateSipCamera            对Sip网关自动写入的GB28181设备进行激活
### Sip网关相关
- /SipGate​/ActiveDeviceCatalogQuery           获取Sip设备的目录列表
- /SipGate​/GetAutoPushStreamState          获取Sip网关自动推流状态
- SipGate​/SetAutoPushStreamState           设置Sip网关自动推流状态
- /SipGate​/LiveVideo           请求实时视频
- /SipGate​/ByeLiveVideo            停止实时视频预览
- /SipGate​/GetSipDeviceList            获取已注册的设备列表
- /SipGate​/PtzControl          ptz控制
### 系统相关
- /System​/GetGlobleSystemInfo          获取全局的系统信息
- /System​/GetMediaServerInstance           获取一个流媒体服务的实例
- ​/System​/GetMediaServerList          获取流媒体服务器列表
### Test
- 一些测试接口，可以无视
### WebHook
- 用于ZLMediaKit回调的一些接口，可以无视

## TODO List
- 预计全面改用Log4net来记录日志，取消掉原来的Console.WriteLine等记录日志的手段(已完成)
- 增加接口调用的鉴权机制(未开始)
- 考虑SIP网关支持级联到上级平台(未开始)
- 直播推流的完善支持(未开始)
## 更新日志
### 2020-10-12
1. 更新读取ZLMediaKit配置文件时碰到以#开头的配置项时解析出错的情况，会先将此配置文件中所有以#开头的行改成以;开头，以确保以正确的ini标准的配置文件的注释。
2. 调整配置文件位置，system.conf及logconfig.xml到项目的Config/下面。
3. 跨平台方向上的测试与调优。
### 2020-10-10
1. 全面改用Log4Net来记录日志
### 2020-10-09
1. 【支持】StreamNode已经与最新版(2020-10-09)ZLMediaKit兼容，不再需要修改ZLMediaKit的源码了。
### 2020-10-05    
1. 【新增】sip网关收到gb28181设备的设备列表后，自动向Camera表插入这些设备列表作为可推流的设备后选 ，设置激活状态为非激活状态，此类设备需要通过接口进行激活。
2. 【新增】增加/MediaServer/ActivateSipCamera接口，来完成对自动写入数据库的数据进行激活。
3. 【修正】修正一个停止推流的bug。
4. 【修正】修正一个可能存在的，针对于公网非固定ip的gb28181设备的通讯障碍问题（效果有待验证）。


## 结构介绍
- ![StreamNode结构.jpg](https://i.loli.net/2020/09/29/xwkeW8agYspHKUt.jpg)
## Sip网关的工作流程

- 摄像头配置StreamNodeWebApi中Sip网关的相关参数信息
- 摄像头上线时，自动通过GB28181协议向Sip网关注册自己
- Sip网关发现摄像头上线后，发启设备目录查询请求
- 摄像头收到设备目录查询请求后，按GB28181协议要求发送自身设备列表到Sip网关
- Sip网关收到设备列表后，完成摄像头推拉流必须的参数
- 摄像头与Sip网关保持心跳响应
- Sip网关在一定周期内发现摄像头心跳断连，自动跳出Sip设备列表中的摄像头

## StreamNode推拉流的工作流程
- StreamNodeWebApi支持FFmpeg及GB28181方式接入摄像头，其中FFmpeg目前只拉Rtsp视频流
- GB28181协议兼容支持GB28181-2016
- 在接入摄像头前需明确了解摄像头是按Rtsp方式接入还是按GB28181方式接入
- Rtsp方式接入需要正确使用摄像头的Rtsp地址，如：rtsp://username:password@ip:port/main/ch1 
- GB28181方式接入需要在摄像头配置页面配置StreamNodeWebApi中Sip网关的相关参数
- GB28181参数主要有 Sip网关ip地址，Sip网关ID，摄像头ID，摄像头音视频流通道ID等
- GB28181及Rtps方式接入的摄像头音视频编码格式最好使用H.264/ACC,以保证摄像头音视频流正常被ZLMediaKit识别解析，其他支持格式请查看ZLMediakit官方说明
- GB28181方式接入的摄像头在启动时，会尝试连接配置在摄像头中的Sip网关
- Sip网关在收到摄像头的注册请求时会通过GB28181-2016协议与摄头通讯，详见《Sip网关的工作流程》
- StreamNodeWebApi维护了一个定时检查的注册摄像头推拉流状态，如果发现注册摄像头在线并且还未处于推流或拉流状态（没有视频流）的情况下将发启推拉流请求
- Rtsp拉流：当此注册摄像头注册信息中的拉流方式为Rtsp时，StreamNodeWebApi将生成ZLMediakitAddFFmpegProxy请求结构（含摄像头的Rtsp地址），并向ZLMediaKit的AddFFmpegProxy WebApi接口发起拉流请求，ZLMediaKit服务收到AddFFmpegProxy请求后，启动ffmpeg进程，将ffmpeg拉流产生的音视频流推送给流媒体服务器，拉流成功后，ZLMediaKit将会产生OnStreamChange事件，通过Webhook的方式来告知StreamNode(还有其他辅助手段，不再此介绍了)，正常拉流的情况下，可以在操作系统层面看到如下进程信息”/opt/ffmpeg/ffmpeg -re -rtsp_transport tcp -i rtsp://admin:password@192.168.2.165:554/Streaming/Channels/101?transportmode=unicast&profile=Profile_1 -vcodec copy -acodec copy -f flv -y rtmp://127.0.0.1/rtsp_proxy/FDC35E14“
- GB28181请求推流：当此注册摄像头注册信息中的拉流方式为GB28181时，StreamNodeWebApi,将向ZLMediaKit申请rtp动态端口（TCP，UDP全开），端口申请成功获取到实际端口号后，StreamNodeApi根据摄像头注册信息组织实时流请求的Sip报文，并在此时确定流的ssrc值，将生成的Sip请求实时流报文通过Sip通讯通渞发送给摄像头，摄像头收到请求后根据Sip报文内容向ZLMediakit流媒体服务器的Sip报文指定端口以指定ssrc值推送rtp流，ZLMediaKit收到rtp流后对期进行解码，生产Onpublish事件并告知SteramNodeWebApi的Webhook接口，StreamNodeWebApi实现后续的相关处理
- 摄像头的推拉流工作受控与StreamNodeWebApi的摄像头注册信息中的LiveEnable字段，可以通过改变此字段的值（true,false）来实现摄像头音视频流的开关

## StreamNode录制计划
- 可对每个注册摄像头进行录制计划的控制，以控制某时某刻音视频文件录制的启停
- 系统当前仅支持mp4文件的录制
- StreamNodeWebApi，以星期为单位控制每个星期n的00:00:01到23:59:59的录制与不录制控制（可以出现多个星期n的数据，表示每天启用与停用录制可以多段，我记得我好像是这么实现的）
- 如果某个摄像头启用了录制功能，却没有指定星期n的录制计划要求，系统默认其为全天录制
## StreamNode对音视频文件的裁剪与合并操作
- 有时需要提取某个摄像头在某个时间范围的视频，可以通过StreamNodeWebApi的相关接口进行获取
- 此操作是一个耗时操作，因此采用异步回调的方式来获取任务结果，调用方需提供一个WebApi接口来接受任务结果
- 可能因为某些原因造成回调时调用方的WebApi不可用，导致任务结果未收到的情况，系统提供任务状态查询接口供调用方查询，此接口同样适用于任务进度的追踪（StreamMediaServerKeeper被重启后所有之前的任务结果会被清空，因为StreamMediaServerKeeper没有数据库持久保存这些数据）

<details>
  <summary>老版本ZLMediaKit的代码修改请看这里</summary>

## 修改ZLMediaKit的部分代码（ZLMediaKit官方已经在2020-10-09日合并了我的pr,使用2020-10-09以后ZLMeidakit代码生成的可的行文件就不需要再做以下代码修改了）
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

-src/Record/Recorder.cpp
~~~c++
string Recorder::getRecordPath(Recorder::type type, const string &vhost, const string &app, const string &stream_id, const string &customized_path) {
    GET_CONFIG(bool, enableVhost, General::kEnableVhost);
    switch (type) {
        case Recorder::type_hls: {
            GET_CONFIG(string, hlsPath, Hls::kFilePath);
            string m3u8FilePath;
            if (enableVhost) {
                m3u8FilePath = vhost + "/" + app + "/" + stream_id + "/hls.m3u8";
            } else {
                m3u8FilePath = app + "/" + stream_id + "/hls.m3u8";
            }
            //Here we use the customized file path.
            if (!customized_path.empty()) {
                m3u8FilePath = customized_path + "/hls.m3u8";
            }
            return File::absolutePath(m3u8FilePath, hlsPath);
        }
        case Recorder::type_mp4: {
            GET_CONFIG(string, recordPath, Record::kFilePath);
            GET_CONFIG(string, recordAppName, Record::kAppName);
            string mp4FilePath;
            if (enableVhost) {
                mp4FilePath = vhost + "/" + recordAppName + "/" + app + "/" + stream_id + "/";
            } else {
                mp4FilePath = recordAppName + "/" + app + "/" + stream_id + "/";
            }
            //Here we use the customized file path.
            if (!customized_path.empty()) {
                /*开始删除*/
                // mp4FilePath = customized_path + "/";
                /*删除结束*/
                /*开始添加*/
                return  customized_path + "/"+mp4FilePath;
                /*开始添加*/
            }

            return File::absolutePath(mp4FilePath, recordPath);
        }
        default:
            return "";
    }
}
~~~

</details>

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




## StreamNodeWebApi/Config/system.conf
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

## StreamMediaServerKeeper/Config/config.conf 
- StreamMediaServerKeeper的配置文件
~~~
MediaServerBinPath::/root/MediaService/MediaServer;//ZLMediaKit流媒体服务器可执行文件路径
StreamNodeServerUrl::http://192.168.2.43:5800/WebHook/MediaServerRegister; //向哪个StreamNodeWebApi注册自己的服务
HttpPort::6880;//服务的WebApi端口
IpAddress::192.168.2.43;//本机ip地址
CustomizedRecordFilePath::/home/cdtnb; //自定义存储视频的位置 
~~~

## StreamNodeWebApi/Config/logconfig.xml & StreamMediaServerKeeper/Config/logconfig.xml
- 日志配置文件
~~~xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <!-- This section contains the log4net configuration settings -->
    <log4net>
        <appender name="ConsoleAppender" type="log4net.Appender.ConsoleAppender">
            <layout type="log4net.Layout.PatternLayout" value="%date [%thread] %-5level %logger - %message%newline" />
        </appender>
        

        <appender name="RollingLogFileAppender" type="log4net.Appender.RollingFileAppender">
            <file value="log/" />
            <appendToFile value="true" />
            <rollingStyle value="Composite" />
            <staticLogFileName value="false" />
            <datePattern value="yyyyMMdd'.log'" />
            <maxSizeRollBackups value="10" />
            <maximumFileSize value="10MB" />
            <layout type="log4net.Layout.PatternLayout" value="%date [%thread] %-5level %logger - %message%newline" />
        </appender>

        <!-- Setup the root category, add the appenders and set the default level -->
        <root>
            <level value="ALL" />
            <appender-ref ref="ConsoleAppender" />
            <appender-ref ref="RollingLogFileAppender" />
        </root>

    </log4net>
</configuration>
~~~


# 运行
## 环境
- .net core 3.1
- mysql 5.7以上或者其他freesql支持的数据库
- ffmpeg 4.2.2以上
- ffmpeg 需要放在StreamNodeWebApi和StreamMediaServerKeeper的部署目录中
## 启动
- mysql中创建一个名为“straemnode”的数据库，要和StreamNodeWebApi/system.conf中db行指定的一致，字符集请使用utf-8
- StreamNodeWebApi 全局只启动一份
- 部署目录中手工创建一个log文件夹
~~~shell
nohup dotnet StreamNodeWebApi.dll >/dev/null &
~~~
- StreamMediaServerKeeper 一个流媒体启动一份，可以与StreamNodeWebApi不在同一台服务器
- 部署目录中手工创建一个log文件夹
~~~shell
nohup dotnet StreamMediaServerKeeper.dll >/dev/null &
~~~
# 调试
## StreamNodeWebApi
- http://ip:port(5800)/swagger/index.html
## StreamMediaServerKeeper
- http://ip:port(6880)/swagger/index.html
