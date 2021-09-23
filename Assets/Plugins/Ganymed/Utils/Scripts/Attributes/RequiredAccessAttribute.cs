using System;

namespace Ganymed.Utils.Attributes
{
    /// <summary>
    /// Attribute determines accessibility requirements of the member to which the target attribute can be assigned.
    /// Attributes only attribute. 
    /// </summary>
    [AttributeTarget(typeof(Attribute), Inherited = true)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredAccessAttribute : Attribute
    {
        #region --- [PROPERTIES] ---

        /// <summary>
        /// Are targets required to be static or non static.
        /// </summary>
        public bool Static
        {
            get => _static ?? false;
            set => _static = value;
        }
        
        /// <summary>
        /// Are targets required to be public or private.
        /// </summary>
        public bool Public
        {
            get => _public ?? false;
            set => _public = value;
        }
        
        
        /// <summary>
        /// Are targets required to be public (true), private (false) or neither (null).
        /// </summary>
        public bool? PublicRequiredOrNull => _public;

        /// <summary>
        /// Are targets required to be static (true), non static (false) or neither (null).
        /// </summary>
        public bool? StaticRequiredOrNull => _static;

        #endregion

        #region --- [FIELDS] ---

        private bool? _static = null;
        private bool? _public = null;

        #endregion
        
        /// <summary>
        /// Attribute determines accessibility requirements of the member to which the target attribute can be assigned.
        /// Attributes only attribute. 
        /// </summary>
        public RequiredAccessAttribute()
        {
            
        }
    }
}