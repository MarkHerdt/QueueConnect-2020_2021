namespace Ganymed.Utils
{
    public interface IActive
    {
        /// <summary>
        /// Set the visible state of the object instance.
        /// </summary>
        /// <param name="active"></param>
        void SetActive(bool active);
        
        /// <summary>
        /// Returns the visible state
        /// </summary>
        bool IsActive { get; }
        
        event ActiveDelegate OnActiveStateChanged;
    }
}