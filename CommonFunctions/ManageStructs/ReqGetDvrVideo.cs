using System;
using System.Collections.Generic;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class ReqGetDvrVideo : ReqGetDvrPlan
    {
        private int? _pageIndex;
        private int? _pageSzie;
        private bool? _includeDeleted;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private List<OrderByStruct>? _orderBy;


        public int? PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        public int? PageSize
        {
            get => _pageSzie;
            set => _pageSzie = value;
        }

        public bool? IncludeDeleted
        {
            get => _includeDeleted;
            set => _includeDeleted = value;
        }

        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        public List<OrderByStruct>? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }
    }
}