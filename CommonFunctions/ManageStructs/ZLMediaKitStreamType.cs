using System;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// ZLMediaKit流类型
    /// hls=hls流
    /// rtsp=rtsp流
    /// rtmp=rtmp流
    /// </summary>
    [Serializable]
    public enum ZLMediaKitStreamType
    {
        hls,
        rtsp,
        rtmp,
    }
}