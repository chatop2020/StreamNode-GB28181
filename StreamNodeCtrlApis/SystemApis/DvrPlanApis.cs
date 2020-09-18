using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using TaskStatus = CommonFunctions.ManageStructs.TaskStatus;

namespace StreamNodeCtrlApis.SystemApis
{
    public static class DvrPlanApis
    {
        /// <summary>
        /// 获取需要裁剪合并的文件列表 
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        private static List<CutMergeStruct> analysisVideoFile(ReqCutOrMergeVideoFile rcmv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            int startPos = -1;
            int endPos = -1;
            DateTime _start = DateTime.Parse(rcmv.StartTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(-20); //向前推20秒
            DateTime _end = DateTime.Parse(rcmv.EndTime.ToString("yyyy-MM-dd HH:mm:ss")).AddSeconds(20); //向后延迟20秒
            var videoList = OrmService.Db.Select<RecordFile>()
                .Where(x => x.StartTime > _start.AddMinutes(-60) && x.EndTime <= _end.AddMinutes(60))
                .WhereIf(!string.IsNullOrEmpty(rcmv.MediaServerId),
                    x => x.PushMediaServerId!.Trim().ToLower().Equals(rcmv.MediaServerId!.Trim().ToLower()))
                .WhereIf(!string.IsNullOrEmpty(rcmv.CameraId),
                    x => x.CameraId!.Trim().ToLower().Equals(rcmv.CameraId!.Trim().ToLower()))
              
                .ToList(); //取条件范围的前60分钟及后60分钟内的所有数据

            var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(rcmv.MediaServerId));
            if (mediaObj == null || !mediaObj.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            List<RecordFile> cutMegerList = null!;
            if (videoList != null && videoList.Count > 0)
            {
                for (int i = 0; i <= videoList.Count - 1; i++)
                {
                    if (!mediaObj.CheckFileExists(videoList[i].VideoPath, out _))
                    {
                        continue;
                    }

                    DateTime startInDb =
                        DateTime.Parse(((DateTime) videoList[i].StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    DateTime endInDb =
                        DateTime.Parse(((DateTime) videoList[i].EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                    if (startInDb <= _start && endInDb > _start) //找符合要求的开始视频
                    {
                        startPos = i;
                    }

                    if (startInDb < _end && endInDb >= _end) //找符合要求的结束视频
                    {
                        endPos = i;
                    }
                }

                if (startPos >= 0 && endPos >= 0) //如果开始和结束都找到了，就取这个范围内的视频
                {
                    cutMegerList = videoList.GetRange(startPos, endPos - startPos + 1);
                }

                if (startPos < 0 && endPos >= 0) //如果开始没有找到，而结束找到了
                {
                    List<KeyValuePair<int, double>> tmpStartList = new List<KeyValuePair<int, double>>();
                    for (int i = 0; i <= videoList.Count - 1; i++)
                    {
                        tmpStartList.Add(new KeyValuePair<int, double>(i,
                            Math.Abs(((DateTime) videoList[i]!.StartTime!).Subtract(_start)
                                .TotalMilliseconds))); //对所有视频做开始时间减需的开始时间，取绝对值
                    }

                    tmpStartList.Sort((left, right) => //对相减后的绝对值排序
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int) left.Value == (int) right.Value)
                            return 0;
                        else
                            return -1;
                    });

                    cutMegerList =
                        videoList.GetRange(tmpStartList[0].Key, endPos - tmpStartList[0].Key + 1); //取离要求时间最近的那个视频为开始视频
                    for (int i = cutMegerList.Count - 1; i >= 0; i--)
                    {
                        if (cutMegerList[i].StartTime > _end && cutMegerList[i].EndTime > _end
                        ) //如果视频的开始时间大于要求的结束时间，并且不是最后一个视频，就过滤掉这个视频
                        {
                            if (i > 0)
                            {
                                cutMegerList[i] = null!;
                            }
                        }
                    }

                    Common.RemoveNull(cutMegerList);
                }

                if (startPos >= 0 && endPos < 0) //开始视频找到了，结束视频没有找到
                {
                    List<KeyValuePair<int, double>> tmpEndList = new List<KeyValuePair<int, double>>();

                    for (int i = 0; i <= videoList.Count - 1; i++)
                    {
                        tmpEndList.Add(new KeyValuePair<int, double>(i,
                            Math.Abs(((DateTime) videoList[i]!.EndTime!).Subtract(_end)
                                .TotalMilliseconds))); //上上面一样，取绝对值
                    }

                    tmpEndList.Sort((left, right) => //排序
                    {
                        if (left.Value > right.Value)
                            return 1;
                        else if ((int) left.Value == (int) right.Value)
                            return 0;
                        else
                            return -1;
                    });
                    cutMegerList = videoList.GetRange(startPos, tmpEndList[0].Key - startPos + 1);
                    for (int i = cutMegerList.Count - 1; i >= 0; i--)
                    {
                        if (cutMegerList[i].StartTime > _end && cutMegerList[i].EndTime > _end) //过滤
                        {
                            if (i > 0)
                            {
                                cutMegerList[i] = null!;
                            }
                        }
                    }

                    Common.RemoveNull(cutMegerList);
                }

                if (startPos < 0 && endPos < 0) //如果开始也没找到，结束也没找到，那就报错
                {
                }
            }

            if (cutMegerList != null && cutMegerList.Count > 0) //取到了要合并文件的列表
            {
                List<CutMergeStruct> cutMergeStructList = new List<CutMergeStruct>();
                for (int i = 0; i <= cutMegerList.Count - 1; i++)
                {
                    var tmpCutMeger = cutMegerList[i];
                    if (tmpCutMeger != null && i == 0) //看第一个文件是否需要裁剪
                    {
                        DateTime tmpCutMegerStartTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        DateTime tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        if (tmpCutMegerStartTime < _start && tmpCutMegerEndTime > _start
                        ) //如果视频开始时间大于需要的开始时间，而视频结束时间大于需要的开始时间
                        {
                            TimeSpan ts = -tmpCutMegerStartTime.Subtract(_start); //视频的开始时间减去需要的开始时间，再取反
                            TimeSpan ts2 = tmpCutMegerEndTime.Subtract(_start) + ts; //视频的结束时间减去需要的开始时间，再加上前面的值
                            CutMergeStruct tmpStruct = new CutMergeStruct();
                            tmpStruct.DbId = cutMegerList[i].Id;
                            tmpStruct.Duration = cutMegerList[i].Duration;
                            tmpStruct.EndTime = cutMegerList[i].EndTime;
                            tmpStruct.FilePath = cutMegerList[i].VideoPath;
                            tmpStruct.FileSize = cutMegerList[i].FileSize;
                            tmpStruct.StartTime = cutMegerList[i].StartTime;

                            if (ts2.Hours <= 0 && ts2.Minutes <= 0 && ts2.Seconds <= 0) //如果时间ts2的各项都小于0，说明不需要裁剪
                            {
                                tmpStruct.CutEndPos = "";
                                tmpStruct.CutStartPos = "";
                            }
                            else //否则做裁剪参数设置
                            {
                                tmpStruct.CutEndPos = ts2.Hours.ToString().PadLeft(2, '0') + ":" +
                                                      ts2.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                      ts2.Seconds.ToString().PadLeft(2, '0');
                                tmpStruct.CutStartPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                        ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                        ts.Seconds.ToString().PadLeft(2, '0');
                            }

                            cutMergeStructList.Add(tmpStruct); //加入到处理列表中
                        }
                        else //如果视频时间大于等于需要的开始时间或者大于等于需要的结束时间，时间刚刚正好，直接加进来
                        {
                            CutMergeStruct tmpStruct = new CutMergeStruct()
                            {
                                DbId = cutMegerList[i].Id,
                                CutEndPos = null,
                                CutStartPos = null,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime,
                            };
                            cutMergeStructList.Add(tmpStruct);
                        }
                    }
                    else if (tmpCutMeger != null && i == cutMegerList.Count - 1) //处理最后一个视频，看是否需要裁剪，后续操作同上
                    {
                        DateTime tmpCutMegerStartTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.StartTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        DateTime tmpCutMegerEndTime =
                            DateTime.Parse(((DateTime) tmpCutMeger.EndTime!).ToString("yyyy-MM-dd HH:mm:ss"));
                        if (tmpCutMegerEndTime > _end)
                        {
                            TimeSpan ts = tmpCutMegerEndTime.Subtract(_end);
                            ts = (tmpCutMegerEndTime - tmpCutMegerStartTime).Subtract(ts);
                            CutMergeStruct tmpStruct = new CutMergeStruct();
                            tmpStruct.DbId = cutMegerList[i].Id;
                            tmpStruct.Duration = cutMegerList[i].Duration;
                            tmpStruct.EndTime = cutMegerList[i].EndTime;
                            tmpStruct.FilePath = cutMegerList[i].VideoPath;
                            tmpStruct.FileSize = cutMegerList[i].FileSize;
                            tmpStruct.StartTime = cutMegerList[i].StartTime;
                            if (ts.Hours <= 0 && ts.Minutes <= 0 && ts.Seconds <= 0)
                            {
                                tmpStruct.CutEndPos = "";
                                tmpStruct.CutStartPos = "";
                            }
                            else
                            {
                                tmpStruct.CutEndPos = ts.Hours.ToString().PadLeft(2, '0') + ":" +
                                                      ts.Minutes.ToString().PadLeft(2, '0') + ":" +
                                                      ts.Seconds.ToString().PadLeft(2, '0');
                                tmpStruct.CutStartPos = "00:00:00";
                            }


                            cutMergeStructList.Add(tmpStruct);
                        }
                        else if (tmpCutMegerEndTime <= _end)
                        {
                            CutMergeStruct tmpStruct = new CutMergeStruct()
                            {
                                DbId = cutMegerList[i].Id,
                                CutEndPos = null,
                                CutStartPos = null,
                                Duration = cutMegerList[i].Duration,
                                EndTime = cutMegerList[i].EndTime,
                                FilePath = cutMegerList[i].VideoPath,
                                FileSize = cutMegerList[i].FileSize,
                                StartTime = cutMegerList[i].StartTime,
                            };
                            cutMergeStructList.Add(tmpStruct);
                        }
                    }
                    else //如果不是第一个也不是最后一个，就是中间部分，直接加进列表 
                    {
                        CutMergeStruct tmpStruct = new CutMergeStruct()
                        {
                            DbId = cutMegerList[i].Id,
                            CutEndPos = null,
                            CutStartPos = null,
                            Duration = cutMegerList[i].Duration,
                            EndTime = cutMegerList[i].EndTime,
                            FilePath = cutMegerList[i].VideoPath,
                            FileSize = cutMegerList[i].FileSize,
                            StartTime = cutMegerList[i].StartTime,
                        };
                        cutMergeStructList.Add(tmpStruct);
                    }
                }

                return cutMergeStructList;
            }

            rs = new ResponseStruct() //报错，视频资源没有找到
            {
                Code = ErrorNumber.DvrCutMergeFileNotFound,
                Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeFileNotFound],
            };
            return null!;
        }

        /// <summary>
        /// 裁剪或合并视频文件
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CutMergeTaskResponse CutOrMergeVideoFile(ReqCutOrMergeVideoFile rcmv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (rcmv.StartTime >= rcmv.EndTime)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null!;
            }

            if ((rcmv.EndTime - rcmv.StartTime).Minutes > 120) //超过120分钟不允许执行任务
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.DvrCutMergeTimeLimit,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutMergeTimeLimit],
                };

                return null!;
            }

            if (string.IsNullOrEmpty(rcmv.CallbackUrl) || !Common.IsUrl(rcmv.CallbackUrl!))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };

                return null!;
            }

            //异步回调
            var mergeList = analysisVideoFile(rcmv, out rs);
            if (mergeList != null && mergeList.Count > 0)
            {
                CutMergeTask task = new CutMergeTask()
                {
                    CutMergeFileList = mergeList,
                    CallbakUrl = rcmv.CallbackUrl,
                    CreateTime = DateTime.Now,
                    TaskId = Common.CreateUuid()!,
                    TaskStatus = TaskStatus.Create,
                    ProcessPercentage = 0,
                };
                try
                {
                    var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(rcmv.MediaServerId));
                    if (mediaObj == null || !mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };

                        return null!;
                    }

                    var ret = mediaObj.AddCutOrMergeTask(task, out rs);
                    if (ret == null || rs.Code != ErrorNumber.None)
                    {
                        return null!;
                    }

                    return new CutMergeTaskResponse()
                    {
                        Duration = -1,
                        FilePath = "",
                        FileSize = -1,
                        Status = CutMergeRequestStatus.WaitForCallBack,
                        Task = task,
                        Request = rcmv,
                    };
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct() //报错，队列大于最大值
                    {
                        Code = ErrorNumber.DvrCutProcessQueueLimit,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.DvrCutProcessQueueLimit] + "\r\n" +
                                  ex.Message + "\r\n" + ex.StackTrace,
                    };
                    return null!;
                }
            }

            return null!;
        }


        /// <summary>
        /// 获取裁剪合并任务状态
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CutMergeTaskStatusResponse GetMergeTaskStatus(string mediaServerId,string taskId ,out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (string.IsNullOrEmpty(mediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null;
            }

            var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaObj == null || !mediaObj.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            var ret = mediaObj.GetMergeTaskStatus(taskId,out rs);
            if ( rs.Code == ErrorNumber.None)
            {
                return ret;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }
        
        /// <summary>
        /// 获取裁剪合并任务积压列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<CutMergeTaskStatusResponse> GetBacklogTaskList(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (string.IsNullOrEmpty(mediaServerId))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null;
            }

            var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaObj == null || !mediaObj.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            var retList = mediaObj.GetBacklogTaskList(out rs);
            if ( rs.Code == ErrorNumber.None)
            {
                return retList;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }

        /// <summary>
        /// 通过id删除一个录制计划
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteDvrPlanById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return false;
            }

            List<StreamDvrPlan> retSelect = null!;
            int retDelete = -1;

            retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x => x.Id == id).ToList();
            retDelete = OrmService.Db.Delete<StreamDvrPlan>().Where(x => x.Id == id).ExecuteAffrows();


            if (retDelete > 0)
            {
                foreach (var select in retSelect)
                {
                    OrmService.Db.Delete<DvrDayTimeRange>().Where(x => x.StreamDvrPlanId == select.Id)
                        .ExecuteAffrows();
                }


                return true;
            }


            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            return false;
        }

        /// <summary>
        /// 启用或停止一个录制计划
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enable"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool OnOrOffDvrPlanById(long id, bool enable, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            if (id <= 0)
            {
                rs.Code = ErrorNumber.FunctionInputParamsError;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                return false;
            }


            var retUpdate = OrmService.Db.Update<StreamDvrPlan>().Set(x => x.Enable, enable)
                .Where(x => x.Id == id)
                .ExecuteAffrows();
            if (retUpdate > 0)
                return true;

            rs.Code = ErrorNumber.SrsDvrPlanNotExists;
            rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
            return false;
        }


        public static List<StreamDvrPlan> GetDvrPlanList(ReqGetDvrPlan rgdp, out ResponseStruct rs)
        {
            bool idFound = !string.IsNullOrEmpty(rgdp.MediaServerId);
            bool cameraId = !string.IsNullOrEmpty(rgdp.CameraId);
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };


            /*联同子类一起查出*/
            return OrmService.Db.Select<StreamDvrPlan>().IncludeMany(a => a.TimeRangeList)
                .WhereIf(idFound == true,
                    x => x.MediaServerId.Trim().ToLower().Equals(rgdp.MediaServerId!.Trim().ToLower()))
                .WhereIf(cameraId == true,
                    x => x.CameraId.Trim().ToLower().Equals(rgdp.CameraId!.Trim().ToLower()))
                .ToList();
            /*联同子类一起查出*/
        }


        /// <summary>
        /// 修改dvrplan
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SetDvrPlanById(long id, ReqStreamDvrPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            try
            {
                StreamDvrPlan retSelect = null!;
                int retDelete = -1;

                retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x => x.Id == id).First();
                retDelete = OrmService.Db.Delete<StreamDvrPlan>().Where(x => x.Id == id).ExecuteAffrows();


                if (retDelete > 0)
                {
                    OrmService.Db.Delete<DvrDayTimeRange>()
                        .Where(x => x.StreamDvrPlanId == retSelect.Id).ExecuteAffrows();


                    var retCreate = CreateDvrPlan(sdp, out rs); //创建新的dvr
                    if (retCreate)
                    {
                        return true;
                    }

                    return false;
                }

                rs.Code = ErrorNumber.SrsDvrPlanNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanNotExists];
                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                return false;
            }
        }


        /// <summary>
        /// 创建一个录制计划
        /// </summary>
        /// <param name="sdp"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CreateDvrPlan(ReqStreamDvrPlan sdp, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
            {
                foreach (var timeRange in sdp.TimeRangeList)
                {
                    if (timeRange.StartTime >= timeRange.EndTime)
                    {
                        rs.Code = ErrorNumber.FunctionInputParamsError;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError];
                        return false;
                    }

                    if ((timeRange.EndTime - timeRange.StartTime).TotalSeconds <= 120)
                    {
                        rs.Code = ErrorNumber.SrsDvrPlanTimeLimitExcept;
                        rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanTimeLimitExcept];

                        return false;
                    }
                }
            }

            StreamDvrPlan retSelect = null!;

            retSelect = OrmService.Db.Select<StreamDvrPlan>().Where(x =>
                x.MediaServerId!.Trim().ToLower().Equals(sdp.MediaServerId!.Trim().ToLower())
                && x.CameraId!.Trim().ToLower().Equals(sdp!.CameraId.Trim().ToLower())).First();


            if (retSelect != null)
            {
                rs.Code = ErrorNumber.SrsDvrPlanAlreadyExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SrsDvrPlanAlreadyExists];

                return false;
            }

            try
            {
                StreamDvrPlan tmpStream = new StreamDvrPlan();

                tmpStream.Enable = sdp.Enable;
                tmpStream.MediaServerId = sdp.MediaServerId;
                tmpStream.LimitDays = sdp.LimitDays;
                tmpStream.LimitSpace = sdp.LimitSpace;
                tmpStream.CameraId = sdp.CameraId;
                tmpStream.OverStepPlan = sdp.OverStepPlan;
                tmpStream.TimeRangeList = new List<DvrDayTimeRange>();
                if (sdp.TimeRangeList != null && sdp.TimeRangeList.Count > 0)
                {
                    foreach (var tmp in sdp.TimeRangeList)
                    {
                        tmpStream.TimeRangeList.Add(new DvrDayTimeRange()
                        {
                            EndTime = tmp.EndTime,
                            StartTime = tmp.StartTime,
                            WeekDay = tmp.WeekDay,
                        });
                    }
                }

                /*联同子类一起插入*/
                var repo = OrmService.Db.GetRepository<StreamDvrPlan>();
                repo.DbContextOptions.EnableAddOrUpdateNavigateList = true; //需要手工开启
                var ret = repo.Insert(tmpStream);
                /*联同子类一起插入*/
                if (ret != null)
                {
                    return true;
                }


                return false;
            }
            catch (Exception ex)
            {
                rs.Code = ErrorNumber.SystemDataBaseExcept;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message;

                return false;
            }
        }
    }
}