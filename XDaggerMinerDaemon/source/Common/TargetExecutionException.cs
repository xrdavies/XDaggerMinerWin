using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;

namespace XDaggerMiner.Common
{
    public class TargetExecutionException : Exception
    {
        public int ErrorCode
        {
            get; private set;
        }

        public TargetExecutionException()
        {

        }

        public TargetExecutionException(int errorCode, Exception exception)
            : base(exception.Message, exception)
        {
            this.ErrorCode = errorCode;
        }

        public TargetExecutionException(DaemonErrorCode errorCode, Exception exception)
            : base(exception.Message, exception)
        {
            this.ErrorCode = errorCode.GetHashCode();
        }

        public TargetExecutionException(int errorCode, string message)
           : base(message)
        {
            this.ErrorCode = errorCode;
        }

        public TargetExecutionException(DaemonErrorCode errorCode, string message)
            : base(message)
        {
            this.ErrorCode = errorCode.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}||{1}", this.ErrorCode, this.Message);
        }
    }
}
