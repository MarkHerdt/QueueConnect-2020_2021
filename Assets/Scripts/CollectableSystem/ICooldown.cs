using System;

namespace QueueConnect.CollectableSystem
{
    public interface ICooldown
    {        
        
        string Name { get; }
        
        /// <summary>
        /// Value that determines the total cooldown of an object.
        /// </summary>
        /// <returns></returns>
        float Duration { get; }
        

        /// <summary>
        ///  Method will be invoked when a cooldown is either increased or decreased.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void OnCooldownChanged(float oldValue, float newValue);
        
        /// <summary>
        /// Method will be invoked when a cooldown has ended.
        /// </summary>
        void OnCooldownEnd();
        
        /// <summary>
        /// Method will be invoked when a cooldown has started.
        /// </summary>
        void OnCooldownStart();
        
        /// <summary>
        /// Method will be invoked when a cooldown has been cancelled.
        /// </summary>
        void OnCooldownCancel();

        /// <summary>
        /// Event is invoked after a cooldown has started.
        /// </summary>
        event Action onCooldownStart;
        
        /// <summary>
        /// Event is invoked after a cooldown has ended and was not cancelled.
        /// </summary>
        event Action onCooldownEnd;
        
        /// <summary>
        /// Event is invoked after a cooldown has been cancelled.
        /// </summary>
        event Action onCooldownCanceled;
    }
}