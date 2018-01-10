
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pdw.Core
{
    public class BaseException : Exception
    {
        public List<string> Parameters { get; set; }
        public long ErrorCode { get; set; }
        public string ErrorMessage { get; set; }        
        public List<BaseException> Errors { get; set; }

        private string _errorDetail;
        public string ErrorDetail
        {
            get 
            {
                if (string.IsNullOrEmpty(_errorDetail))
                    return base.StackTrace;

                return _errorDetail;
            }
            set
            {
                _errorDetail = value;
            }
        }

        private BaseException()
        {
            Parameters = new List<string>();
            Errors = new List<BaseException>();
        }

        protected BaseException(long errorCode)
            : this()
        {
            this.ErrorCode = errorCode;
        }

        private BaseException(long errorCode, string errorMessage)
            : this(errorCode)
        {
            this.ErrorMessage = errorMessage;
        }

        protected BaseException(long errorCode, string errorMessage, string errorDetail)
            : this(errorCode, errorMessage)
        {
            _errorDetail = errorDetail;
        }

        public override string Message
        {
            get
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                    return ErrorMessage;

                return base.Message;
            }
        }
    }
}