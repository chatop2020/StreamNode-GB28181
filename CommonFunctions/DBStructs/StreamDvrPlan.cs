using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace CommonFunctions.DBStructs
{
    [Serializable]
    /// <summary>
    /// 超过限制时怎么处理
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
        [Column(IsPrimary = true, IsIdentity = true)]
        [JsonIgnore]
        public int? Id { get; set; }

        public bool Enable { get; set; }

        public string MediaServerId { get; set; } = null!;

        public string CameraId { get; set; } = null;

        public long? LimitSpace { get; set; }

        public int? LimitDays { get; set; }
        
       
        [Column(MapType = typeof(string))] public OverStepPlan? OverStepPlan { get; set; }

        [Navigate(nameof(DvrDayTimeRange.StreamDvrPlanId))]
        public List<DvrDayTimeRange> TimeRangeList { get; set; } = null!;
    }
}