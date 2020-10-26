using System;

namespace CommonFunctions.ManageStructs
{
    /// <summary>
    /// 返回状态结构
    /// </summary>
    [Serializable]
    public class ResponseStruct
    {
        private ErrorNumber code;
        private string message = null!;

        /// <summary>
        /// 返回结构体构造
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ResponseStruct(ErrorNumber code, string message)
        {
            Code = code;
            Message = message;
        }

        public ResponseStruct()
        {
        }

        /// <summary>
        /// 状态代码
        /// </summary>
        public ErrorNumber Code
        {
            get => code;
            set => code = value;
        }

        /// <summary>
        /// 状态描述
        /// </summary>
        public string Message
        {
            get => message;
            set => message = value;
        }
    }
}