using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MicroLightServerRuntime.Logging.Interfaces;

namespace MicroLightServerRuntime.Logging.Implementation
{
    public class LogManager : ILogger
    {
        public bool IsDebugEnabled { get  ; set  ; }

        public void Debug(object arg)
        {
            Console.WriteLine(arg);
        }

        public void DebugFormat(object arg1, object arg2)
        {
            Console.Write(arg1);
            Console.Write(arg2);
            Console.WriteLine();
        }

        public void Error(object arg)
        {
            Console.WriteLine(arg);
         
        }
    }
}
