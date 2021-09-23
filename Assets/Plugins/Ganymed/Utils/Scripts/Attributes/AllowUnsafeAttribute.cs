using System;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Base class for attributes that suppress exceptions on "unsafe" Types that would otherwise provoke an exception.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    [RequiresAdditionalAttributes(typeof(Attribute), Inherited = true)]
    public abstract class AllowUnsafeAttribute : Attribute
    {
        /// <summary>
        /// Base class for attributes that suppress exceptions on "unsafe" Types that would otherwise provoke an exception.
        /// </summary>
        protected AllowUnsafeAttribute() { }
    }
}
