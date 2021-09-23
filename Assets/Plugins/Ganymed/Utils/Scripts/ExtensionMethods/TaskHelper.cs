using System;
using System.Threading;
using System.Threading.Tasks;
using Ganymed.Utils.Callbacks;
using UnityEngine;

namespace Ganymed.Utils.ExtensionMethods
{
    public static class TaskHelper
    {
        public static Task Then(this Task task, Action<Task> func)
        {
            return task.ContinueWith(func, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task Then<X>(this Task<X> task, Action<Task<X>> func)
        {
            return task.ContinueWith(func, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task<T> Then<T>(this Task task, Func<Task, T> func)
        {
            return task.ContinueWith<T>(func, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public static Task<T> Then<X, T>(this Task<X> task, Func<Task<X>, T> func)
        {
            return task.ContinueWith<T>(func, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Method can be used to break the current context (stacktrace). 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static Task BreakStack(this Task task) => Task.Run(() => { });

        //--------------------------------------------------------------------------------------------------------------

        private static volatile CancellationTokenSource ctsInternal = new CancellationTokenSource();


        static TaskHelper()
        {
            UnityEventCallbacks.AddEventListener(() =>
            {
                ctsInternal.Cancel();
                ctsInternal.Dispose();
                ctsInternal = new CancellationTokenSource();
            }, UnityEventType.ApplicationQuit, UnityEventType.EnteredPlayMode);
        }

        #region --- [AWAIT CONDITION] ---

        /// <summary>
        /// Task returns when the condition is met.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task AwaitCondition(Func<bool> condition, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token);
            var token = linked.Token;

            return Task.Run(() =>
            {
                try
                {
                    while (condition.Invoke())
                    {
                        token.ThrowIfCancellationRequested();
                        Thread.Sleep(sleep);
                    }
                }
                finally
                {
                    linked.Dispose();
                }
            }, token);
        }
       
        
        

        /// <summary>
        /// Task returns when the condition is met.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="ct">Cancellation Token for manual cancellation</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task AwaitCondition(Func<bool> condition, CancellationToken ct, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token, ct);
            var token = linked.Token;

            return Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                finally
                {
                    linked.Dispose();
                }
            }, token);
        }

        #endregion


        #region --- [AWAIT CONDITION SAFE (TIMEOUT)] ---

        /// <summary>
        /// Task returns when the condition is met or after a given timeout has passed.
        /// Note that unknown exceptions will be thrown.
        /// </summary>
        /// <param name="condition">Task returns if this returns true</param>
        /// <param name="timeOutMS">The timeout tolerance in milliseconds</param>
        /// <param name="logs">Optional settings determine what will be logged</param>
        /// <param name="sleep"></param>
        /// <returns></returns>
        public static Task AwaitConditionSafe(Func<bool> condition, int timeOutMS, AwaitLogs logs = AwaitLogs.TimeOut, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token);
            var token = linked.Token;

            var task = Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception exception)
                {
                    switch (exception)
                    {
                        case OperationCanceledException operationCanceledException:
                            if (logs.HasFlag(AwaitLogs.TimeOut))
                                Debug.Log($"Timeout after [{timeOutMS}]ms");
                            if (logs.HasFlag(AwaitLogs.OperationCanceledException))
                                Debug.LogException(operationCanceledException);
                            break;

                        case ThreadAbortException threadAbortException:
                            if (logs.HasFlag(AwaitLogs.ThreadAbortException))
                                Debug.LogException(threadAbortException);
                            break;

                        default:
                            Debug.LogException(exception);
                            break;
                    }
                }
            }, token);

            var timeOut = Task.Delay(timeOutMS, token);


            return Task.WhenAny(task, timeOut).Then(delegate
            {
                linked.Cancel();
                linked.Dispose();
            });
        }


        /// <summary>
        /// Task returns when the condition is met or after a given timeout has passed.
        /// Note that unknown exceptions will be thrown.
        /// </summary>
        /// <param name="condition">Task returns if this returns true</param>
        /// <param name="timeOutMS">The timeout tolerance in milliseconds</param>
        /// <param name="logs">Optional settings determine what will be logged</param>
        /// <param name="sleep"></param>
        /// <returns></returns>
        public static Task AwaitConditionSafe(Func<bool> condition, CancellationToken ct, int timeOutMS, AwaitLogs logs = AwaitLogs.TimeOut, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token, ct);
            var token = linked.Token;

            var task = Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception exception)
                {
                    switch (exception)
                    {
                        case OperationCanceledException operationCanceledException:
                            if (logs.HasFlag(AwaitLogs.TimeOut))
                                Debug.Log($"Timeout after [{timeOutMS}]ms");
                            if (logs.HasFlag(AwaitLogs.OperationCanceledException))
                                Debug.LogException(operationCanceledException);
                            break;

                        case ThreadAbortException threadAbortException:
                            if (logs.HasFlag(AwaitLogs.ThreadAbortException))
                                Debug.LogException(threadAbortException);
                            break;

                        default:
                            Debug.LogException(exception);
                            break;
                    }
                }
            }, token);

            var timeOut = Task.Delay(timeOutMS, token);


            return Task.WhenAny(task, timeOut).Then(delegate
            {
                linked.Cancel();
                linked.Dispose();
            });
        }
        

        #endregion

        
        #region --- [AWAIT CONDITION SUCCESS] ---

        /// <summary>
        /// Task returns when the condition is met or if it is cancelled.
        /// Result will be true if the task was not cancelled.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task<bool> AwaitConditionSuccess(Func<bool> condition, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token);
            var token = linked.Token;

            return Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    linked.Dispose();
                }
                return true;
            }, token);
        }


        /// <summary>
        /// Task returns when the condition is met or if it is cancelled.
        /// Result will be true if the task was not cancelled.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="ct">Cancellation Token for manual cancellation</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task<bool> AwaitConditionSuccess(Func<bool> condition, CancellationToken ct, int sleep = 10)
        {
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token, ct);
            var token = linked.Token;

            return Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch
                {
                    return false;
                }
                finally
                {
                    linked.Dispose();
                }
                return true;
            }, token);
        }

        #endregion
        

        #region --- [AWAIT CONDITION SUCCESS (TIMEOUT)] ---

        /// <summary>
        /// Task returns when the condition is met, if it is cancelled or if a timeout occurs.
        /// The result is true if the task was not canceled or a timeout occurred.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="timeOutMS">The timeout tolerance in milliseconds</param>
        /// <param name="logs">Log configuration</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task<bool> AwaitConditionSuccessSafe(Func<bool> condition, int timeOutMS, AwaitLogs logs = AwaitLogs.TimeOut, int sleep = 10)
        {
            var result = false;
            
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token);
            var token = linked.Token;

            var getInstance = Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                    result = true;
                }
                catch (Exception exception)
                {
                    switch (exception)
                    {
                        case OperationCanceledException operationCanceledException:
                            if (logs.HasFlag(AwaitLogs.TimeOut))
                                Debug.Log($"Timeout after [{timeOutMS}]ms");
                            if (logs.HasFlag(AwaitLogs.OperationCanceledException))
                                Debug.LogException(operationCanceledException);
                            break;

                        case ThreadAbortException threadAbortException:
                            if (logs.HasFlag(AwaitLogs.ThreadAbortException))
                                Debug.LogException(threadAbortException);
                            break;

                        default:
                            Debug.LogException(exception);
                            break;
                    }
                    result = false;
                }
            }, token);

            var timeOut = Task.Delay(timeOutMS, token).Then(delegate
            {
                linked.Cancel();
            });


            return Task.WhenAny(getInstance, timeOut).Then(delegate
            {
                linked.Cancel();
                linked.Dispose();
                return result;
            });
        }


        /// <summary>
        /// Task returns when the condition is met, if it is cancelled or if a timeout occurs.
        /// The result is true if the task was not canceled or a timeout occurred.
        /// </summary>
        /// <param name="condition">The condition that is awaited</param>
        /// <param name="ct">Cancellation Token for manual cancellation</param>
        /// <param name="timeOutMS">The timeout tolerance in milliseconds</param>
        /// <param name="logs">Log configuration</param>
        /// <param name="sleep">Delay in milliseconds in which the condition will be verified</param>
        /// <returns></returns>
        public static Task<bool> AwaitConditionSuccessSafe(Func<bool> condition, CancellationToken ct, 
            int timeOutMS, AwaitLogs logs = AwaitLogs.TimeOut, int sleep = 10)
        {
            var success = true;
            var linked = CancellationTokenSource.CreateLinkedTokenSource(ctsInternal.Token, ct);
            var token = linked.Token;

            var getInstance = Task.Run(delegate
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    while (condition.Invoke())
                    {
                        Thread.Sleep(sleep);
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception exception)
                {
                    success = false;
                    switch (exception)
                    {
                        case OperationCanceledException operationCanceledException:
                            if (logs.HasFlag(AwaitLogs.TimeOut))
                                Debug.Log($"Timeout after [{timeOutMS}]ms");
                            if (logs.HasFlag(AwaitLogs.OperationCanceledException))
                                Debug.LogException(operationCanceledException);
                            break;

                        case ThreadAbortException threadAbortException:
                            if (logs.HasFlag(AwaitLogs.ThreadAbortException))
                                Debug.LogException(threadAbortException);
                            break;

                        default:
                            Debug.LogException(exception);
                            break;
                    }
                }
            }, token);

            var timeOut = Task.Delay(timeOutMS, token);


            return Task.WhenAny(getInstance, timeOut).Then(delegate
            {
                linked.Cancel();
                linked.Dispose();
                return success;
            });
        }

        #endregion
        
        
    }
}
