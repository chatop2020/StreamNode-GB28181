using System;
using System.Collections.Generic;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 请求结构-获取录制文件-支持分页
    /// </summary>
    [Serializable]
    public class ReqGetDvrVideo : ReqGetDvrPlan
    {
        private int? _pageIndex;
        private int? _pageSzie;
        private bool? _includeDeleted;
        private DateTime? _startTime;
        private DateTime? _endTime;
        private List<OrderByStruct>? _orderBy;


        /// <summary>
        /// 页码（从1开始）
        /// </summary>
        public int? PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        /// <summary>
        /// 每页记录数量
        /// </summary>
        public int? PageSize
        {
            get => _pageSzie;
            set => _pageSzie = value;
        }

        /// <summary>
        /// 是否包含已删除记录
        /// </summary>
        public bool? IncludeDeleted
        {
            get => _includeDeleted;
            set => _includeDeleted = value;
        }

        /// <summary>
        /// 录制开始时间
        /// </summary>
        public DateTime? StartTime
        {
            get => _startTime;
            set => _startTime = value;
        }

        /// <summary>
        /// 录制结束时间
        /// </summary>
        public DateTime? EndTime
        {
            get => _endTime;
            set => _endTime = value;
        }

        /// <summary>
        /// 排序结构
        /// </summary>
        public List<OrderByStruct>? OrderBy
        {
            get => _orderBy;
            set => _orderBy = value;
        }
    }
}