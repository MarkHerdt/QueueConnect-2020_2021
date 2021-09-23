using System;

namespace Ganymed.Utils
{
    public interface IState : IEnabled, IActive, IVisible
    {
        
        /// <summary>
        /// Set the enabled, visible and visibility state of the object instance.
        /// </summary>
        /// <param name="value"></param>
        void SetStates(bool value);
        
        /// <summary>
        /// Is the object enabled, visible and visible
        /// </summary>
        bool IsEnabledActiveAndVisible { get; }

        //--------------------------------------------------------------------------------------------------------------
        
        event ActiveAndVisibleDelegate OnAnyStateChanged;
    }
}