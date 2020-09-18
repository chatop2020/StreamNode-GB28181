using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;



namespace StreamMediaServerKeeper
{
    public static class CutMergeService
    {
        public static bool start = false;
        public static BlockingCollection<CutMergeTask> CutMergeTaskList = new BlockingCollection<CutMergeTask>(10);
        public static List<CutMergeTask> CutMergeTaskStatusList= new List<CutMergeTask>();
        static CutMergeService()
        {
          
            Task.Factory.StartNew(() =>
            {
                foreach (var value in CutMergeTaskList.GetConsumingEnumerable())
                {
                  //  CutMergeTaskStatusList.Add(value);
                    var taskReturn = CutMerge(value);
                    if (taskReturn != null)
                    {
                        taskReturn.Uri = "http://"+Common.MyIPAddress+":" + Common.HttpPort.ToString() +"/"+
                                         taskReturn.FilePath!.Replace(Common.StaticFilePath, "");
                        var postDate = JsonHelper.ToJson(taskReturn);
                        var ret = NetHelper.HttpPostRequest(taskReturn.Task.CallbakUrl!, null!, postDate);
                    }
                }
            });
        }
        
        /// <summary>
        /// 获取合并裁剪任务的情况
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CutMergeTaskStatusResponse GetMergeTaskStatus(string taskId, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var ret = CutMergeService.CutMergeTaskStatusList.FindLast(x => x.TaskId == taskId);
            
            if (ret == null)
            {
               
                return null!;
            }

            
            var result = new CutMergeTaskStatusResponse()
            {
                CallbakUrl = ret.CallbakUrl,
                CreateTime = ret.CreateTime,
                ProcessPercentage = ret.ProcessPercentage,
                TaskId = ret.TaskId,
                TaskStatus = ret.TaskStatus,
                
            };
           
            return result;
        }
        
        /// <summary>
        /// 获取裁剪积压任务列表
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static List<CutMergeTaskStatusResponse> GetBacklogTaskList(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (CutMergeTaskStatusList != null && CutMergeTaskStatusList.Count>0)
            {
                var retList= CutMergeTaskStatusList.FindAll(x => x.TaskStatus == TaskStatus.Create).ToList();
                if (retList != null && retList.Count > 0)
                {
                    List<CutMergeTaskStatusResponse> resultList= new List<CutMergeTaskStatusResponse>();
                    foreach (var ret in retList!)
                    {
                        CutMergeTaskStatusResponse res = new CutMergeTaskStatusResponse()
                        {
                            CallbakUrl = ret.CallbakUrl,
                            CreateTime = ret.CreateTime,
                            ProcessPercentage = ret.ProcessPercentage,
                            TaskId = ret.TaskId,
                            TaskStatus = ret.TaskStatus,
                        };
                        resultList.Add(res);
                    }

                    return resultList;
                }
            }

            return null!;
        }

        

        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="task"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static CutMergeTaskResponse AddCutOrMergeTask(CutMergeTask task, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            try
            {
                CutMergeTaskList.Add(task);
                CutMergeTaskStatusList.Add(task);
                return new CutMergeTaskResponse()
                {
                    Duration = -1,
                    FilePath = "",
                    FileSize = -1,
                    Status = CutMergeRequestStatus.WaitForCallBack,
                    Task = task,
                    Request = null,
                };
            }
            catch
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Other,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Other],
                };
                return null;
            }
        }
        /// <summary>
        /// 将mp4转为ts格式封装，这里可能需要捕获异常，超时30分钟
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static CutMergeTask packageToTsStreamFile(CutMergeTask task)
        {
            task.TaskStatus = TaskStatus.Packaging;
            string tsPath = Common.CutOrMergeTempPath + task.TaskId + "/ts";
            if (!Directory.Exists(tsPath))
            {
                Directory.CreateDirectory(tsPath);
            }

            for (int i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
            {
                string videoFileNameWithOutExt = Path.GetFileNameWithoutExtension(task.CutMergeFileList[i]!.FilePath!);
                string videoTsFileName = videoFileNameWithOutExt + ".ts";
                string videoTsFilePath = tsPath + "/" + videoTsFileName;
                string ffmpegCmd = Common.FFmpegBinPath + " -i " + task.CutMergeFileList[i]!.FilePath! +
                                   " -vcodec copy -acodec copy -vbsf h264_mp4toannexb " + videoTsFilePath + " -y";
                var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out string std, out string err);

                if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                    File.Exists(videoTsFilePath))
                {
                    
                    FileInfo fileInfo= new FileInfo(videoTsFilePath);
                    if (fileInfo.Length > 10)
                    {
                        task.CutMergeFileList[i].FilePath = videoTsFilePath; 
                    }
                   
                }
                task.ProcessPercentage +=   ((double) 1 / (double) task.CutMergeFileList!.Count * 100f) * 0.4f;
                Thread.Sleep(20);
            }

            return task;
        }

        /// <summary>
        /// 生成合并文件，并合并ts文件，同时输出mp4文件， -movflags faststart 标记是可让mp4在web上快速加载播放，超时30分钟
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private static string mergeProcess(CutMergeTask task)
        {
            task.TaskStatus = TaskStatus.Mergeing;
            string mergePath = Common.CutOrMergeTempPath+ task.TaskId;
            string outPutPath =Common.CutOrMergePath +
                                DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (!Directory.Exists(outPutPath))
            {
                Directory.CreateDirectory(outPutPath);
            }

            List<string> mergeStringList = new List<string>();
            for (int i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
            {
                mergeStringList.Add("file '" + task.CutMergeFileList[i].FilePath + "'");
               
            }

            File.WriteAllLines(mergePath + "files.txt", mergeStringList);
            
            string newFilePath = outPutPath + "/" + task.TaskId + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") +
                                 ".mp4";
            string ffmpegCmd = Common.FFmpegBinPath + " -threads " + Common.FFmpegThreadCount.ToString() +
                               " -f concat -safe 0 -i " + mergePath +
                               "files.txt" + " -c copy  -movflags faststart " + newFilePath;
           
            var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out string std, out string err);
            task.ProcessPercentage += 40f;
            if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                File.Exists(newFilePath) )
            {
                long find = -1;
                FileInfo fileInfo= new FileInfo(newFilePath);
                if (fileInfo.Length > 10)
                {
                    return newFilePath;
                }
            }

            /*LogWriter.WriteLog("合并请求任务失败(mergeProcess失败)...", task.TaskId! + "\r\n" + err, ConsoleColor.Yellow);*/
            return null!;
        }

        /// <summary>
        /// 对需要裁剪的视频进行裁剪，超时30分钟
        /// </summary>
        /// <param name="cms"></param>
        /// <returns></returns>
        private static CutMergeStruct cutProcess(CutMergeStruct cms)
        {
            string tsPath = Path.GetDirectoryName(cms.FilePath!)!;
            string fileName = Path.GetFileName(cms.FilePath!)!;
            string newTsName = tsPath + "/cut_" + fileName;


            string ffmpegCmd = Common.FFmpegBinPath + " -i " + cms.FilePath +
                               " -vcodec copy -acodec copy -ss " + cms.CutStartPos + " -to " + cms.CutEndPos + " " +
                               newTsName + " -y";
            var retRun = LinuxShell.Run(ffmpegCmd, 1000 * 60 * 30, out string std, out string err);
            if (retRun && (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err)) &&
                File.Exists(newTsName))
            {
                long find = -1;
                if (!string.IsNullOrEmpty(std))
                {
                    var str = Common.GetValue(std, "video:", "audio:");
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str.ToLower();
                        str = str.Replace("kb", "");
                        long.TryParse(str, out find);
                    }
                }
                else if (!string.IsNullOrEmpty(err))
                {
                    var str = Common.GetValue(err, "video:", "audio:");
                    str = str.ToLower();
                    str = str.Replace("kb", "");
                    long.TryParse(str, out find);
                }

                if (find > 0)
                {
                  
                    cms.FilePath = newTsName;
                }
                else
                {
                    /*
                    LogWriter.WriteLog("合并请求任务裁剪失败(cutProcess)...", ffmpegCmd + "\r\n" + err, ConsoleColor.Yellow);
                */
                }
            }

            return cms;
        }

        /// <summary>
        /// 对文件进行操作
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static CutMergeTaskResponse CutMerge(CutMergeTask task)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); //  开始监视代码运行时间
            /*
            LogWriter.WriteLog("合并请求任务开始(CutMerge)...", task.TaskId!);
            */
            string taskPath = "";
            if (task != null && task.CutMergeFileList != null && task.CutMergeFileList.Count > 0)
            {
                taskPath = Common.CutOrMergeTempPath + task.TaskId;
                if (!Directory.Exists(taskPath))
                {
                    Directory.CreateDirectory(taskPath);
                }

                try
                {
                    task = packageToTsStreamFile(task); //转ts文件
                  
                    
                    task.TaskStatus = TaskStatus.Cutting;

                    List<CutMergeStruct> cutFileList = task.CutMergeFileList!
                        .FindAll(x => x.CutEndPos != null && x.CutStartPos != null).ToList();
                    for (int i = 0; i <= task.CutMergeFileList!.Count - 1; i++)
                    {
                        if (task.CutMergeFileList[i].CutStartPos != null && task.CutMergeFileList[i].CutEndPos != null)
                        {
                            task.ProcessPercentage +=   ((double) 1 / (double)cutFileList.Count * 100f) * 0.15f;
                          
                            //做剪切
                            task.CutMergeFileList[i] = cutProcess(task.CutMergeFileList[i]);
                            Thread.Sleep(20);
                        }
                    }
                  
                    string filePath = mergeProcess(task);
                   
                    task.ProcessPercentage = 100f;
                    task.TaskStatus = TaskStatus.Closed;
                    stopwatch.Stop(); //  停止监视
                    TimeSpan timespan = stopwatch.Elapsed;
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    {
                        long duration = -1;
                        string newPath = "";
                      
                        FFmpegGetDuration.GetDuration(Common.FFmpegBinPath, filePath, out duration,out newPath);
                        var ret = CutMergeTaskStatusList.FindLast(x => x.TaskId == task.TaskId);
                        if (ret != null)
                        {
                            
                        }
                      
                        return new CutMergeTaskResponse
                        {
                            FilePath = newPath,
                            FileSize = new FileInfo(filePath).Length,
                            Duration = duration,
                            Status = CutMergeRequestStatus.Succeed,
                            Task = task,
                            TimeConsuming = timespan.TotalMilliseconds,
                        };
                    }
                  
                    return new CutMergeTaskResponse
                    {
                        FilePath = "",
                        Status = CutMergeRequestStatus.Failed,
                        FileSize = -1,
                        Duration = -1,
                        Task = task,
                        TimeConsuming = timespan.TotalMilliseconds,
                    };
                }
                catch (Exception ex)
                {
                    /*
                    LogWriter.WriteLog("裁剪合并视频文件时出现异常...", ex.Message + "\r\n" + ex.StackTrace, ConsoleColor.Yellow);
                    */
                    return null!;
                }
                finally
                {
                    if (!string.IsNullOrEmpty(taskPath) && Directory.Exists(taskPath)) //清理战场
                    {
                        Directory.Delete(taskPath, true);
                    }

                    if (File.Exists(Common.CutOrMergePath + task!.TaskId + "files.txt")
                    ) //清理战场
                    {
                        File.Delete(Common.CutOrMergePath  + task!.TaskId + "files.txt");
                    }
                }
            }


            return null!;
        }
    }
}