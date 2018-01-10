using System;

namespace Pdwx.DataObjects
{
    public class FileNotExistException : Exception
    {
        public override string Message
        {
            get
            {
                return "File path is not exist";
            }
        }
    }

    public class NotIsOpenXmlFormatException : Exception
    {
        public override string Message
        {
            get
            {
                return "This pdw file is not openxml format";
            }
        }
    }

    public class NotIsPdwFormatException : Exception
    {
        public override string Message
        {
            get
            {
                return "This file not is pdw format";
            }
        }
    }

    public class InvalidInputExtension : Exception
    {
        public override string Message
        {
            get
            {
                return PdwxConstants.InvalidInputExtension;
            }
        }
    }

    public class InvalidOutputExtension : Exception
    {
        private string _message;

        public InvalidOutputExtension()
        {
        }

        public InvalidOutputExtension(string message)
        {
            _message = message;
        }

        public override string Message
        {
            get
            {
                return string.IsNullOrEmpty(_message) ? PdwxConstants.InvalidOutputExtension : _message;
            }
        }
    }

    public class InvalidRenderSetting : Exception
    {
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
