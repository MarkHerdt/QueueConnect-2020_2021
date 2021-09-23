using System;

namespace Ganymed.Utils.ExtensionMethods
{
    [Flags]
    public enum AwaitLogs
    {
        None = 0,
            
        /// <summary>
        /// Timeout will be logged.
        /// </summary>
        TimeOut = 1,
            
        /// <summary>
        /// OperationCanceledExceptions are thrown when the task is manually canceled.
        /// </summary>
        OperationCanceledException = 2,
            
        /// <summary>
        /// ThreadAbortException are thrown when the task is canceled by the environment.
        /// </summary>
        ThreadAbortException = 4,
            
        /// <summary>
        /// Only exceptions will be logged. Timeout will not be logged even thought it is invoked by a OperationCanceledException.
        /// </summary>
        ExceptionsOnly = 6,
            
        /// <summary>
        /// Everything will be logged.
        /// </summary>
        All = 7
    }
}