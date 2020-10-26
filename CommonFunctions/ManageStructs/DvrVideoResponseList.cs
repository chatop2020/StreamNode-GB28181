using System;
using System.Collections.Generic;
using CommonFunctions.DBStructs;

namespace CommonFunctions.ManageStructs
{
    [Serializable]
    public class DvrVideoResponseList
    {
        private List<RecordFile>? _dvrVideoList;
        private ReqGetDvrVideo? _request;
        private long? _total;

        
        /// <summary>
        /// 录制文件列表
        /// </summary>
        public List<RecordFile>? DvrVideoList
        {
            get => _dvrVideoList;
            set => _dvrVideoList = value;
        }

        /// <summary>
        /// 请求结构
        /// </summary>
        public ReqGetDvrVideo? Request
        {
            get => _request;
            set => _request = value;
        }

        /// <summary>
        /// 记录数
        /// </summary>
        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}