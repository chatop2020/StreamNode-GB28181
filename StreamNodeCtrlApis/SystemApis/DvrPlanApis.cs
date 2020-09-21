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