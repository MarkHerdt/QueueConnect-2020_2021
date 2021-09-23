namespace Ganymed.Utils
{
    public interface IVisible
    {
        /// <summary>
        /// Set the visibility of the object instance. Objects can only be visible if they are visible and enabled
        /// </summary>
        /// <param name="visible"></param>
        void SetVisible(bool visible);
        
        /// <summary>
        /// Is the object visible
        /// </summary>
        bool IsVisible { get;}
        
        event VisibilityDelegate OnVisibilityStateChanged;
    }
}