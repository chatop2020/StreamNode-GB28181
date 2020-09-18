using System;
using System.Collections.Generic;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// allowkey管理类
    /// </summary>
    [Serializable]
    public class AllowKey
    {
        private List<string> _ipArray = new List<string>();
        private string _key = null!;

        /// <summary>
        /// key值
        /// </summary>
        public string Key
        {
            get => _key;
            set => _key = value;
        }

        /// <summary>
        /// ip地址列表
        /// </summary>
        public List<string> IpArray
        {
            get => _ipArray;
            set => _ipArray = value;
        }
    }
}