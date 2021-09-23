namespace Ganymed.Utils.ExtensionMethods
{
    /// <summary>
    /// Extension Methods for integer
    /// </summary>
    public static class IntExtensions
    {

        /// <summary>
        /// set minimum value for the target.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int Min(this int origin, int min)
            => origin < min ? min : origin;
        
        /// <summary>
        /// set a maximum value for the target.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Max(this int origin, int max)
            => origin > max ? max : origin;
        

        /// <summary>
        /// Is the integer even.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsEven(this int n) 
            => (n ^ 1) == n + 1;
        
        /// <summary>
        /// Is the integer odd
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static bool IsOdd(this int n) 
            => (n ^ 1) != n + 1;

    }
}
