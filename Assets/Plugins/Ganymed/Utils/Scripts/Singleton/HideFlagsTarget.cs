using System;

namespace Ganymed.Utils.Singleton
{
    [Flags]
    public enum HideFlagsTarget
    {
        /// <summary>
        /// The default value. If you want to set the hideFlags of a target select a different option.
        /// </summary>
        None = 0,
        
        /// <summary>
        /// Select the gameObject of the component as the target.
        /// </summary>
        GameObject = 1,
        
        /// <summary>
        /// Select the script of the component as the target.
        /// </summary>
        Script = 2,
        
        /// <summary>
        /// Select both the script and the gameObject of the component as the target.
        /// </summary>
        GameObjectAndScript = 3
    }
}