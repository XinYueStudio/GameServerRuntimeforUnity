using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroLightServerRuntime.Logging.Interfaces
{
 public interface    ILogger
    {

        bool IsDebugEnabled { get; set; }
        void Debug(object arg);
        void DebugFormat(object arg1,object arg2);
        void Error(object arg);
    }
}
