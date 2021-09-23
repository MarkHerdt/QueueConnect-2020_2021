namespace Ganymed.Utils
{
    public enum InvokeOrigin
    {
        /// <summary>
        /// Method was invoked by unknown source
        /// </summary>
        Unknown,
        
        /// <summary>
        /// Method was invoked by instance
        /// </summary>
        Instance,
        
        /// <summary>
        /// Method was invoked by recompile callback
        /// </summary>
        Recompile,
        
        /// <summary>
        /// Method was invoked by OnGUI callback
        /// </summary>
        GUI,
        
        /// <summary>
        /// Method was invoked by constructor
        /// </summary>
        Constructor,
        
        /// <summary>
        /// Method was invoked by initialization event
        /// </summary>
        Initialization,
        
        /// <summary>
        /// Method was invoked by termination event (OnDestroy or Destructor)
        /// </summary>
        Termination,
        
        /// <summary>
        /// Event was invoked by termination of the application
        /// </summary>
        ApplicationQuit,
        
        /// <summary>
        /// Event was invoked by entering/exiting Play/EditMode 
        /// </summary>
        ApplicationStateChanged,
        
        /// <summary>
        /// Event was invoked by Unity UnityMessage (Update, Awake etc.)
        /// </summary>
        UnityMessage,
        
        /// <summary>
        /// Event was invoked while building player
        /// </summary>
        BuildPlayer,
    }
}