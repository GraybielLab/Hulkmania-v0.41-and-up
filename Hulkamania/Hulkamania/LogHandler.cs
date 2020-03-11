using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace Brandeis.AGSOL.Hulkamania
{
    /// <summary>
    /// Decorator class for log4net. Handles checking whether log types are enabled and messages should be logged.
    /// </summary>
    
    public class LogHandler
    {
        /// <summary>
        /// The log4net ILog object.
        /// </summary>
        private ILog logger = null;

        /// <summary>
        /// Handles debug messages.
        /// </summary>
        /// <param name="msg">The log message</param>
        public void Debug(String msg){
            if (logger.IsDebugEnabled) {
                logger.Debug(msg);
            }
        }

        /// <summary>
        /// Handles info messages.
        /// </summary>
        /// <param name="msg">The log message</param>
        public void Info(String msg)
        {
            if (logger.IsInfoEnabled) {
                logger.Info(msg);
            }
        }

        /// <summary>
        /// Handles error messages.
        /// </summary>
        /// <param name="msg">The log message</param>
        public void Error(String msg)
        {
            if (logger.IsErrorEnabled) {
                logger.Error(msg);
            }
        }

        /// <summary>
        /// Handles error messages that involve an exception.
        /// </summary>
        /// <param name="msg">The log message</param>
        /// <param name="e">The Exception object</param>
        public void Error(String msg, Exception e)
        {
            if (logger.IsErrorEnabled) {
                logger.Error(msg, e);
            }
        }

        /// <summary>
        /// Handles fatal messages.
        /// </summary>
        /// <param name="msg">The log message</param>
        public void Fatal(String msg)
        {
            if (logger.IsFatalEnabled) {
                logger.Fatal(msg);
            }
        }

        /// <summary>
        /// Handles warn messages.
        /// </summary>
        /// <param name="msg">The log message</param>
        public void Warn(String msg)
        {
            if (logger.IsWarnEnabled) {
            logger.Warn(msg);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_declaringType">Type of the class that instantiated this LogHandler object</param>
        public LogHandler(Type _declaringType)
        {
            logger = LogManager.GetLogger(_declaringType);
        }

    }
}
