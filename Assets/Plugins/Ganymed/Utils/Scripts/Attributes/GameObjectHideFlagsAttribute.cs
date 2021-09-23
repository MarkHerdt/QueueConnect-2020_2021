using System;
using UnityEngine;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Set HideFlags of the target behaviours gameObject.
    /// HideFlags are updated OnLoad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [AttributeTarget(typeof(MonoBehaviour), Inherited = true)]
    public class GameObjectHideFlagsAttribute : Attribute
    {
        public HideFlags hideFlags { get; }
        
        /// <summary>
        /// Set HideFlags of the target behaviours gameObject.
        /// HideFlags are updated OnLoad.
        /// </summary>
        /// <param name="hideFlags"></param>
        public GameObjectHideFlagsAttribute(HideFlags hideFlags)
        {
            this.hideFlags = hideFlags;
        }
    }

    /// <summary>
    /// Set HideFlags of the target behaviours gameObject to HideInHierarchy.
    /// For more options use the GameObjectHideFlags attribute.
    /// HideFlags are updated OnLoad.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [AttributeTarget(typeof(MonoBehaviour), Inherited = true)]
    public class GameObjectHideInHierarchy : GameObjectHideFlagsAttribute
    {
        /// <summary>
        /// Set HideFlags of the target behaviours gameObject to HideInHierarchy.
        /// For more options use the GameObjectHideFlags attribute.
        /// HideFlags are updated OnLoad.
        /// </summary>
        public GameObjectHideInHierarchy() : base(HideFlags.HideInHierarchy)
        {
        }
    }
}
