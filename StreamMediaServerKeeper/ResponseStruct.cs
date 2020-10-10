using System;

namespace StreamMediaServerKeeper
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ResponseStruct
    {
        private ErrorNumber _code;
        private string _message = null!;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ResponseStruct(ErrorNumber code, string message)
        {
            Code = code;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public ResponseStruct()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public ErrorNumber Code
        {
            get => _code;
            set => _code = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get => _message;
            set => _message = value;
        }
    }
}