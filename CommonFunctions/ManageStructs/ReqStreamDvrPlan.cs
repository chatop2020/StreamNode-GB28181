using System;
using System.Collections.Generic;
using CommonFunctions.DBStructs;


namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class ReqDvrDayTimeRange
    {
       
        public int StreamDvrPlanId { get; set; }
        public DayOfWeek WeekDay { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
    
    [Serializable]
    public class ReqStreamDvrPlan
    {
        public bool Enable { get; set; }
            public string MediaServerId { get; set; } = null!;
            public string CameraId { get; set; } = null;
            public long? LimitSpace { get; set; }

            public int? LimitDays { get; set; }
            
            public OverStepPlan? OverStepPlan { get; set; }
            public List<ReqDvrDayTimeRange> TimeRangeList { get; set; } = null!;
        
    }
}