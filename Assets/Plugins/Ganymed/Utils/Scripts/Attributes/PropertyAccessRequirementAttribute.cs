using System;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Attribute determines the read and wire accessibility required for the properties to
    /// which a target attribute can be assigned.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [AttributeTarget(typeof(Attribute), Inherited = true)]
    public sealed class PropertyAccessRequirementAttribute : Attribute
    {
        #region --- [PROPERTIES] ---
      
        /// <summary>
        ///  Does the attribute require the property to have read (get) and write (set) access.
        /// </summary>
        public bool RequiresReadAndWrite => requiresWrite && requiresRead;
        
        /// <summary>
        /// Does the attribute require the property to have write (set) access. 
        /// </summary>
        public bool RequiresWrite
        {
            get => requiresWrite;
            set => requiresWrite = value;
        }

        /// <summary>
        /// Does the attribute require the property to have read (get) access.
        /// </summary>
        public bool RequiresRead
        {
            get => requiresRead;
            set => requiresRead = value;
        }

        #endregion

        #region --- [FIELDS] ---

        private bool requiresWrite;
        private bool requiresRead;

        #endregion
        
        /// <summary>
        /// Attribute determines the read and wire accessibility required for the properties to
        /// which a target attribute can be assigned.
        /// </summary>
        public PropertyAccessRequirementAttribute()
        {
            
        }
    }
}
