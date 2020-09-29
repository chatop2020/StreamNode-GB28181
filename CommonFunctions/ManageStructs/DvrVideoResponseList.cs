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

        public List<RecordFile>? DvrVideoList
        {
            get => _dvrVideoList;
            set => _dvrVideoList = value;
        }

        public ReqGetDvrVideo? Request
        {
            get => _request;
            set => _request = value;
        }

        public long? Total
        {
            get => _total;
            set => _total = value;
        }
    }
}