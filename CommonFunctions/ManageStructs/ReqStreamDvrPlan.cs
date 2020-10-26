using System;
using System.Collections.Generic;
using CommonFunctions.DBStructs;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 请求结构-录制计划-时间范围
    /// </summary>
    [Serializable]
    public class ReqDvrDayTimeRange
    {
        /// <summary>
        /// 录制计划ID
        /// </summary>
        public int StreamDvrPlanId { get; set; }

        /// <summary>
        /// 星期n
        /// </summary>
        public DayOfWeek WeekDay { get; set; }

        /// <summary>
        /// 开始录制的时间（仅取时间部分）
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束录制的时间（仅取时间部分）
        /// </summary>
        public DateTime EndTime { get; set; }
    }

    [Serializable]
    public class ReqStreamDvrPlan
    {
        /// <summary>
        /// 是否启用该录制计划
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 流媒体服务器ID
        /// </summary>
        public string MediaServerId { get; set; } = null!;

        /// <summary>
        /// 摄像头实例ID
        /// </summary>
        public string CameraId { get; set; } = null;

        /// <summary>
        /// 录制占用空间限制（Byte）,最大录制到某个值后做相应处理
        /// </summary>
        public long? LimitSpace { get; set; }

        /// <summary>
        /// 录制占用天数限制,最大录制到某个值后做相应处理
        /// </summary>
        public int? LimitDays { get; set; }

        /// <summary>
        /// 超过限制以后的处理方法
        /// StopDvr=停止录制
        /// DeleteFile=删除文件
        /// </summary>
        public OverStepPlan? OverStepPlan { get; set; }

        /// <summary>
        /// 请求结构-录制计划-时间范围列表
        /// </summary>
        public List<ReqDvrDayTimeRange> TimeRangeList { get; set; } = null!;
    }
}