using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDaggerMiner.Common;

namespace XDaggerMinerDaemon.Commands
{
    public class CommandResult
    {
        public static CommandResult OKResult()
        {
            return new CommandResult();
        }

        public static CommandResult CreateResult(object data)
        {
            return new CommandResult(data);
        }

        public static CommandResult ErrorResult(int code, string errorMessage)
        {
            return new CommandResult(code, errorMessage);
        }

        public static CommandResult ErrorResult(DaemonErrorCode code, string errorMessage)
        {
            return new CommandResult(code.GetHashCode(), errorMessage);
        }

        public static CommandResult ErrorResult(TargetExecutionException executionException)
        {
            return new CommandResult(executionException.ErrorCode, executionException.Message);
        }

        public CommandResult()
            : this(EmptyResultData.Create())
        {
        }

        public CommandResult(object data)
        {
            this.Code = 0;
            this.Data = JsonConvert.SerializeObject(data);
        }

        public CommandResult(int code, string errorMessage)
        {
            this.Code = code;
            this.ErrorMessage = errorMessage;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int Code
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage
        {
            get; private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Data
        {
            get; private set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Code == 0)
            {
                return string.Format("0||{0}", this.Data);
            }
            else
            {
                return string.Format("{0}||{1}", this.Code, this.ErrorMessage);
            }
        }
    }

    public class EmptyResultData
    {
        public static EmptyResultData Create()
        {
            return new EmptyResultData();
        }
    }
}
