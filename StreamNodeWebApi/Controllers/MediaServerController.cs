using System;
using System.Collections.Generic;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using LibGB28181SipGate;
using Microsoft.AspNetCore.Mvc;
using StreamNodeCtrlApis.SystemApis;
using Swashbuckle.AspNetCore.Annotations;

namespace StreamNodeWebApi.Controllers
{
    /// <summary>
    /// 流媒体相关接口类
    /// </summary>
    [ApiController]
    [Route("/MediaServer")]
    [SwaggerTag("流媒体相关接口")]
    public class MediaServerController : ControllerBase
    {
        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <returns></returns>
        [Route("CutOrMergeVideoFile")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public CutMergeTaskResponse CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.CutOrMergeVideoFile(rcmv, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取裁剪合并任务状态
        /// </summary>
        /// <returns></returns>
        [Route("GetMergeTaskStatus")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public CutMergeTaskStatusResponse GetMergeTaskStatus(string mediaServerId, string taskId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetMergeTaskStatus(mediaServerId, taskId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取裁剪合并任务积压列表
        /// </summary>
        /// <returns></returns>
        [Route("GetBacklogTaskList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<CutMergeTaskStatusResponse> GetBacklogTaskList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetBacklogTaskList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 恢复被软删除的录像文件
        /// </summary>
        /// <returns></returns>
        [Route("UndoSoftDelete")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool UndoSoftDelete(long dvrVideoId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.UndoSoftDelete(dvrVideoId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 删除一个录像文件ById（硬删除，立即删除文件，数据库做delete标记）
        /// </summary>
        /// <returns></returns>
        [Route("HardDeleteDvrVideoById")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool HardDeleteDvrVideoById(long dvrVideoId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.HardDeleteDvrVideoById(dvrVideoId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 删除一批录像文件ById（硬删除，立即删除文件，数据库做delete标记）
        /// </summary>
        /// <returns></returns>
        [Route("HardDeleteDvrVideoByIdList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool HardDeleteDvrVideoByIdList(List<long> dvrVideoId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.HardDeleteDvrVideoByIdList(dvrVideoId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 删除一个录像文件ById（软删除，只做标记，不删除文件，文件在24小时后删除）
        /// </summary>
        /// <returns></returns>
        [Route("SoftDeleteDvrVideoById")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool SoftDeleteDvrVideoById(long dvrVideoId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.SoftDeleteDvrVideoById(dvrVideoId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 获取录像文件(条件灵活)
        /// </summary>
        /// <returns></returns>
        [Route("GetDvrVideoList")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public DvrVideoResponseList GetDvrVideoList(ReqGetDvrVideo rgdv)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetDvrVideoList(rgdv, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取摄像头实例列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetCameraInstanceList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<CameraInstance> GetCameraInstanceList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetCameraInstanceList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 修改一个注册摄像头实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("ModifyCameraInstance")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public CameraInstance ModifyCameraInstance(string mediaServerId, ReqMoidfyCameraInstance req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.ModifyCameraInstance(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 删除一个摄像头实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="cameraId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("DeleteCameraInstance")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool DeleteCameraInstance(string mediaServerId, string cameraId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.DeleteCameraInstance(mediaServerId, cameraId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 注册添加一个摄像头实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("AddCameraInstance")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public CameraInstance AddCameraInstance(string mediaServerId, ReqAddCameraInstance req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.AddCameraInstance(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取在线播放器列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetPlayerSessionList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<PlayerSession> GetPlayerSessionList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetPlayerSessionList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            foreach (var obj in ret)
            {
                obj.UpTime = (DateTime.Now - obj.OnlineTime).TotalSeconds;
            }

            return ret;
        }

        /// <summary>
        /// 获取在线摄像头列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetCameraSessionList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<CameraSession> GetCameraSessionList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetCameraSessionList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            ret = ret.FindAll(x => x.IsOnline == true);
            foreach (var obj in ret)
            {
                obj.UpTime = (DateTime.Now - obj.OnlineTime).TotalSeconds;
            }

            return ret;
        }
        /*/// <summary>
        /// 获取客户端列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetOnlineClientSessionList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public List<OnlineClientSession> GetOnlineClientSessionList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetOnlineClientSessionList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }*/


        /// <summary>
        /// 获取流媒体配置信息
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetConfig")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResZLMediaKitConfig GetConfig(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetConfig(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 启动一个ffmpeg代理流
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="src_url"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("AddFFmpegProxy")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResZLMediaKitAddFFmpegProxy AddFFmpegProxy(string mediaServerId, string src_url)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.AddFFmpegProxy(mediaServerId, src_url, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 关闭一个流
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("CloseStreams")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitCloseStreams CloseStreams(string mediaServerId, ReqZLMediaKitCloseStreams req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.CloseStreams(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 获取流列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetStreamList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResZLMediaKitMediaList GetStreamList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetStreamList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 启动流的录制
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StartRecord")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitStartStopRecord StartRecord(string mediaServerId, ReqZLMediaKitStartRecord req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.StartRecord(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 停止流的录制
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StopRecord")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitStartStopRecord StopRecord(string mediaServerId, ReqZLMediaKitStopRecord req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.StopRecord(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取流的录制状态
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetRecordStatus")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitIsRecord GetRecordStatus(string mediaServerId, ReqZLMediaKitStopRecord req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetRecordStatus(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 打开某个rtp端口
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("OpenRtpPort")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitOpenRtpPort OpenRtpPort(string mediaServerId, ReqZLMediaKitOpenRtpPort req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.OpenRtpPort(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 关闭某个rtp端口
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("CloseRtpPort")]
        [HttpPost]
        [Log]
        [AuthVerify]
        public ResZLMediaKitCloseRtpPort CloseRtpPort(string mediaServerId, ReqZLMediaKitCloseRtpPort req)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.CloseRtpPort(mediaServerId, req, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 获取流媒体已经开放的rtp端口列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("GetRtpPortList")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public ResZLMediaKitRtpPortList GetRtpPortList(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.GetRtpPortList(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 检查流媒体服务是否正在运行
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("CheckMediaServerRunning")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool CheckMediaServerRunning(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.CheckMediaServerIsRunning(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 重启流媒体服务
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("RestartMediaServer")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool RestartMediaServer(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.RestartMediaServer(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 关闭流媒体服务
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StopMediaServer")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool StopMediaServer(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.StopMediaServer(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 启动流媒体服务
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StartMediaServer")]
        [HttpGet]
        [Log]
        [AuthVerify]
        public bool StartMediaServer(string mediaServerId)
        {
            ResponseStruct rs;
            var ret = MediaServerApis.StartMediaServer(mediaServerId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
    }
}