using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonFunction.ManageStructs;
using CommonFunctions;
using CommonFunctions.DBStructs;
using CommonFunctions.ManageStructs;
using CommonFunctions.MediaServerControl;
using CommonFunctions.WebApiStructs.Request;
using CommonFunctions.WebApiStructs.Response;
using GB28181.Servers.SIPMonitor;
using GB28181.Sys.Model;
using LibGB28181SipGate;

namespace StreamNodeCtrlApis.SystemApis
{
    public static class MediaServerApis
    {
        /// <summary>
        /// 恢复被软删除的录制文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool UndoSoftDelete(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            RecordFile retSelect = null!;

            retSelect = OrmService.Db.Select<RecordFile>().Where(x => x.Id == id).First();
            if (retSelect == null)
            {
                rs.Code = ErrorNumber.SystemDataBaseRecordNotExists;
                rs.Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseRecordNotExists];
                return false;
            }

            var mediaServer =
                Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retSelect.PushMediaServerId));
            if (mediaServer != null && mediaServer.IsRunning)
            {
                if (!mediaServer.CheckFileExists(retSelect.VideoPath, out _))
                {
                    rs.Code = ErrorNumber.DvrVideoFileNotExists;
                    rs.Message = ErrorMessage.ErrorDic![ErrorNumber.DvrVideoFileNotExists];
                    return false;
                }
            }

            var retUpdate = OrmService.Db.Update<RecordFile>().Set(x => x.Deleted, false)
                .Set(x => x.UpdateTime, DateTime.Now)
                .Set(x => x.Undo, false).Where(x => x.Id == (long) id).ExecuteAffrows();
            if (retUpdate > 0)
            {
                return true;
            }


            return false;
        }


        /// <summary>
        /// 删除一个录像文件（硬删除，立即删除文件，数据库置Delete）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool HardDeleteDvrVideoById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            List<RecordFile> retSelect = null!;
            int retUpdate = -1;

            retSelect = OrmService.Db.Select<RecordFile>().Where(x => x.Id == id).ToList();
            if (retSelect != null && retSelect.Count > 0)
            {
                var mediaServer =
                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retSelect[0].PushMediaServerId));
                if (mediaServer != null && mediaServer.IsRunning)
                {
                    retUpdate = OrmService.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, false)
                        .Set(x => x.UpdateTime, DateTime.Now).Where(x => x.Id == (long) id).ExecuteAffrows();
                    if (retUpdate > 0)
                    {
                        foreach (var select in retSelect)
                        {
                            var ret = mediaServer.DeleteFile(select.VideoPath, out _);
                            return ret;
                        }
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitNotRunning,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                    };
                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return false;
        }


        /// <summary>
        /// 删除一批录像文件（硬删除，立即删除文件，数据库置Delete）
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool HardDeleteDvrVideoByIdList(List<long> idList, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            List<RecordFile> retSelect = null!;
            int retUpdate = -1;
            retSelect = OrmService.Db.Select<RecordFile>().Where(x => idList.Contains(x.Id)).ToList();
            if (retSelect != null && retSelect.Count > 0)
            {
                var mediaServer =
                    Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(retSelect[0].PushMediaServerId));
                if (mediaServer != null && mediaServer.IsRunning)
                {
                    retUpdate = OrmService.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                        .Set(x => x.Undo, false)
                        .Set(x => x.UpdateTime, DateTime.Now).Where(x => idList.Contains(x.Id)).ExecuteAffrows();
                    if (retUpdate > 0)
                    {
                        var deleteFileList = retSelect.Select(x => x.VideoPath).ToList();
                        var ret = mediaServer.DeleteFileList(deleteFileList, out _);
                        return ret;
                    }
                }
                else
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.ZLMediaKitNotRunning,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                    };
                    return false;
                }
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return false;
        }

        /// <summary>
        /// 删除一个录像文件（软删除，只做标记不删除文件，文件保留24小时后删除）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool SoftDeleteDvrVideoById(long id, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            var retUpdate = OrmService.Db.Update<RecordFile>().Set(x => x.Deleted, true)
                .Set(x => x.UpdateTime, DateTime.Now)
                .Set(x => x.Undo, true).Where(x => x.Id == (long) id).ExecuteAffrows();
            if (retUpdate > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取录像文件列表
        /// </summary>
        /// <param name="rgdv"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static DvrVideoResponseList GetDvrVideoList(ReqGetDvrVideo rgdv, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            bool idFound = !string.IsNullOrEmpty(rgdv.MediaServerId);
            bool cameraIdFound = !string.IsNullOrEmpty(rgdv.CameraId);
            bool isPageQuery = (rgdv.PageIndex != null && rgdv.PageIndex >= 1);
            bool haveOrderBy = rgdv.OrderBy != null;
            if (isPageQuery)
            {
                if (rgdv.PageSize > 10000)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    return null!;
                }

                if (rgdv.PageIndex <= 0)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.SystemDataBaseLimited,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseLimited],
                    };
                    return null!;
                }
            }

            string orderBy = "";
            if (haveOrderBy)
            {
                foreach (var order in rgdv.OrderBy!)
                {
                    if (order != null)
                    {
                        orderBy += order.FieldName + " " + Enum.GetName(typeof(OrderByDir), order.OrderByDir!) + ",";
                    }
                }

                orderBy = orderBy.TrimEnd(',');
            }

            long total = -1;
            List<RecordFile> retList = null!;

            if (!isPageQuery)
            {
                retList = OrmService.Db.Select<RecordFile>().Where("1=1")
                    .WhereIf(idFound,
                        x => x.PushMediaServerId!.Trim().ToLower().Equals(rgdv.MediaServerId!.Trim().ToLower()))
                    .WhereIf(cameraIdFound, x => x.CameraId!.Trim().ToLower().Equals(rgdv.CameraId!.Trim().ToLower()))
                    .WhereIf(rgdv.StartTime != null, x => x.StartTime >= rgdv.StartTime)
                    .WhereIf(rgdv.EndTime != null, x => x.EndTime <= rgdv.EndTime)
                    .WhereIf(!(bool) rgdv.IncludeDeleted!, x => x.Deleted == false)
                    .OrderBy(orderBy)
                    .ToList();
            }
            else
            {
                retList = OrmService.Db.Select<RecordFile>().Where("1=1")
                    .WhereIf(idFound,
                        x => x.PushMediaServerId!.Trim().ToLower().Equals(rgdv.MediaServerId!.Trim().ToLower()))
                    .WhereIf(cameraIdFound, x => x.CameraId!.Trim().ToLower().Equals(rgdv.CameraId!.Trim().ToLower()))
                    .WhereIf(rgdv.StartTime != null, x => x.StartTime >= rgdv.StartTime)
                    .WhereIf(rgdv.EndTime != null, x => x.EndTime <= rgdv.EndTime)
                    .WhereIf(!(bool) rgdv.IncludeDeleted!, x => x.Deleted == false).OrderBy(orderBy)
                    .Count(out total)
                    .Page((int) rgdv.PageIndex!, (int) rgdv.PageSize!)
                    .ToList();
            }

            DvrVideoResponseList result = new DvrVideoResponseList();
            result.DvrVideoList = retList;
            if (!isPageQuery)
            {
                if (retList != null)
                {
                    total = retList.Count;
                }
                else
                {
                    total = 0;
                }
            }

            result.Total = total;
            result.Request = rgdv;
            return result;
        }

        /// <summary>
        /// 获取已注册的摄像头列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<CameraInstance> GetCameraInstanceList(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            try
            {
                return OrmService.Db.Select<CameraInstance>().Where(x => x.PushMediaServerId.Equals(mediaServerId))
                    .ToList();
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message,
                };
                return null;
            }
        }

        /// <summary>
        /// 修改注册摄像头的配置信息
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CameraInstance ModifyCameraInstance(string mediaServerId, ReqMoidfyCameraInstance req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            try
            {
                var ret = OrmService.Db.Update<CameraInstance>()
                    .SetIf(!string.IsNullOrEmpty(req.CameraName) && !req.CameraName.ToLower().Trim().Equals("string"),
                        x => x.CameraName, req.CameraName)
                    .SetIf(!string.IsNullOrEmpty(req.DeptId) && !req.DeptId.ToLower().Trim().Equals("string"),
                        x => x.DeptId, req.DeptId)
                    .SetIf(!string.IsNullOrEmpty(req.DeptName) && !req.DeptName.ToLower().Trim().Equals("string"),
                        x => x.DeptName, req.DeptName)
                    .SetIf(!string.IsNullOrEmpty(req.PDeptId) && !req.PDeptId.ToLower().Trim().Equals("string"),
                        x => x.PDetpId, req.PDeptId)
                    .SetIf(!string.IsNullOrEmpty(req.PDeptName) && !req.PDeptName.ToLower().Trim().Equals("string"),
                        x => x.PDetpName, req.PDeptName)
                    .SetIf(req.EnableLive != null, x => x.EnableLive, req.EnableLive)
                    .SetIf(req.IfGb28181Tcp != null, x => x.IfGb28181Tcp, req.IfGb28181Tcp)
                    .SetIf(req.EnablePtz != null, x => x.EnablePtz, req.EnablePtz).Set(x => x.UpdateTime, DateTime.Now)
                    .Where(x => x.PushMediaServerId.Equals(mediaServerId)).Where(x => x.CameraId.Equals(req.CameraId))
                    .ExecuteAffrows();
                if (ret > 0)
                {
                    return OrmService.Db.Select<CameraInstance>().Where(x => x.CameraId.Equals(req.CameraId))
                        .Where(x => x.PushMediaServerId.Equals(mediaServerId)).First();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message,
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }

        /// <summary>
        /// 删除一个注册的摄像头实例
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="cameraId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteCameraInstance(string mediaServerId, string cameraId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return false;
            }

            try
            {
                var ret = OrmService.Db.Delete<CameraInstance>().Where(x => x.CameraId.Equals(cameraId))
                    .Where(x => x.PushMediaServerId.Equals(mediaServerId))
                    .ExecuteAffrows();
                if (ret > 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message,
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return false;
        }

        /// <summary>
        /// 注册一个摄像头信息以供流发布时的鉴权
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="req"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CameraInstance AddCameraInstance(string mediaServerId, ReqAddCameraInstance req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            req.PushMediaServerId = mediaServerId;
            req.UpdateTime = DateTime.Now;
            if (req.CameraType == CameraType.Rtsp)
            {
                if (string.IsNullOrEmpty(req.IfRtspUrl) || req.IfRtspUrl.Trim().ToLower().Equals("string"))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.FunctionInputParamsError,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                    };
                    return null;
                }

                req.CameraDeviceLable = "";
                req.CameraChannelLable = "";
                req.IfGb28181Tcp = false;
            }

            if (req.CameraType == CameraType.GB28181)
            {
                if (string.IsNullOrEmpty(req.CameraDeviceLable) || string.IsNullOrEmpty(req.CameraChannelLable) ||
                    req.CameraChannelLable.Trim().ToLower().Equals("string") ||
                    req.CameraDeviceLable.Trim().ToLower().Equals("string"))
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.FunctionInputParamsError,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                    };
                    return null;
                }

                req.IfRtspUrl = "";
            }

            req.CameraId = string.Format("{0:X8}", CRC32Cls.GetCRC32(req.PushMediaServerId + req.CameraType +
                                                                     req.CameraIpAddress + req.CameraChannelLable +
                                                                     req.IfRtspUrl));
            //使用流媒体id+摄像头类型+摄像头ip+gb28181设备名+gb28181通道名+rtsp地址做crc32得到一个值，做为这个摄像头的唯一id
            req.CreateTime = DateTime.Now;
            if (string.IsNullOrEmpty(req.CameraName))
            {
                req.CameraName = req.CameraId;
            }

            var cameraItc = (req as CameraInstance);
            if (string.IsNullOrEmpty(cameraItc.CameraName) || cameraItc.CameraName.Trim().ToLower().Equals("string"))
            {
                cameraItc.CameraName = "";
            }

            if (string.IsNullOrEmpty(cameraItc.DeptId) || cameraItc.DeptId.Trim().ToLower().Equals("string"))
            {
                cameraItc.DeptId = "";
            }

            if (string.IsNullOrEmpty(cameraItc.DeptName) || cameraItc.DeptName.Trim().ToLower().Equals("string"))
            {
                cameraItc.DeptName = "";
            }

            if (string.IsNullOrEmpty(cameraItc.PDetpId) || cameraItc.PDetpId.Trim().ToLower().Equals("string"))
            {
                cameraItc.PDetpId = "";
            }

            if (string.IsNullOrEmpty(cameraItc.PDetpName) || cameraItc.PDetpName.Trim().ToLower().Equals("string"))
            {
                cameraItc.PDetpName = "";
            }

            try
            {
                var ret = OrmService.Db.Insert<CameraInstance>(cameraItc).ExecuteAffrows();
                if (ret > 0)
                {
                    return OrmService.Db.Select<CameraInstance>().Where(x => x.CameraId.Equals(cameraItc.CameraId))
                        .Where(x => x.PushMediaServerId.Equals(mediaServerId)).First();
                }
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.SystemDataBaseExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.SystemDataBaseExcept] + "\r\n" + ex.Message,
                };

                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Other,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
            };
            return null;
        }


        /// <summary>
        /// 获取在线player列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<PlayerSession> GetPlayerSessionList(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            if (!mediaServer.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            lock (Common.PlayerSessionListLock)
            {
                return new List<PlayerSession>(Common.PlayerSessions);
            }
        }

        /// <summary>
        /// 获取在线摄像头列表
        /// </summary>
        /// <param name="mediaServerId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<CameraSession> GetCameraSessionList(string mediaServerId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var mediaServer = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(mediaServerId));
            if (mediaServer == null)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            if (!mediaServer.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            lock (Common.CameraSessionLock)
            {
                return new List<CameraSession>(Common.CameraSessions);
            }
        }


        /// <summary>
        /// 获取流媒体配置信息
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitConfig GetConfig(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        var obj = mediaObj.WebApi.GetConfig(out rs);
                        if (obj != null && obj.Data != null && obj.Data[0] != null)
                        {
                            lock (mediaObj.Config)
                            {
                                mediaObj.Config = obj.Data[0];
                            }
                        }

                        return obj;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        public static ResZLMediaKitAddStreamProxy AddStreamProxy(string deviceid, string src_url, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(deviceid) || string.IsNullOrEmpty(src_url))
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.FunctionInputParamsError,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
                };
                return null;
            }

            var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
            if (mediaObj == null || !mediaObj.IsRunning)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.ZLMediaKitNotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                };
                return null;
            }

            try
            {
                var result = mediaObj.WebApi.AddStreamProxy(src_url, out rs);
                if (result != null)
                {
                    CameraSession session = null;
                    lock (Common.CameraSessionLock)
                    {
                        session = Common.CameraSessions.FindLast(x => x.Vhost.Equals(result.Vhost)
                                                                      && x.App.Equals(
                                                                          result.App) &&
                                                                      x.StreamId.Equals(
                                                                          result.StreamId));
                    }

                    if (session == null)
                    {
                        CameraInstance camera = null;
                        lock (Common.CameraInstanceListLock)
                        {
                            camera = Common.CameraInstanceList.FindLast(x =>
                                x.IfRtspUrl.Equals(result.Src_Url));
                        }

                        if (camera != null)
                        {
                            CameraSession sessionnew = new CameraSession()
                            {
                                CameraId = camera.CameraId,
                                MediaServerId = camera.PushMediaServerId,
                                CameraName = camera.CameraName,
                                CameraDeptId = camera.DeptId,
                                CameraDeptName = camera.DeptName,
                                CameraPDeptId = camera.PDetpId,
                                CameraPDeptName = camera.PDetpName,
                                CameraType = camera.CameraType,
                                ClientType = ClientType.Camera,
                                CameraEx = new CameraEx()
                                {
                                    App = result.App,
                                    Camera = null,
                                    Ctype = CameraType.Rtsp,
                                    MediaServerId = mediaObj.MediaServerId,
                                    PushStreamSocketType = PushStreamSocketType.TCP,
                                    SipCameraStatus = null,
                                    StreamId = CRC32Cls.GetCRC32(result.Src_Url),
                                    StreamServerIp = mediaObj.Ipaddress,
                                    StreamServerPort = mediaObj.MediaServerHttpPort,
                                    Vhost = result.Vhost,
                                    InputUrl = result.Src_Url,
                                },
                                IsOnline = true,
                                IsRecord = false,
                                PlayUrl = result.Play_Url,
                                CameraIpAddress = camera.CameraIpAddress,
                                UpTime = 0,
                                OnlineTime = DateTime.Now,
                                Vhost = result.Vhost,
                                App = result.App,
                                StreamId = result.StreamId,
                                MediaServerIp = mediaObj.Ipaddress,
                            };

                            lock (Common.CameraSessionLock)
                            {
                                Common.CameraSessions.Add(sessionnew);
                            }

                            ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                            {
                                App = session.App,
                                CameraProtocolType = session.CameraType,
                                ClientType = session.ClientType,
                                CreateTime = DateTime.Now,
                                Ipaddress = session.CameraIpAddress,
                                CameraId = session.CameraId,
                                OnOff = OnOff.On,
                                PushMediaServerId = mediaObj.MediaServerId,
                                Vhost = session.Vhost,
                                StreamId = session.StreamId,
                            };
                            OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                        }

                        return null;
                    }

                    //如果存在session就更新一下online属性
                    lock (Common.CameraSessionLock)
                    {
                        session.IsOnline = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Other,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                };
                return null;
            }
        }

        /// <summary>
        /// 启动一个FFmpeg流
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitAddFFmpegProxy AddFFmpegProxy(string deviceid, string src_url, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        var result = mediaObj.WebApi.TryAddRtspProxy(src_url, out rs);
                        if (result != null)
                        {
                            CameraSession session = null;
                            lock (Common.CameraSessionLock)
                            {
                                session = Common.CameraSessions.FindLast(x => x.Vhost.Equals(result.Vhost)
                                                                              && x.App.Equals(
                                                                                  result.App) &&
                                                                              x.StreamId.Equals(
                                                                                  result.StreamId));
                            }

                            if (session == null)
                            {
                                CameraInstance camera = null;
                                lock (Common.CameraInstanceListLock)
                                {
                                    camera = Common.CameraInstanceList.FindLast(x =>
                                        x.IfRtspUrl.Equals(result.Src_Url));
                                }

                                if (camera != null)
                                {
                                    CameraSession sessionnew = new CameraSession()
                                    {
                                        CameraId = camera.CameraId,
                                        MediaServerId = camera.PushMediaServerId,
                                        CameraName = camera.CameraName,
                                        CameraDeptId = camera.DeptId,
                                        CameraDeptName = camera.DeptName,
                                        CameraPDeptId = camera.PDetpId,
                                        CameraPDeptName = camera.PDetpName,
                                        CameraType = camera.CameraType,
                                        ClientType = ClientType.Camera,
                                        CameraEx = new CameraEx()
                                        {
                                            App = result.App,
                                            Camera = null,
                                            Ctype = CameraType.Rtsp,
                                            MediaServerId = mediaObj.MediaServerId,
                                            PushStreamSocketType = PushStreamSocketType.TCP,
                                            SipCameraStatus = null,
                                            StreamId = CRC32Cls.GetCRC32(result.Src_Url),
                                            StreamServerIp = mediaObj.Ipaddress,
                                            StreamServerPort = mediaObj.MediaServerHttpPort,
                                            Vhost = result.Vhost,
                                            InputUrl = result.Src_Url,
                                        },
                                        IsOnline = true,
                                        IsRecord = false,
                                        PlayUrl = result.Play_Url,
                                        CameraIpAddress = camera.CameraIpAddress,
                                        UpTime = 0,
                                        OnlineTime = DateTime.Now,
                                        Vhost = result.Vhost,
                                        App = result.App,
                                        StreamId = result.StreamId,
                                        MediaServerIp = mediaObj.Ipaddress,
                                    };

                                    lock (Common.CameraSessionLock)
                                    {
                                        Common.CameraSessions.Add(sessionnew);
                                    }

                                    ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                                    {
                                        App = session.App,
                                        CameraProtocolType = session.CameraType,
                                        ClientType = session.ClientType,
                                        CreateTime = DateTime.Now,
                                        Ipaddress = session.CameraIpAddress,
                                        CameraId = session.CameraId,
                                        OnOff = OnOff.On,
                                        PushMediaServerId = mediaObj.MediaServerId,
                                        Vhost = session.Vhost,
                                        StreamId = session.StreamId,
                                    };
                                    OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                                }

                                return null;
                            }

                            //如果存在session就更新一下online属性
                            lock (Common.CameraSessionLock)
                            {
                                session.IsOnline = true;
                            }
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 关闭一个流
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitCloseStreams CloseStreams(string deviceid, ReqZLMediaKitCloseStreams req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        StopRecord(mediaObj.MediaServerId, new ReqZLMediaKitStopRecord() //关流的时候先停掉录制
                        {
                            App = req.App,
                            Secret = "",
                            Stream = req.Stream,
                            Type = 1,
                            Vhost = req.Vhost,
                        }, out _);
                        Thread.Sleep(100); //延时一下

                        var result = mediaObj.WebApi.CloseStreams(req, out rs);
                        if (result != null)
                        {
                            CameraSession session = null;
                            lock (Common.CameraSessionLock)
                            {
                                session = Common.CameraSessions.FindLast(x => x.App.Equals(req.App)
                                                                              && x.Vhost.Equals(
                                                                                  req.Vhost) &&
                                                                              x.StreamId.Equals(
                                                                                  req.Stream));
                            }

                            if (session != null)
                            {
                                lock (Common.CameraSessionLock)
                                {
                                    session.IsOnline = false;
                                }

                                ClientOnOffLog tmpClientLog = new ClientOnOffLog()
                                {
                                    App = session.App,
                                    CameraProtocolType = session.CameraType,
                                    ClientType = session.ClientType,
                                    CreateTime = DateTime.Now,
                                    Ipaddress = session.CameraIpAddress,
                                    OnOff = OnOff.Off,
                                    PushMediaServerId = mediaObj.MediaServerId,
                                    Vhost = session.Vhost,
                                    StreamId = session.StreamId,
                                };

                                OrmService.Db.Insert<ClientOnOffLog>(tmpClientLog).ExecuteAffrows();
                            }
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 获取流列表 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitMediaList GetStreamList(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.GetMediaList(out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 开始录制 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitStartStopRecord StartRecord(string deviceid, ReqZLMediaKitStartRecord req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.StartRecord(req, out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }

        /// <summary>
        /// 停止录制 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitStartStopRecord StopRecord(string deviceid, ReqZLMediaKitStopRecord req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.StopRecord(req, out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 获取录制状态 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitIsRecord GetRecordStatus(string deviceid, ReqZLMediaKitStopRecord req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.GetRecordStatus(req, out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 关闭rtp端口 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitOpenRtpPort OpenRtpPort(string deviceid, ReqZLMediaKitOpenRtpPort req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.OpenRtpPort(req, out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }


        /// <summary>
        /// 关闭rtp端口 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitCloseRtpPort CloseRtpPort(string deviceid, ReqZLMediaKitCloseRtpPort req,
            out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.CloseRtpPort(req, out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }

        /// <summary>
        /// 获取rtp端口列表 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ResZLMediaKitRtpPortList GetRtpPortList(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    if (!mediaObj.IsRunning)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.ZLMediaKitNotRunning,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.ZLMediaKitNotRunning],
                        };
                        return null;
                    }

                    try
                    {
                        return mediaObj.WebApi.GetRtpPortList(out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return null;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return null;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return null;
        }

        /// <summary>
        /// 流媒体服务是否正在运行
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CheckMediaServerIsRunning(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    try
                    {
                        return mediaObj.GetIsRunning(out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return false;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return false;
        }


        /// <summary>
        /// 重启流媒体服务
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool RestartMediaServer(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    try
                    {
                        var streamList = GetStreamList(mediaObj.MediaServerId, out _);
                        if (streamList != null && streamList.Data != null && streamList.Data.Count > 0)
                        {
                            foreach (var data in streamList.Data)
                            {
                                if (data != null)
                                {
                                    try
                                    {
                                        //重启mediaserver前踢掉所有流

                                        CloseStreams(mediaObj.MediaServerId, new ReqZLMediaKitCloseStreams()
                                        {
                                            App = data.App,
                                            Force = true,
                                            Schema = data.Schema,
                                            Secret = "",
                                            Stream = data.Stream,
                                            Vhost = data.Vhost,
                                        }, out _);
                                    }
                                    catch
                                    {
                                        Thread.Sleep(20);
                                        continue;
                                    }

                                    Thread.Sleep(20);
                                }
                            }
                        }

                        var result = mediaObj.RestartServer(out rs);
                        if (result)
                        {
                            GetConfig(deviceid, out _); //重启时获取一次配置信息
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return false;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return false;
        }


        /// <summary>
        /// 停止流媒体服务
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StopMediaServer(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    try
                    {
                        var streamList = GetStreamList(mediaObj.MediaServerId, out _);
                        if (streamList != null && streamList.Data != null && streamList.Data.Count > 0)
                        {
                            foreach (var data in streamList.Data)
                            {
                                if (data != null)
                                {
                                    try
                                    {
                                        //关闭mediaserver前踢掉所有流

                                        CloseStreams(mediaObj.MediaServerId, new ReqZLMediaKitCloseStreams()
                                        {
                                            App = data.App,
                                            Force = true,
                                            Schema = data.Schema,
                                            Secret = "",
                                            Stream = data.Stream,
                                            Vhost = data.Vhost,
                                        }, out _);
                                    }
                                    catch
                                    {
                                        Thread.Sleep(20);
                                        continue;
                                    }

                                    Thread.Sleep(20);
                                }
                            }
                        }

                        return mediaObj.StopServer(out rs);
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return false;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return false;
        }

        /// <summary>
        /// 启动流媒体服务
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool StartMediaServer(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (!string.IsNullOrEmpty(deviceid))
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    try
                    {
                        var result = mediaObj.RunServer(out rs);
                        if (result)
                        {
                            GetConfig(deviceid, out _); //启动时获取一次配置信息
                        }

                        return result;
                    }
                    catch (Exception ex)
                    {
                        rs = new ResponseStruct()
                        {
                            Code = ErrorNumber.Other,
                            Message = ErrorMessage.ErrorDic![ErrorNumber.Other] + "\r\n" + ex.Message,
                        };
                        return false;
                    }
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServerInstancesNotFound,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServerInstancesNotFound],
                };
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.FunctionInputParamsError,
                Message = ErrorMessage.ErrorDic![ErrorNumber.FunctionInputParamsError],
            };
            return false;
        }

        /// <summary>
        /// 获取流媒体服务器列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<MediaServerInstance> GetMediaServerList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            return Common.MediaServerList;
        }

        /// <summary>
        /// 获取一个MediaServer的实例
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static MediaServerInstance GetMediaServerInstance(string deviceid, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerList != null && Common.MediaServerList.Count > 0)
            {
                var mediaObj = Common.MediaServerList.FindLast(x => x.MediaServerId.Equals(deviceid));
                if (mediaObj != null)
                {
                    return mediaObj;
                }

                return null;
            }

            return null;
        }
    }
}