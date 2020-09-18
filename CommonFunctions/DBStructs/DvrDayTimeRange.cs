using System;
using System.Text.Json.Serialization;
using FreeSql.DataAnnotations;

namespace CommonFunctions.DBStructs
{
    [Serializable]
    [Table(Name = "DvrDayTimeRange")]
    /// <summary>
    /// 用于每周的记录时间
    /// </summary>
    public class DvrDayTimeRange
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        [JsonIgnore]
        public int Id { get; set; }

        public int StreamDvrPlanId { get; set; }
        [Column(MapType = typeof(string))] public DayOfWeek WeekDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}