using System;
using UnityEngine;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Set HideFlags of the target behaviour.
    /// HideFlags are updated OnLoad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [AttributeTarget(typeof(MonoBehaviour), Inherited = true)]
    public class HideFlagsAttribute : Attribute
    {
        public HideFlags hideFlags { get; }
        
        /// <summary>
        /// Set HideFlags of the target behaviours gameObject.
        /// HideFlags are updated OnLoad.
        /// </summary>
        /// <param name="hideFlags"></param>
        public HideFlagsAttribute(HideFlags hideFlags)
        {
            this.hideFlags = hideFlags;
        }
    }
    
    
    /// <summary>
    /// Set HideFlags of the target behaviour to HideInHierarchy.
    /// For more options use the HideFlags attribute.
    /// HideFlags are updated OnLoad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [AttributeTarget(typeof(MonoBehaviour), Inherited = true)]
    public class HideInHierarchy : HideFlagsAttribute
    {
        public HideInHierarchy() : base(HideFlags.HideInHierarchy)
        {
        }
    }
}