using System.Collections.Generic;

namespace QueueConnect.ExtensionMethods
{
    public static class QueueExtensions
    {
        /// <summary>
        /// Checks if an Object is in the Queue before returning it <br/>
        /// Returns the default value if no Object is in the Queue
        /// </summary>
        /// <typeparam name="T">Type of the Object</typeparam>
        /// <param name="_Queue"></param>
        /// <returns>Returns the first Object from the Queue or the default value of that Type</returns>
        public static T SafePeek<T>(this Queue<T> _Queue)
        {
            return _Queue.Count > 0 ? _Queue.Peek() : default;
        }

        /// <summary>
        /// Checks if an Object is in the Queue and removes it <br/>
        /// </summary>
        /// <typeparam name="T">Type of the Object</typeparam>
        /// <param name="_Queue"></param>
        /// <returns>Returns the first Object from the Queue or the default value of that Type</returns>
        public static T SaveDequeue<T>(this Queue<T> _Queue)
        {
            return _Queue.Count > 0 ? _Queue.Dequeue() : default;
        }
    }
}