namespace Ganymed.Utils
{
    public interface IEnabled
    {
        /// <summary>
        /// Set the enable state of the object instance.
        /// </summary>
        /// <param name="enable"></param>
        void SetEnabled(bool enable);
        
        /// <summary>
        /// Returns the enable state
        /// </summary>
        bool IsEnabled { get; }
        
        event EnabledDelegate OnEnabledStateChanged;
    }
}