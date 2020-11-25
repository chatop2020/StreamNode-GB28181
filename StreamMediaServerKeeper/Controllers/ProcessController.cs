using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace StreamMediaServerKeeper.Controllers
{
    [ApiController]
    [Route("/")]
    public class ProcessController : ControllerBase
    {
        /// <summary>
        /// 获取合并裁剪任务积压列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetBacklogTaskList")]
        public List<CutMergeTaskStatusResponse> GetBacklogTaskList()
        {
            ResponseStruct rs;
            var ret = CutMergeService.GetBacklogTaskList(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 获取合并剪辑任务的进度信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetMergeTaskStatus")]
        public CutMergeTaskStatusResponse GetMergeTaskStatus(string taskId)
        {
            ResponseStruct rs;
            var ret = CutMergeService.GetMergeTaskStatus(taskId, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 添加一个裁剪合并任务
        /// </summary>
        /// <param name="rcmv"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("AddCutOrMergeTask")]
        [HttpPost]
        public CutMergeTaskResponse AddCutOrMergeTask(CutMergeTask task)
        {
            ResponseStruct rs;
            var ret = CutMergeService.AddCutOrMergeTask(task, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("FileExists")]
        [HttpGet]
        public bool FileExists(string filePath)
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.FileExists(filePath, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="filePathList"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("DeleteFileList")]
        [HttpPost]
        public bool DeleteFile(List<string> filePathList)
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.DeleteFileList(filePathList, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("DeleteFile")]
        [HttpGet]
        public bool DeleteFile(string filePath)
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.DeleteFile(filePath, out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }


        /// <summary>
        /// 清空录制目录中的空目录
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("ClearNoFileDir")]
        [HttpGet]
        public bool ClearNoFileDir()
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.ClearNoFileDir(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StartServer")]
        [HttpGet]
        public int StartServer()
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.RunServer(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 停止流媒体服务器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("StopServer")]
        [HttpGet]
        public bool StopServer()
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.StopServer(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 重启流媒体服务器
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("RestartServer")]
        [HttpGet]
        public int RestartServer()
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.RestartServer(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }

        /// <summary>
        /// 流媒体服务器是否正在运行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="HttpResponseException"></exception>
        [Route("CheckIsRunning")]
        [HttpGet]
        public int CheckIsRunning()
        {
            ResponseStruct rs;
            var ret = Common.ProcessApis.CheckIsRunning(out rs);
            if (rs.Code != ErrorNumber.None)
            {
                throw new HttpResponseException(JsonHelper.ToJson(rs));
            }

            return ret;
        }
    }
}