using System;
using GB28181.Net;

namespace GB28181.Servers
{
    public interface IMediaAction
    {
        /// <summary>
        /// 实时视频请求
        /// </summary>
        /// <param name="streamid"></param>
        /// <param name="needResult"></param>
        /// <returns></returns>
        int RealVideoReq(uint streamid, out string callid, out string serverip, out int serverport,
            bool needResult = true);

        /// <summary>
        /// 实时视频请求，动态生成 rtp端口
        /// </summary>
        /// <param name="streamid"></param>
        /// <param name="callid"></param>
        /// <param name="serverip"></param>
        /// <param name="serverport"></param>
        /// <param name="needResult"></param>
        /// <returns></returns>
        int RealVideoReq(uint streamid, out string callid, string rtpServerIp, ushort rtpServerPort,
            bool needResult = true, bool tcp = false);


        /// <summary>
        /// 实时视频请求
        /// </summary>
        void RealVideoReq();

        /// <summary>
        /// 实时视频请求
        /// </summary>
        int RealVideoReq(int[] mediaPort, string receiveIP, bool needResult);

        //if an operation need Result you can wait the Result by WaitRequestResult
        Tuple<SIPRequest, SIPResponse> WaitRequestResult();

        /// <summary>
        /// 取消实时视频请求
        /// </summary>
        void ByeVideoReq(out string _callid, bool needResult = false);

        void ByeVideoReq(string sessionid);

        /// <summary>
        /// 确认接收实时视频请求
        /// </summary>
        /// <param name="toTag">ToTag</param>
        /// <returns>sip请求</returns>
        void AckRequest(SIPResponse response);


        /// <summary>
        /// 视频流回调完成
        /// </summary>
        event Action<RTPFrame> OnStreamReady;


        #region 录像点播

        /// <summary>
        /// 录像文件查询结果
        /// </summary>
        /// <param name="recordTotal">录像条数</param>
        void RecordQueryTotal(int recordTotal);

        /// <summary>
        /// 录像文件检索
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// </summary>
        int RecordFileQuery(DateTime beginTime, DateTime endTime, string type);

        /// <summary>
        /// 录像点播视频请求
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        void BackVideoReq(DateTime beginTime, DateTime endTime);

        int BackVideoReq(ulong beginTime, ulong endTime, int[] mediaPort, string receiveIP, bool needResult = false);

        /// <summary>
        /// 录像文件下载请求
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        void VideoDownloadReq(DateTime beginTime, DateTime endTime);

        int VideoDownloadReq(DateTime beginTime, DateTime endTime, int[] mediaPort, string receiveIP,
            bool needResult = false);

        /// <summary>
        /// 录像点播视频播放速度控制请求
        /// </summary>
        /// <param name="scale">播放快进比例</param>
        /// <param name="range">视频播放时间段</param>
        bool BackVideoPlaySpeedControlReq(string range, bool needResult = false);

        bool BackVideoPlaySpeedControlReq(string sessionid, float scale, bool needResult = false);

        /// <summary>
        /// 控制录像随机拖拽
        /// </summary>
        /// <param name="range">时间范围</param>
        bool BackVideoPlayPositionControlReq(int range, bool needResult = false);

        bool BackVideoPlayPositionControlReq(string sessionid, long time, bool needResult = false);

        /// <summary>
        /// 录像点播视频继续播放控制请求
        /// </summary>
        void BackVideoContinuePlayingControlReq(bool needResult = false);

        bool BackVideoContinuePlayingControlReq(string sessionid, bool needResult = false);

        /// <summary>
        /// 录像点播视频暂停控制请求
        /// </summary>
        void BackVideoPauseControlReq(bool needResult = false);

        bool BackVideoPauseControlReq(string sessionid, bool needResult = false);

        /// <summary>
        /// 录像点播视频停止播放控制请求
        /// </summary>
        void BackVideoStopPlayingControlReq(bool needResult = false);

        bool BackVideoStopPlayingControlReq(string sessionid);

        #endregion
    }
}