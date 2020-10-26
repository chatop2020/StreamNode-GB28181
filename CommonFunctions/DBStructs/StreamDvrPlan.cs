using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace CommonFunctions.DBStructs
{
    [Serializable]
    /// <summary>
    /// 超过限制时怎么处理
    /// StopDvr=停止录制
    /// DeleteFile=删除文件
    /// </summary>
    public enum OverStepPlan
    {
        StopDvr,
        DeleteFile,
    }

    [Table(Name = "StreamDvrPlan")]
    [Index("uk_dvrPlan_MId", "MediaServerId", false)]
    [Index("uk_dvrPlan_CId", "CameraId", false)]
    [Serializable]
    /// <summary>
    /// 录制计划
    /// </summary>
    public class StreamDvrPlan
    {
        /// <summary>
        /// 数据库主键
        /// </summary>
        [Column(IsPrimary = true, IsIdentity = true)]
        [JsonIgnore]
        public int? Id { get; set; }

        /// <summary>
        /// 是否启用该录制计划(true为启用)
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
        /// 具体的录制计划列表
        /// </summary>
        [Column(MapType = typeof(string))]
        public OverStepPlan? OverStepPlan { get; set; }

        [Navigate(nameof(DvrDayTimeRange.StreamDvrPlanId))]
        public List<DvrDayTimeRange> TimeRangeList { get; set; } = null!;
    }
}