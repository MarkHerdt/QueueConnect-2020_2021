using System;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Determines specific types to which the target attribute can be applied. 
    /// </summary>
    [AttributeTarget(typeof(Attribute), Inherited = true)]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AttributeTargetAttribute : Attribute
    {
        #region --- [PROPERTIES] ---

        /// <summary>
        /// Are types permitted that are a subclass of or are derived from the specified types.
        /// </summary>
        public bool Inherited { get; set; } = true;

        /// <summary>
        /// Array of types to which the target attribute can be applied. 
        /// </summary>
        public Type[] PermittedTypes { get; private set; }
        
        #endregion
        
        //--------------------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// Determines specific types to which the target attribute can be applied. 
        /// </summary>
        public AttributeTargetAttribute(Type type, params Type[] types)
        {
            PermittedTypes = new Type[types.Length + 1];
            PermittedTypes[0] = type;
            for (var i = 0; i < types.Length; i++)
            {
                PermittedTypes[i + 1] = types[i];
            }
        }
    }
}