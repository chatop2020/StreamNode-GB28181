using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using StreamNodeCtrlApis.SystemApis;

namespace StreamNodeWebApi.AutoTasker
{
    /// <summary>
    /// 录制计划控制
    /// </summary>
    public class RecordAutoKeeper
    {
        /// <summary>
        /// 清除24小时前软删除的文件
        /// </summary>
        /// <param name="sdp"></param>
        private void doDeleteFor24HourAgo(StreamDvrPlan sdp)
        {
            try
            {
                List<RecordFile> retList = null!;
                retList = OrmService.Db.Select<RecordFile>()
                    .WhereIf(!string.IsNullOrEmpty(sdp.MediaServerId), x =>
                        x.PushMediaServerId!.Trim().ToLower().Equals(sdp.MediaServerId!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(sdp.CameraId), x =>
                        x.CameraId!.Trim().ToLower().Equals(sdp.CameraId!.Trim().ToLower()))
                    .Where(x => x.Deleted == true)
                    .Where(x => ((DateTime) x.UpdateTime!).AddHours(24) <= DateTime.Now)
                    .ToList();


                if (retList != null && retList.Count > 0)
                {
                    var deleteFileList = retList.Select(x => x.VideoPath).ToList();
                    var deleteFileIdList = retList.Select(x => x.Id).ToList();

                    var mediaServer =
                        Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retList[0].PushMediaServerId));
                    if (mediaServer != null && mediaServer.IsRunning)
                    {
                        var delRet = mediaServer.DeleteFileList(deleteFileList, out _);

                        if (delRet)
                        {
                            var a = OrmService.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                                .Set(x => x.Undo, false)
                                .Where(x => deleteFileIdList.Contains(x.Id)).ExecuteAffrows();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Logger.Error(ex.Message + "->" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取记录日期列表
        /// </summary>
        /// <param name="plan"></param>
        /// <returns></returns>
        private List<string> getDvrPlanFileDataList(StreamDvrPlan plan)
        {
            List<string?> ret = null!;
            ret = OrmService.Db.Select<RecordFile>()
                .WhereIf(!string.IsNullOrEmpty(plan.MediaServerId), x =>
                    x.PushMediaServerId!.Trim().ToLower().Equals(plan.MediaServerId!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(plan.CameraId), x =>
                    x.CameraId!.Trim().ToLower().Equals(plan.CameraId!.Trim().ToLower()))
                .Where(x => x.Deleted == false)
                .GroupBy(x => x.RecordDate)
                .OrderBy(x => x.Value.RecordDate)
                .ToList(a => a.Value.RecordDate);


            if (ret != null && ret.Count > 0)
            {
                return ret!;
            }

            return null!;
        }

        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns></returns>
        private decimal getDvrPlanFileSize(StreamDvrPlan sdp)
        {
            try
            {
                return OrmService.Db.Select<RecordFile>()
                    .WhereIf(!string.IsNullOrEmpty(sdp.MediaServerId), x =>
                        x.PushMediaServerId!.Trim().ToLower().Equals(sdp.MediaServerId!.Trim().ToLower()))
                    .WhereIf(!string.IsNullOrEmpty(sdp.CameraId), x =>
                        x.CameraId!.Trim().ToLower().Equals(sdp.CameraId!.Trim().ToLower()))
                    .Where(x => x.Deleted == false)
                    .Sum(x => x.FileSize);
            }
            catch (Exception ex)
            {
                Logger.Logger.Error(ex.Message + "->" + ex.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// 一天一天删除文件
        /// </summary>
        /// <param name="days"></param>
        /// <param name="sdp"></param>
        private void DeleteFileByDay(List<string> days, StreamDvrPlan sdp)
        {
            var mediaServer =
                Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(sdp.MediaServerId));
            if (mediaServer == null || !mediaServer.IsRunning)
            {
                return;
            }

            foreach (var day in days)
            {
                var deleteList = OrmService.Db.Select<RecordFile>().Where(x => x.RecordDate == day).ToList();
                if (deleteList != null && deleteList.Count > 0)
                {
                    var deleteFileList = deleteList.Select(x => x.VideoPath).ToList();

                    OrmService.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                        .Set(x => x.Deleted, true)
                        .Where(x => x.RecordDate == day).ExecuteAffrows();
                    mediaServer.DeleteFileList(deleteFileList, out _);
                    Logger.Logger.Info(sdp.MediaServerId+"->"+sdp.CameraId+"->"+"要删除除一天的文件，数据库标记为删除 -> " + day!);
                    // LogWriter.WriteLog("要删除除一天的文件，数据库标记为删除", day!);
                }

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 文件一个一个删除
        /// </summary>
        /// <param name="videoSize"></param>
        /// <param name="sdp"></param>
        private void deleteFileOneByOne(decimal videoSize, StreamDvrPlan sdp)
        {
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(sdp.MediaServerId));
            if (mediaServer == null || !mediaServer.IsRunning)
            {
                return;
            }

            long deleteSize = 0;
            List<OrderByStruct> orderBy = new List<OrderByStruct>();
            orderBy.Add(new OrderByStruct()
            {
                FieldName = "starttime",
                OrderByDir = OrderByDir.ASC,
            });
            ReqGetDvrVideo rgdv = new ReqGetDvrVideo()
            {
                CameraId = sdp.CameraId,
                MediaServerId = sdp.MediaServerId,
                EndTime = null,
                IncludeDeleted = false,
                OrderBy = orderBy,
                PageIndex = 1,
                PageSize = 10,
                StartTime = null,
            };
            while (videoSize - deleteSize > sdp.LimitSpace)
            {
                DvrVideoResponseList videoList = MediaServerApis.GetDvrVideoList(rgdv, out ResponseStruct rs);
                if (videoList != null && videoList.DvrVideoList != null && videoList.DvrVideoList.Count > 0)
                {
                    OrmService.Db.Transaction(() =>
                    {
                        foreach (var ret in videoList.DvrVideoList)
                        {
                            if (ret != null)
                            {
                                if (mediaServer.CheckFileExists(ret.VideoPath, out _))
                                {
                                    mediaServer.DeleteFile(ret.VideoPath, out _);
                                    deleteSize += (long) ret.FileSize!;
                                    OrmService.Db.Update<RecordFile>().Set(x => x.UpdateTime, DateTime.Now)
                                        .Set(x => x.Deleted, true)
                                        .Where(x => x.Id == ret!.Id).ExecuteAffrows();
                                    Logger.Logger.Info(sdp.MediaServerId+"->"+sdp.CameraId+"->"+"删除录制文件 -> " + ret.VideoPath!);
                                    Thread.Sleep(20);
                                }
                            }

                            if ((videoSize - deleteSize) < sdp.LimitSpace)
                            {
                                break;
                            }
                        }
                    });
                }
            }
        }


        /// <summary>
        /// 执行删除指令
        /// </summary>
        /// <param name="sdp"></param>
        private void ExecDelete(StreamDvrPlan sdp)
        {
            doDeleteFor24HourAgo(sdp); //删除24小时过期的软删除文件

            if (sdp.OverStepPlan == OverStepPlan.DeleteFile)
            {
                if (sdp.LimitDays > 0) //处理有时间限制的
                {
                    List<string?> dateList = null!;
                    dateList = getDvrPlanFileDataList(sdp)!;
                    if (dateList != null)
                    {
                        Common.RemoveNull(dateList);
                    }

                    if (dateList != null && dateList.Count > sdp.LimitDays)
                    {
                        //执行一天一天删除
                        int? loopCount = dateList.Count - sdp.LimitDays;

                        List<string> willDeleteDays = new List<string>();
                        for (int i = 0; i < loopCount; i++)
                        {
                            willDeleteDays.Add(dateList[i]!);
                        }

                        DeleteFileByDay(willDeleteDays, sdp);
                    }
                }

                if (sdp.LimitSpace > 0) //处理有容量限制的情况
                {
                    decimal videoSize = getDvrPlanFileSize(sdp);

                    if (videoSize > sdp.LimitSpace)
                    {
                        //一个一个删除文件
                        deleteFileOneByOne(videoSize, sdp);
                    }
                }
            }
        }

        /// <summary>
        /// 检查时间范围
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private bool isTimeRange(DvrDayTimeRange d)
        {
            TimeSpan nowDt = DateTime.Now.TimeOfDay;
            string start = d.StartTime.ToString("HH:mm:ss");
            string end = d.EndTime.ToString("HH:mm:ss");
            TimeSpan workStartDt = DateTime.Parse(start).TimeOfDay;
            TimeSpan workEndDt = DateTime.Parse(end).TimeOfDay;
            if (nowDt > workStartDt && nowDt < workEndDt)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 检查是否在时间范围内
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns></returns>
        private bool checkTimeRange(StreamDvrPlan sdp)
        {
            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                bool haveFalse = false;
                foreach (var sdpTimeRange in sdp.TimeRangeList)
                {
                    
                    if ( sdpTimeRange!=null && sdpTimeRange.WeekDay == DateTime.Now.DayOfWeek && isTimeRange(sdpTimeRange))
                    {
                        return true;//有当天计划并在时间反问内返回true
                    }

                    if (sdpTimeRange != null && sdpTimeRange.WeekDay == DateTime.Now.DayOfWeek &&
                        !isTimeRange(sdpTimeRange))
                    {
                        haveFalse = true;//当天计划存在，但不在范围，先做个标记，因为也许会有多个星期n的情况
                    }
                   
                }
                if (haveFalse) 
                {
                    return false;//如果循环以外，haveFalse为true,说明真的不在范围内
                }
              
                /*var t = sdp.TimeRangeList.FindLast(x => x.WeekDay == DateTime.Now.DayOfWeek);
                if (t != null && isTimeRange(t)) //有当天计划并在时间反问内返回true
                {
                    return true;
                }

                if (t != null && !isTimeRange(t)) //只有设置当天计划并且不在当天计划时间内，返回false
                {
                    return false;
                }

                return true; //如果没有设置当天计划也返回true*/
            }

            return true; //如果是空的，就直接返回可运行
        }

        /// <summary>
        /// 获取录制状态
        /// </summary>
        /// <param name="sdp"></param>
        /// <returns></returns>
        private bool getDvrOnorOff(StreamDvrPlan sdp)
        {
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(sdp.MediaServerId));
            if (mediaServer != null && mediaServer.IsRunning)
            {
                var sessionList = MediaServerApis.GetCameraSessionList(mediaServer.MediaServerId, out _);
                if (sessionList != null)
                {
                    var obj = sessionList.FindLast(x => x.CameraId!.Equals(sdp.CameraId));
                    if (obj != null)
                    {
                        var ret = mediaServer.WebApi.GetRecordStatus(new ReqZLMediaKitStopRecord()
                        {
                            App = obj.App,
                            Secret = "",
                            Stream = obj.StreamId,
                            Vhost = obj.Vhost,
                        }, out _);
                        if (ret.Code == 0 && ret.Status != null)
                        {
                            try
                            {
                                lock (Common.CameraSessionLock)
                                {
                                    Common.CameraSessions.FindLast(x => x.CameraId!.Equals(obj.CameraId)
                                                                        && x.MediaServerId!.Equals(obj.MediaServerId))!
                                        .IsRecord = ret.Status;
                                }
                            }
                            catch
                            {
                                //
                            }

                            return (bool) ret.Status;
                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 设置是否启用录制
        /// </summary>
        /// <param name="sdp"></param>
        /// <param name="enable"></param>
        private void setDvrOnorOff(StreamDvrPlan? sdp, bool enable)
        {
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(sdp.MediaServerId));
            if (mediaServer != null && mediaServer.IsRunning)
            {
                var sessionList = MediaServerApis.GetCameraSessionList(mediaServer.MediaServerId, out _);
                if (sessionList != null)
                {
                    var obj = sessionList.FindLast(x => x.CameraId!.Equals(sdp!.CameraId));
                    if (obj != null)
                    {
                        if (enable)
                        {
                            mediaServer.WebApi.StartRecord(new ReqZLMediaKitStartRecord()
                            {
                                App = obj.App,
                                Secret = "",
                                Stream = obj.StreamId,
                                Vhost = obj.Vhost,
                                Customized_Path = mediaServer.RecordFilePath,
                            }, out _);
                        }
                        else
                        {
                            mediaServer.WebApi.StopRecord(new ReqZLMediaKitStopRecord()
                            {
                                App = obj.App,
                                Secret = "",
                                Stream = obj.StreamId,
                                Vhost = obj.Vhost,
                            }, out _);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 执行启动和关闭指令
        /// </summary>
        /// <param name="sdp">参数</param>
        private void execOnOrOff(StreamDvrPlan sdp)
        {
            bool isEnable = true;
            int dateCount = 0;
            decimal videoSize = 0;
            List<string?> dateList = null!;
            videoSize = getDvrPlanFileSize(sdp)!;
            dateList = getDvrPlanFileDataList(sdp)!;
            if (dateList != null && dateList.Count > 0)
            {
                Common.RemoveNull(dateList);
                dateCount = dateList.Count;
            }
            else
            {
                dateCount = 0;
            }

            if (sdp.OverStepPlan == OverStepPlan.StopDvr)
            {
                if (sdp.LimitDays > 0) //处理有天数限制的情况
                {
                    if (sdp.LimitDays < dateCount)
                    {
                        //停掉
                        isEnable = false;
                    }
                }

                if (sdp.LimitSpace > 0) //处理有天数限制的情况
                {
                    if (videoSize > sdp.LimitSpace)
                    {
                        //停掉
                        isEnable = false;
                    }
                }
            }

            bool isTime = checkTimeRange(sdp);
            isEnable = isEnable && sdp.Enable; //要处理计划停用的状态


            if (isTime && isEnable)
            {
                if (!getDvrOnorOff(sdp))
                {
                    Logger.Logger.Info("录制计划即将启动录制,因为视频流没有达到受限条件，已经进入计划规定时间内并且录制程序处于关闭状态 -> " +
                                       sdp.MediaServerId + "->" +
                                       sdp.CameraId + "->" +
                                       sdp.CameraId +
                                       "\t" + "空间限制：" +
                                       sdp.LimitSpace.ToString() +
                                       "字节::实际空间占用：" +
                                       videoSize.ToString() +
                                       "字节 \t时间限制：" +
                                       sdp.LimitDays.ToString() +
                                       "天::实际录制天数：" +
                                       dateCount.ToString() +
                                       "\t录制计划启用状态:" +
                                       sdp.Enable.ToString());
                    /*LogWriter.WriteLog("录制计划即将启动录制,因为视频流没有达到受限条件，已经进入计划规定时间内并且录制程序处于关闭状态",
                        sdp.MediaServerId + "->" +
                        sdp.CameraId + "->" +
                        sdp.CameraId +
                        "\t" + "空间限制：" +
                        sdp.LimitSpace.ToString() +
                        "字节::实际空间占用：" +
                        videoSize.ToString() +
                        "字节 \t时间限制：" +
                        sdp.LimitDays.ToString() +
                        "天::实际录制天数：" +
                        dateCount.ToString() +
                        "\t录制计划启用状态:" +
                        sdp.Enable.ToString());*/
                    setDvrOnorOff(sdp, true);
                }
            }
            else
            {
                if (getDvrOnorOff(sdp))
                {
                    Logger.Logger.Info("录制计划即将关闭录制,因为视频流可能达到受限条件或者已经离开计划规定时间内并且录制程序处于启动状态 -> " +
                                       sdp.MediaServerId + "->" +
                                       sdp.CameraId + "->" +
                                       "\t" + "空间限制：" +
                                       sdp.LimitSpace.ToString() +
                                       "字节::实际空间占用：" +
                                       videoSize.ToString() +
                                       "字节 \t时间限制：" +
                                       sdp.LimitDays.ToString() +
                                       "天::实际录制天数：" +
                                       dateCount.ToString() +
                                       "\t录制计划启用状态:" +
                                       sdp.Enable.ToString());
                    /*LogWriter.WriteLog("录制计划即将关闭录制,因为视频流可能达到受限条件或者已经离开计划规定时间内并且录制程序处于启动状态",
                        sdp.MediaServerId + "->" +
                        sdp.CameraId + "->" +
                        "\t" + "空间限制：" +
                        sdp.LimitSpace.ToString() +
                        "字节::实际空间占用：" +
                        videoSize.ToString() +
                        "字节 \t时间限制：" +
                        sdp.LimitDays.ToString() +
                        "天::实际录制天数：" +
                        dateCount.ToString() +
                        "\t录制计划启用状态:" +
                        sdp.Enable.ToString());*/
                    setDvrOnorOff(sdp, false);
                }
            }
        }

        private bool getCameraSessionStatus(string mediaServerId, string cameraId)
        {
            lock (Common.CameraSessionLock)
            {
                var session = Common.CameraSessions.FindLast(x => x.MediaServerId!.Equals(mediaServerId)
                                                                  && x.CameraId!.Equals(cameraId));
                if (session != null)
                {
                    if (session.IsOnline != null)
                    {
                        return (bool) session.IsOnline;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        private void Run()
        {
            int i = 0;
            while (true)
            {
                try
                {
                    i++;
                    if (Common.MediaServerList == null || Common.MediaServerList.Count <= 0)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    foreach (var mediaServer in Common.MediaServerList)
                    {
                        if (mediaServer != null && mediaServer.IsRunning)
                        {
                            if (i % 50 == 0)
                            {
                                mediaServer.ClearNoFileDir(out _); //清除空目录
                            }

                            ReqGetDvrPlan rgdp = new ReqGetDvrPlan();
                            rgdp.MediaServerId = mediaServer.MediaServerId;
                            var dvrPlanList = DvrPlanApis.GetDvrPlanList(rgdp, out ResponseStruct rs);
                            if (dvrPlanList == null || dvrPlanList.Count == 0) continue;
                            foreach (var dvrPlan in dvrPlanList)
                            {
                                if (dvrPlan.Enable == false)
                                {
                                    bool ret = true;
                                    ret = getDvrOnorOff(
                                        dvrPlan!); //如果录制计划为停止状态，在处理下一个计划任务前要查看该录制计划是否正在执行，正在扫行的话，要停掉它

                                    if (ret)
                                    {
                                        setDvrOnorOff(dvrPlan, false);
                                    }

                                    continue;
                                }

                                CameraInstance? camera = null;
                                lock (Common.CameraInstanceList)
                                {
                                    camera =
                                        Common.CameraInstanceList.FindLast(x =>
                                            x.CameraId.Equals(dvrPlan.CameraId));
                                }

                                if (camera != null)
                                {
                                    ExecDelete(dvrPlan);
                                    if (camera.EnableLive &&
                                        getCameraSessionStatus(camera.PushMediaServerId, camera.CameraId))
                                    {
                                        execOnOrOff(dvrPlan);
                                    }
                                }

                                Thread.Sleep(2000);
                            }
                        }
                    }

                    Thread.Sleep(5000);
                }
                catch (Exception ex)
                {
                    Logger.Logger.Error("报错了->" + ex.Message + "->" + ex.StackTrace);
                    continue;
                }
            }
        }

        /// <summary>
        /// 录制计划的自动化
        /// </summary>
        public RecordAutoKeeper()
        {
            new Thread(new ThreadStart(delegate

            {
                try
                {
                    Run();
                }
                catch
                {
                    //
                }
            })).Start();
        }
    }
}